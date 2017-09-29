using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using myoddweb.viewer.utils;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier.core
{
  internal class MailProcessor
  {
    /// <summary>
    /// Our timer
    /// </summary>
    private Timer _ticker;

    /// <summary>
    /// The engine that will help us to process the mail.
    /// </summary>
    private readonly Engine _engine;

    /// <summary>
    /// The current session
    /// </summary>
    private readonly Outlook.NameSpace _session;

    /// <summary>
    /// The mail items we want to move and the categories we are moving them to.
    /// </summary>
    private List<string> _mailItems;

    /// <summary>
    /// Our lock...
    /// </summary>
    private readonly object _lock = new object();

    public MailProcessor(Engine engine, Outlook.NameSpace session )
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
      lock (_lock)
      {
        //  add this item to our list.
        _mailItems.Add( entryIdItem );

        // start the timer
        StartTimer();
      }
    }

    private void StartTimer()
    {
      lock (_lock)
      {
        //  stop it
        StopTimer();

        // recreate it.
        _ticker = new Timer(_engine.Options.ClassifyDelaySeconds);
        _ticker.Elapsed += async (sender, e) => await HandleTimer();
        _ticker.AutoReset = true;
        _ticker.Enabled = true;
      }
    }

    /// <summary>
    /// Stop the timer and reset it.
    /// </summary>
    private void StopTimer()
    {
      lock (_lock)
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
    }

    /// <summary>
    /// Fired when the timer even it called.
    /// </summary>
    /// <returns></returns>
    private Task<bool> HandleTimer()
    {
      lock (_lock)
      {
        // we are inside the lock
        // so we can stop the timer
        StopTimer();

        // handle all the mail items
        var tasks = new List<Task>();
        foreach (var mailItem in _mailItems)
        {
          tasks.Add( HandleItem(mailItem ) );
        }

        // wait for all the tasks now.
        Task.WaitAll(tasks.ToArray());

        // clear the list.
        _mailItems = new List<string>();
      }
      return Task.FromResult(true);
    }

    private async Task<bool> HandleItem( string entryIdItem)
    {
      Outlook.MailItem mailItem = null;
      try
      {
        // new email has arrived, we need to try and classify it.
        mailItem = _session.GetItemFromID(entryIdItem, System.Reflection.Missing.Value) as Outlook.MailItem;
      }
      catch (System.Runtime.InteropServices.COMException e)
      {
        _engine.LogError(e.ToString());

        // Could not find that message anymore
        // @todo log this entry id could not be located.
      }

      if (mailItem == null)
      {
        _engine.LogWarning($"Could not locate mail item {entryIdItem} to move.");
        return false;
      }

      // the message note.
      if (!Categories.IsUsableClassNameForClassification(mailItem?.MessageClass))
      {
        return false;
      }

      // start the watch
      var watch = StopWatch.Start(_engine);

      // look for the category
      var guessCategoryResponse = await _engine.Categories.CategorizeAsync(mailItem).ConfigureAwait(false);

      // 
      var categoryId = guessCategoryResponse.CategoryId;
      var wasMagnetUsed = guessCategoryResponse.WasMagnetUsed;

      // did we find a category?
      if (-1 == categoryId)
      {
        _engine.LogVerbose($"I could not classify the new message {entryIdItem} into any categories. ('{mailItem.Subject}')");
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
          _engine.LogWarning($"Could not locate folder for category {categoryId}, cannot move item.");

          //  the user does not want to move to another folder.
          return false;
        }

        // this is where we want to move to.
        var itemToFolder = folder.OutlookFolder;

        // don't move it if we don't need to.
        var currentFolder = (Outlook.Folder)mailItem.Parent;

        // if they are not the same, we can move it.
        if (currentFolder.EntryID == folder.OutlookFolder.EntryID)
        {
          _engine.LogVerbose($"No need to move mail, '{mailItem.Subject}', to folder, '{itemToFolder.Name}', already in folder");
          return true;
        }

        // if this is an ignored conversation, we will not move it.
        if (!IsIgnored(mailItem))
        {
          // try and move 
          mailItem.Move(itemToFolder);
          _engine.LogVerbose($"Moved mail, '{mailItem.Subject}', to folder, '{itemToFolder.Name}'");
        }
      }
      catch (Exception ex)
      {
        _engine.LogError(ex.ToString());
        return false;
      }
      return true;
    }

    private static bool IsIgnored(Outlook._MailItem mailItem)
    {
      // does the folder allow conversations?
      var folder = mailItem.Parent as Outlook.Folder;
      var store = folder?.Store;
      if (store?.IsConversationEnabled != true)
      {
        return false;
      }

      // get that conversation
      var conv = mailItem.GetConversation();
      if (conv == null)
      {
        return false;
      }

      // 
      return false;
    }
  }
}
