using myoddweb.classifier.core;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace myoddweb.classifier
{
  public partial class ThisAddIn
  {
    /// <summary>
    /// The engine that does the classification.
    /// </summary>
    private Engine _engine;

    private Outlook.Explorers _explorers;

    private Outlook.Folders _folders;

    private MailProcessor _mailProcessor;

    // all the ongoing tasks.
    private List<Task> _tasks;

    private Engine TheEngine => _engine ?? (_engine = new Engine());

    private MailProcessor TheMailProcessor => _mailProcessor ?? (_mailProcessor = new MailProcessor( TheEngine, _explorers.Application.Session));

    private void ThisAddIn_Startup(object sender, System.EventArgs e)
    {
      _tasks = new List<Task>();

      // tell the engine what the folders are.
      TheEngine.SetRootFolder(Application.Session.DefaultStore.GetRootFolder());

      // get the explorers.
      _explorers = this.Application.Explorers;

      // new email arrives.
      Application.NewMailEx += Application_NewMailEx;

      // monitor for folder changes
      _folders = Application.Session.DefaultStore.GetRootFolder().Folders;

      // listen for new folders
      RegisterAllFolders();

      // do we want to check unprocessed emails?
      if (TheEngine.Options.CheckUnProcessedEmailsOnStartUp)
      {
        _tasks.Add(Task.Run(() => ParseUnprocessedEmails()));
      }
    }

    // parse all the unprocessed emails.
    private void ParseUnprocessedEmails()
    {
      var folders = new UnProcessedFolders(TheEngine, TheMailProcessor);
      folders.Process(_folders);
    }

    private void RegisterAllFolders()
    {
      foreach (Outlook.Folder folder in _folders)
      {
        folder.Items.ItemAdd += FolderItemAdd;
      }
    }
    
    /// <summary>
    /// Called when an item is added to a folder.
    /// </summary>
    /// <param name="item">What is been added.</param>
    private void FolderItemAdd(object item )
    {
      try
      {
        var mailItem = (Outlook._MailItem)item;
        if (mailItem == null)
        {
          return;
        }

        // the message note.
        if (!Categories.IsUsableClassNameForClassification(mailItem?.MessageClass))
        {
          return;
        }

        // get the new folder id
        var folderId = ((Outlook.Folder)mailItem?.Parent).EntryID;

        // get the item id
        var itemId = mailItem?.EntryID;
      }
      catch (System.Runtime.InteropServices.COMException e)
      {
        TheEngine.LogError(e.ToString());
      }
    }

    private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
    {
      Task.WaitAll(_tasks?.ToArray() );
      _tasks = null;
      // Note: Outlook no longer raises this event. If you have code that 
      //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
    }

    /// <summary>
    /// Create a new menu item to handle inbox items. 
    /// </summary>
    /// <returns>CustomUI</returns>
    protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
    {
      return new CustomUI(TheEngine);
    }

    #region VSTO generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InternalStartup()
    {
      this.Startup += new System.EventHandler(ThisAddIn_Startup);
      this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
    }

    #endregion

    private void Application_NewMailEx(string entryIdItem)
    {
      //  just call the async version of this
      Application_NewMailExAsync(entryIdItem).Wait();
    }

    private Task<bool> Application_NewMailExAsync(string entryIdItem)
    {
      try
      {
        // add it to the mail processor.
        TheMailProcessor.Add( entryIdItem );
      }
      catch (System.Exception ex)
      {
        TheEngine.LogError(ex.ToString());
        return Task.FromResult(false);
      }
      return Task.FromResult(true);
    }
  }
}
