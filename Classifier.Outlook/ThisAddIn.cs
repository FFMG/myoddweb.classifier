using myoddweb.classifier.core;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;
using Classifier.Interfaces;
using myoddweb.classifier.utils;
using System.Timers;

namespace myoddweb.classifier
{
  public partial class ThisAddIn
  {
    /// <summary>
    /// The timer we use to call the clean log function.
    /// </summary>
    Timer LogTimer { get; set; }


    /// <summary>
    /// Name for logging in the event viewer,
    /// </summary>
    private const string EventViewSource = "myoddweb.classifier";

    /// <summary>
    /// The engine that does the classification.
    /// </summary>
    private Engine _engine;

    private Outlook.Explorers _explorers;

    private Outlook.Folders _folders;

    private MailProcessor _mailProcessor;

    // all the ongoing tasks.
    private List<Task> _tasks;

    private Engine TheEngine => _engine ?? (_engine = new Engine( InitialiseEngine(), EventViewSource ));

    private MailProcessor TheMailProcessor => _mailProcessor ?? (_mailProcessor = new MailProcessor(TheEngine, _explorers.Application.Session));

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

      // do we want to check unprocessed emails?
      if (TheEngine.Options.CheckUnProcessedEmailsOnStartUp)
      {
        _tasks.Add(Task.Run(() => ParseUnprocessedEmails()));
      }

      // start the 'cleanup' timer.
      StartLogCleanupTimer();
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
      // stop the time
      StopLogCleanupTimer();

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
        TheMailProcessor.Add(entryIdItem);
      }
      catch (System.Exception ex)
      {
        TheEngine.LogError(ex.ToString());
        return Task.FromResult(false);
      }
      return Task.FromResult(true);
    }

    /// <summary>
    /// Initialise the engine and load all the resources neeed.
    /// Will load the database and so on to get the plugin ready for use.
    /// </summary>
    /// <returns>boolean success or not.</returns>
    private static IClassify1 InitialiseEngine()
    {
      // the paths we will be using.
      var directoryName = AppDomain.CurrentDomain.BaseDirectory;

      //  the database path
      // %appdata%\MyOddWeb\Classifier
      var myAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyOddWeb\\Classifier");
      Directory.CreateDirectory(myAppData);
      var databasePath = Path.Combine(myAppData, "database.classifier");

      // initialise the engine.
      return InitialiseEngine(directoryName, databasePath);
    }

    /// <summary>
    /// Initialise the engine and load all the resources neeed.
    /// Will load the database and so on to get the plugin ready for use.
    /// </summary>
    /// <param name="directoryName">string the directory we are loading from.</param>
    /// <param name="databasePath">string the name/path of the database we will be loading.</param>
    /// <returns></returns>
    private static IClassify1 InitialiseEngine(string directoryName, string databasePath)
    {
      var dllInteropPath = Path.Combine(directoryName, "x86\\Classifier.Interop.dll");
      var dllEnginePath = Path.Combine(directoryName, "x86\\Classifier.Engine.dll");
      if (Environment.Is64BitProcess)
      {
        dllInteropPath = Path.Combine(directoryName, "x64\\Classifier.Interop.dll");
        dllEnginePath = Path.Combine(directoryName, "x64\\Classifier.Engine.dll");
      }

      // look for the 
      Assembly asm = null;
      try
      {
        asm = Assembly.LoadFrom(dllInteropPath);
        if (null == asm)
        {
          throw new Exception($"Unable to load the interop file. '{dllInteropPath}'.");
        }
      }
      catch (ArgumentException ex)
      {
        throw new Exception($"The interop file name/path does not appear to be valid. '{dllInteropPath}'.{Environment.NewLine}{Environment.NewLine}{ex.Message}");
      }
      catch (FileNotFoundException ex)
      {
        throw new Exception($"Unable to load the interop file. '{dllInteropPath}'.{Environment.NewLine}{Environment.NewLine}{ex.Message}");
      }

      // look for the interop interface
      var classifyEngine = TypeLoader.LoadTypeFromAssembly<Classifier.Interfaces.IClassify1>(asm);
      if (null == classifyEngine)
      {
        // could not locate the interface.
        throw new Exception($"Unable to load the IClasify1 interface in the interop file. '{dllInteropPath}'.");
      }

      // initialise the engine itself.
      if (!classifyEngine.Initialise(EventViewSource, dllEnginePath, databasePath))
      {
        return null;
      }
      return classifyEngine;
    }

    // start the 'cleanup' timer.
    private void StartLogCleanupTimer()
    {
      //  stop the timer if need be.
      StopLogCleanupTimer();

      // start the new time
      LogTimer = new Timer();
      LogTimer.Elapsed += OnTimedLogEvent;
      LogTimer.Interval = 60 * 60 * 1000;  // one hour
      LogTimer.Enabled = true;
    }

    private void StopLogCleanupTimer()
    {
      LogTimer?.Stop();
      LogTimer?.Dispose();
      LogTimer = null;
    }

    private void OnTimedLogEvent(object source, ElapsedEventArgs e)
    {
      if( null == _engine )
      {
        return;
      }

      var classifyEngine = _engine.ClassifyEngine;
      if (null == classifyEngine)
      {
        return;
      }

      var options = _engine.Options;
      if (null == options )
      {
        return;
      }

      // days of retention
      var daysRetention = options.LogRetention;

      // the oldest log date
      var date = DateTime.UtcNow.AddDays(daysRetention * -1);

      // delete old entries.
      classifyEngine.ClearLogEntries(Helpers.DateTimeToUnix(date));
    }
  }
}
