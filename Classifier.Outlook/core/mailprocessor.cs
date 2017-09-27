using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
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
    private Dictionary<string, int> _mailItems;

    /// <summary>
    /// Our lock...
    /// </summary>
    private readonly object _lock = new object();

    public MailProcessor(Engine engine, Outlook.NameSpace session )
    {
      _engine = engine;
      _session = session;
      _mailItems = new Dictionary<string, int>();
    }

    /// <summary>
    /// Add a mail item to be moved to the folder.
    /// </summary>
    /// <param name="categoryId">Where we want to move this to</param>
    /// <param name="entryIdItem">The item we are moving.</param>
    public void Add(int categoryId, string entryIdItem)
    {
      lock (_lock)
      {
        //  add this item to our list.
        _mailItems[entryIdItem] = categoryId;

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
        _ticker = new Timer(20000);
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
          tasks.Add( HandleItem(mailItem.Value, mailItem.Key) );
        }

        // wait for all the tasks now.
        Task.WaitAll(tasks.ToArray());

        // clear the list.
        _mailItems = new Dictionary<string, int>();
      }
      return Task.FromResult(true);
    }

    private Task<bool> HandleItem(int categoryId, string entryIdItem)
    { 
      try
      {
        // get the posible folder.
        var folder = _engine.Categories.FindFolderByCategoryId(categoryId);
        if (null == folder)
        {
          // 
          _engine.LogWarning($"Could not locate folder for category {categoryId}, cannot move item.");

          //  the user does not want to move to another folder.
          return Task.FromResult( false );
        }

        Outlook.MailItem mailItem;
        try
        {
          // new email has arrived, we need to try and classify it.
          mailItem = _session.GetItemFromID(entryIdItem, System.Reflection.Missing.Value) as Outlook.MailItem;
        }
        catch (System.Runtime.InteropServices.COMException e)
        {
          // log it.
          _engine.LogError($"Could not locate mail item {entryIdItem} to move, an exception was thrown, {e.ToString()}.");

          // Could not find that message anymore
          return Task.FromResult(false);
        }

        if (mailItem == null)
        {
          _engine.LogWarning($"Could not locate mail item {entryIdItem} to move.");
          return Task.FromResult(false);
        }

        // this is where we want to move to.
        var itemToFolder = folder.OutlookFolder;

        // don't move it if we don't need to.
        var currentFolder = (Outlook.Folder)mailItem.Parent;

        // if they are not the same, we can move it.
        if (currentFolder.EntryID == folder.OutlookFolder.EntryID)
        {
          _engine.LogVerbose($"No need to move mail, '{mailItem.Subject}', to folder, '{itemToFolder.Name}', already in folder");
          return Task.FromResult(true);
        }

        // if this is an ignored conversation, we will not move it.
        if (!IsIgnored(mailItem))
        {
          // try and move 
          mailItem.Move(itemToFolder);
          _engine.LogVerbose($"Moved mail, '{mailItem.Subject}', to folder, '{itemToFolder.Name}'");
        }
      }
      catch (System.Exception ex)
      {
        _engine.LogError(ex.ToString());
        return Task.FromResult(false);
      }
      return Task.FromResult(true);
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
