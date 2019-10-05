using myoddweb.classifier.core;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier
{
  public partial class ThisAddIn
  {
    private Outlook._Explorers _explorers;

    private Outlook._Folders _folders;

    private CustomUI _customUI;

    /// <summary>
    /// The cancellation token source, so we can start everything.
    /// </summary>
    private CancellationTokenSource _cts;

    private ItemMove TheIemMove { get; set; }

    public IEngine TheEngine { get; set; }

    public MailProcessor TheMailProcessor { get; private set; }

    private void ThisAddIn_Startup(object sender, EventArgs e)
    {
      // get the explorers.
      _explorers = Application.Explorers;

      // get all the folders.
      _folders = Application.Session.DefaultStore.GetRootFolder().Folders;

      // create all the required values.
      Create();

      // new email arrives.
      Application.NewMailEx += Application_NewMailEx;

      // start all the values
      Start();

      // do we want to check unprocessed emails?
      if (TheEngine.Options.CheckUnProcessedEmailsOnStartUp)
      {
        TasksController.Add( ParseUnprocessedEmailsAsync() );
      }

      // log the version 
      LogStartupInformation();
    }

    private void Start()
    {
      _cts = new CancellationTokenSource();

      TheMailProcessor.Start( _cts.Token );

      // look for item moves.
      TasksController.Add(Task.Run(() => MonitorItemMove()));
    }

    private void Stop()
    {
      // cancel the token
      _cts?.Cancel();

      // stop the mail processor
      TheMailProcessor.Stop();

      // wait for the tasks to be done
      TasksController.WaitAll();

      // clean up
      _cts?.Dispose();
      _cts = null;
    }

    private void Create()
    {
      CreateEngine();
      CreateMailProcessor();
      CreateItemMove();
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
      Stop();

      // unregister all the folders.
      TheIemMove = null;

      // we can now clear the engine as well.
      TheEngine = null;
    }

    /// <summary>
    /// Create the engine.
    /// </summary>
    private void CreateEngine()
    {
      // create the engine
      TheEngine = new OutlookEngine(Application.Session.DefaultStore.GetRootFolder());

      // set the engine
      _customUI.SetEngine(TheEngine);
    }

    /// <summary>
    /// Create the mail processor.
    /// </summary>
    private void CreateMailProcessor()
    {
      if (null == TheEngine)
      {
        throw new ArgumentNullException(nameof(TheEngine));
      }
      if (null == _explorers)
      {
        throw new ArgumentNullException(nameof(_explorers));
      }

      // then create the mail processor
      TheMailProcessor = new MailProcessor(TheEngine, _explorers.Application.Session);

      // the mail processor
      _customUI.SetMailProcessor(TheMailProcessor);
    }

    /// <summary>
    /// Create the item mpve 
    /// </summary>
    private void CreateItemMove()
    {
      if (null == TheMailProcessor)
      {
        throw new ArgumentNullException(nameof(TheMailProcessor));
      }
      if (null == TheEngine)
      {
        throw new ArgumentNullException(nameof(TheEngine));
      }

      // then start monitoring folders for user moving files.
      TheIemMove = new ItemMove(TheEngine.Categories, TheMailProcessor, TheEngine.Options, TheEngine.Logger);
    }

    /// <summary>
    /// Log uselfull information on startup
    /// </summary>
    private void LogStartupInformation()
    {
      if (TheEngine == null)
      {
        throw new ArgumentNullException(nameof(TheEngine));
      }

      // are we logging this?
      if (!TheEngine.Options.CanLog(LogLevels.Information))
      {
        return;
      }

      //  get the file version
      var version = GetFileVersion();

      // get the engine version.
      var engineVersion = TheEngine.GetEngineVersion();

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
    private Task ParseUnprocessedEmailsAsync()
    {
      var folders = new UnProcessedFolders(TheMailProcessor, TheEngine.Logger );
      return folders.ProcessAsync(_folders, (int)Globals.ThisAddIn.TheEngine.Options.NumberOfItemsToParse, true, _cts.Token );
    }

    /// <inheritdoc />
    /// <summary>
    /// Create a new menu item to handle inbox items. 
    /// </summary>
    /// <returns>CustomUI</returns>
    protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
    {
      _customUI = new CustomUI();
      return _customUI;
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
      TasksController.Add( Application_NewMailExAsync(entryIdItem) );
    }

    /// <summary>
    /// Called when a new email arrives.
    /// </summary>
    /// <param name="entryIdItem">The new email</param>
    /// <returns></returns>
    private async Task<bool> Application_NewMailExAsync(string entryIdItem)
    {
      try
      {
        // add it to the mail processor.
        await TheMailProcessor.AddAsync(entryIdItem);
        return true;
      }
      catch (Exception ex)
      {
        TheEngine.Logger.LogException(ex);
        return false;
      }
    }

    public void ParseFolders(IList<Outlook.MAPIFolder> folders)
    {
      // then reparse them all
      foreach (var mapiFolder in folders)
      {
        var unProcessedFolders = new UnProcessedFolders(Globals.ThisAddIn.TheMailProcessor, Globals.ThisAddIn.TheEngine.Logger);
        TasksController.Add(unProcessedFolders.ProcessAsync(mapiFolder, (int)TheEngine.Options.NumberOfItemsToParse, false, _cts.Token ));
      }

    }
  }
}
