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

    private Outlook._Explorers _explorers;

    private Outlook._Folders _folders;

    private ItemMove _itemMove;

    private MailProcessor _mailProcessor;

    // all the ongoing tasks.
    private List<Task> _tasks;

    public ItemMove TheIemMove => _itemMove ?? (_itemMove = new ItemMove( TheEngine.Logger ));

    public OutlookEngine TheEngine => _engine ?? (_engine = new OutlookEngine());

    public MailProcessor TheMailProcessor => _mailProcessor ?? (_mailProcessor = new MailProcessor(TheEngine, _explorers.Application.Session));

    private void ThisAddIn_Startup(object sender, EventArgs e)
    {
      _tasks = new List<Task>();

      // tell the engine what the folders are.
      TheEngine.SetRootFolder(Application.Session.DefaultStore.GetRootFolder());

      // get the explorers.
      _explorers = Application.Explorers;

      // new email arrives.
      Application.NewMailEx += Application_NewMailEx;

      // monitor for folder changes
      _folders = Application.Session.DefaultStore.GetRootFolder().Folders;

      // look for item moves.
      _tasks.Add(Task.Run(() => MonitorItemMove()));
     
      // do we want to check unprocessed emails?
      if (TheEngine.Options.CheckUnProcessedEmailsOnStartUp)
      {
        _tasks.Add(Task.Run(() => ParseUnprocessedEmails()));
      }

      // log the version 
      LogStartupInformation();
    }

    /// <summary>
    /// Note: This coode is run when the users unloads the add-on
    ///       And the Startup function is called when the user re-loads it
    ///       So it is not a terrible idea to unregister things here
    /// Note: Outlook no longer raises this event. If you have code that 
    ///       must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ThisAddIn_Shutdown(object sender, EventArgs e)
    {
      // unregister all the folders.
      _itemMove = null;

      // wait for the tasks to be done
      Task.WaitAll(_tasks.ToArray());
      _tasks = null;

      // release the engine
      _engine?.Release();
      _engine = null;
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

    /// <summary>
    /// Start monitoring all emails moving.
    /// </summary>
    private void MonitorItemMove()
    {
      TheIemMove.Monitor(_folders);
    }

    /// <summary>
    /// parse all the unprocessed emails.
    /// </summary>
    private void ParseUnprocessedEmails()
    {
      var folders = new UnProcessedFolders(TheMailProcessor, TheEngine.Logger );
      folders.Process(_folders);
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
      Startup += ThisAddIn_Startup;
      Shutdown += ThisAddIn_Shutdown;
    }

    #endregion

    private void Application_NewMailEx(string entryIdItem)
    {
      //  just call the async version of this
      Application_NewMailExAsync(entryIdItem).Wait();
    }

    /// <summary>
    /// Called when a new email arrives.
    /// </summary>
    /// <param name="entryIdItem">The new email</param>
    /// <returns></returns>
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
