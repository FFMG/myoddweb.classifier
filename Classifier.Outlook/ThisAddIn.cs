using myoddweb.classifier.core;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier
{
  public partial class ThisAddIn
  {
    /// <summary>
    /// The engine that does the classification.
    /// </summary>
    private OutlookEngine _engine;

    private Outlook.Explorers _explorers;

    private Outlook._Folders _folders;

    private MailProcessor _mailProcessor;

    // all the ongoing tasks.
    private List<Task> _tasks;

    public OutlookEngine TheEngine => _engine ?? (_engine = new OutlookEngine());

    public MailProcessor TheMailProcessor => _mailProcessor ?? (_mailProcessor = new MailProcessor(TheEngine, _explorers.Application.Session));

    private void ThisAddIn_Startup(object sender, EventArgs e)
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

      // log the version 
      LogStartupInformation();
      
      // do we want to check unprocessed emails?
      if (TheEngine.Options.CheckUnProcessedEmailsOnStartUp)
      {
        _tasks.Add(Task.Run(() => ParseUnprocessedEmails()));
      }
    }

    /// <summary>
    /// Log uselfull information on startup
    /// </summary>
    private void LogStartupInformation()
    {
      // are we logging this?
      if (!TheEngine.Options.CanLog(LogLevels.Information))
      {
        return;
      }

      //  get the file version
      var version = GetFileVersion();

      // get the engine version.
      var engineVersion = _engine.GetEngineVersion();

      //Version version = Assembly.GetEntryAssembly().GetName().Version;
      var text = $"Started Classifier - [{version.Major}.{version.Minor}.{version.Build}.{version.Revision}] - (Engine [{engineVersion.Major}.{engineVersion.Minor}.{engineVersion.Build}])";
      TheEngine.Logger.LogInformation( text );
    }

    private static Version GetFileVersion()
    {
      var assembly = System.Reflection.Assembly.GetExecutingAssembly();
      var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      return new Version(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
    }

    // parse all the unprocessed emails.
    private void ParseUnprocessedEmails()
    {
      var folders = new UnProcessedFolders(TheMailProcessor, TheEngine.Logger );
      folders.Process(_folders);
    }

    private void RegisterAllFolders()
    {
      foreach (Outlook.MAPIFolder folder in _folders)
      {
        folder.Items.ItemAdd += FolderItemAdd;
      }
    }

    /// <summary>
    /// Called when an item is added to a folder.
    /// </summary>
    /// <param name="item">What is been added.</param>
    private void FolderItemAdd(object item)
    {
      try
      {
        var mailItem = (Outlook._MailItem)item;
        if (mailItem == null)
        {
          return;
        }

        // the message note.
        if (!MailProcessor.IsUsableClassNameForClassification(mailItem?.MessageClass))
        {
          return;
        }

        // get the new folder id
        var folderId = ((Outlook.MAPIFolder)mailItem?.Parent).EntryID;

        // get the item id
        var itemId = mailItem?.EntryID;
      }
      catch (System.Runtime.InteropServices.COMException e)
      {
        TheEngine.Logger.LogException(e);
      }
    }

    private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
    {
      _engine?.Release();
      _engine = null;

      Task.WaitAll(_tasks?.ToArray());
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
      return new CustomUI();
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
        TheMailProcessor.Add(entryIdItem);
      }
      catch (Exception ex)
      {
        TheEngine.Logger.LogException(ex);
        return Task.FromResult(false);
      }
      return Task.FromResult(true);
    }
  }
}
