using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myoddweb.viewer.utils;
using myoddweb.classifier.utils;
using System.Linq;
using System.Threading;
using Outlook = Microsoft.Office.Interop.Outlook;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.core
{
  public class MailProcessor
  {
    private const string ConfigName = "Processor.LastEmail";

    /// <summary>
    /// Our timer
    /// </summary>
    private System.Timers.Timer _ticker;

    /// <summary>
    /// The engine that will help us to process the mail.
    /// </summary>
    private readonly IEngine _engine;

    /// <summary>
    /// The current session
    /// </summary>
    private readonly Outlook._NameSpace _session;

    /// <summary>
    /// The mail items we want to move and the categories we are moving them to.
    /// </summary>
    private List<string> _mailItems;

    /// <summary>
    /// Our lock...
    /// </summary>
    private ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

    /// <summary>
    /// Get the last time we processed an email
    /// Or the current time we we don't have a valid value.
    /// </summary>
    public DateTime LastProcessed {
      get
      {
        Lock.EnterReadLock();
        try
        {
          var last = Convert.ToInt32(_engine.Config.GetConfigWithDefault(ConfigName, "-1"));
          if (-1 == last)
          {
            return DateTime.Now;
          }
          return Helpers.UnixToDateTime(last);
        }
        finally
        {
          Lock.ExitReadLock();
        }
      }
    }

    public MailProcessor(IEngine engine, Outlook._NameSpace session )
    {
      // can't be null
      if (engine == null)
      {
        throw new ArgumentNullException(nameof(engine));
      }

      if (session == null)
      {
        throw new ArgumentNullException(nameof(session));
      }

      _engine = engine;
      _session = session;
      _mailItems = new List<string>();
    }

    /// <summary>
    /// Add a mail item to be moved to the folder.
    /// </summary>
    /// <param name="entryIdItem">The item we are moving.</param>
    public void Add(string entryIdItem)
    {
      Add(new List<string> { entryIdItem });
    }

    /// <summary>
    /// Add a range of mail entry ids to our list.
    /// </summary>
    /// <param name="ids"></param>
    public void Add(List<string> ids)
    {
      Lock.EnterWriteLock();
      try
      {
        AddInLock(ids);
      }
      finally
      {
        Lock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Add a range of mail entry ids to our list.
    /// We are inside a lock.
    /// </summary>
    /// <param name="ids"></param>
    private void AddInLock(List<string> ids)
    { 
      //  anything to do?
      if ( !ids.Any() )
      {
        return;
      }

      //  add this item to our list.
      _mailItems.AddRange(ids);

      //  if the delay is set to 0 then do everything now.
      if (0 == _engine.Options.ClassifyDelaySeconds)
      {
        HandleAllItemsInLock().Wait();
      }
      else
      {
        // start the timer
        StartTimerInLock();
      }
    }

    /// <summary>
    /// Start a timer inside a lock.
    /// </summary>
    private void StartTimerInLock()
    {
      //  stop it
      StopTimerInLock();

      // recreate the timer, we cannot use a value of 0 in the timer. 
      _ticker = new System.Timers.Timer(0 == _engine.Options.ClassifyDelaySeconds ? 1 : _engine.Options.ClassifyDelayMilliseconds);
      _ticker.Elapsed += async (sender, e) => await HandleTimer();
      _ticker.AutoReset = true;
      _ticker.Enabled = true;
    }
    
    /// <summary>
    /// Stop the timers while we are inside a lock
    /// </summary>
    private void StopTimerInLock()
    {
      if (_ticker == null)
      {
        return;
      }

      _ticker.Enabled = false;
      _ticker.Stop();
      _ticker.Dispose();
      _ticker = null;
    }

    private async Task<bool> HandleTimer()
    {
      Lock.EnterWriteLock();
      try
      {
        if( _ticker == null )
        {
          return false;
        }
        return await HandleTimerInLock().ConfigureAwait( false );
      }
      finally
      {
        Lock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Fired when the timer even it called.
    /// </summary>
    /// <returns></returns>
    private async Task<bool> HandleTimerInLock()
    {
      // we are inside the lock
      // so we can stop the timer
      StopTimerInLock();

      // handle all the mail items
      await HandleAllItemsInLock().ConfigureAwait( false );

      // we are done,
      return true;
    }

    /// <summary>
    /// Handle all the items that are in the queue.
    /// </summary>
    /// <returns></returns>
    private async Task HandleAllItemsInLock()
    {
      // wait for all the tasks now.
      await Task.WhenAll(_mailItems.Select(HandleItemInLock).Cast<Task>().ToArray());

      // clear the list.
      _mailItems = new List<string>();
    }

    /// <summary>
    /// Update the last actually process email.
    /// </summary>
    /// <param name="mailItem"></param>
    private void UpdateLastProcessedEmailInLock(Outlook._MailItem mailItem)
    {
      // the current time
      var when = Helpers.DateTimeToUnix(mailItem.ReceivedTime);

      // get the last processed time
      var last = Convert.ToInt32(_engine.Config.GetConfigWithDefault(ConfigName, "0"));

      // did we handle something older?
      if (last > when)
      {
        return;
      }
      _engine.Config.SetConfig(ConfigName, Convert.ToString(when));
    }

    /// <summary>
    /// Handle the email
    /// </summary>
    /// <param name="entryIdItem">The email we want to handle.</param>
    /// <returns></returns>
    private async Task<bool> HandleItemInLock( string entryIdItem)
    {
      Outlook._MailItem mailItem = null;
      try
      {
        // new email has arrived, we need to try and classify it.
        mailItem = _session.GetItemFromID(entryIdItem, System.Reflection.Missing.Value) as Outlook._MailItem;
      }
      catch (System.Runtime.InteropServices.COMException e)
      {
        _engine.Logger.LogError(e.ToString());
      }

      if (mailItem == null)
      {
        _engine.Logger.LogWarning($"Could not locate mail item {entryIdItem} to move.");
        return false;
      }

      // did we send this email?
      if( MailWasSentByUs(mailItem))
      {
        _engine.Logger.LogWarning($"Mail item {entryIdItem} was sent by us and will not be classified.");
        return false;
      }

      // either way, this is a valid 'processed' email
      UpdateLastProcessedEmailInLock(mailItem);

      // the message note.
      if (!Categories.IsUsableClassNameForClassification(mailItem.MessageClass))
      {
        return false;
      }

      // start the watch
      var watch = StopWatch.Start(_engine.Logger);

      // look for the category
      var guessCategoryResponse = await _engine.Categories.CategorizeAsync(mailItem).ConfigureAwait(false);

      // 
      var categoryId = guessCategoryResponse.CategoryId;
      var wasMagnetUsed = guessCategoryResponse.WasMagnetUsed;

      // did we find a category?
      if (-1 == categoryId)
      {
        _engine.Logger.LogVerbose($"I could not classify the new message {entryIdItem} into any categories. ('{mailItem.Subject}')");
        watch.Checkpoint("I could not classify the new message into any categories: (in: {0})");
        return false;
      }

      // 
      watch.Checkpoint($"I classified the new message category : {categoryId} (in: {{0}})");

      //
      // Do we want to train this
      var options = _engine.Options;
      if (options.ReAutomaticallyTrainMessages || (wasMagnetUsed && options.ReAutomaticallyTrainMagnetMessages))
      {
        // get the weight
        var weight = (wasMagnetUsed ? options.MagnetsWeight : 1);

        // we can now classify it.
        await _engine.Categories.ClassifyAsync(mailItem, (uint)categoryId, weight).ConfigureAwait(false);
      }

      try
      {
        // get the posible folder.
        var folder = _engine.Categories.FindFolderByCategoryId(categoryId);
        if (null == folder)
        {
          // 
          _engine.Logger.LogWarning($"Could not locate folder for category {categoryId}, cannot move item.");

          //  the user does not want to move to another folder.
          return false;
        }

        // don't move it if we don't need to.
        var currentFolder = (Outlook.MAPIFolder)mailItem.Parent;
 
        // if they are not the same, we can move it.
        if (currentFolder.EntryID == folder.Id() )
        {
          _engine.Logger.LogVerbose($"No need to move mail, '{mailItem.Subject}', to folder, '{folder.Name()}', already in folder");
          return true;
        }

        // if this is an ignored conversation, we will not move it.
        if (IsIgnoredConversation(mailItem))
        {
          _engine.Logger.LogVerbose($"Mail, '{mailItem.Subject}' is part of an ignored conversation and will not be moved." );
          return true;
        }

        // this is where we want to move to.
        var itemToFolder = (folder as OutlookFolder)?.Folder;
        if( null == itemToFolder )
        {
          throw new Exception($"The folder {folder.Name()} does not seem to be of type 'Folder' and cannot be used.");
        }

        // try and move 
        mailItem.Move(itemToFolder);

        // and log it.
        _engine.Logger.LogVerbose($"Moved mail, '{mailItem.Subject}', to folder, '{folder.Name()}'");
      }
      catch (Exception ex)
      {
        _engine.Logger.LogError(ex.ToString());
        return false;
      }
      return true;
    }

    /// <summary>
    /// Check if a certain email was sent by us.
    /// </summary>
    /// <param name="mailItem">The mail item we are looking at.</param>
    /// <returns></returns>
    private bool MailWasSentByUs(Outlook._MailItem mailItem)
    {
      // if the received by name is null, then it means we did not receive it
      // and if we did not receive it then we must have sent it...
      return mailItem?.ReceivedByName == null;
    }

    /// <summary>
    /// Check if a mail thread is ignored or not
    /// If it is ignored then there is nothing for us to do with it.
    /// Normally another rule kicks in and deletes it.
    /// </summary>
    /// <param name="mailItem"></param>
    /// <returns></returns>
    private bool IsIgnoredConversation(Outlook._MailItem mailItem)
    {
      // does the folder allow conversations?
      var folder = mailItem.Parent as Outlook.MAPIFolder;
      var store = folder?.Store;
      if (store?.IsConversationEnabled != true)
      {
        return false;
      }

      // get that conversation
      Outlook._Conversation conv = mailItem.GetConversation();
      if (conv == null)
      {
        return false;
      }

      // get the delete rule.
      var delete = conv.GetAlwaysDelete(_session.DefaultStore);
      if( Outlook.OlAlwaysDeleteConversation.olAlwaysDelete == delete )
      {
        return true;
      }

      // 
      return false;
    }
  }
}
