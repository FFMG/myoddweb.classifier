using myoddweb.classifier.core;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier
{
  public partial class ThisAddIn
  {
    private Outlook._Explorers _explorers;

    private Outlook._Folders _folders;

    private CustomUI _customUI;

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
      CreateEngine();
      CreateMailProcessor();
      CreateItemMove();

      // new email arrives.
      Application.NewMailEx += Application_NewMailEx;

      // look for item moves.
      TasksController.Add(Task.Run(() => MonitorItemMove()));
     
      // do we want to check unprocessed emails?
      if (TheEngine.Options.CheckUnProcessedEmailsOnStartUp)
      {
        TasksController.Add(Task.Run(() => ParseUnprocessedEmails()));
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
      // wait for the tasks to be done
      TasksController.WaitAll();

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
    private void ParseUnprocessedEmails()
    {
      var folders = new UnProcessedFolders(TheMailProcessor, TheEngine.Logger );
      folders.Process(_folders);
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
