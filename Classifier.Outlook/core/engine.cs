using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Classifier.Interfaces;
using myoddweb.classifier.utils;
using System.Linq;
using Classifier.Interfaces.Helpers;

namespace myoddweb.classifier.core
{
  public class Engine
  {
    /// <summary>
    /// If NULL we have not check for event source.
    /// Any other value, we will check for.
    /// </summary>
    private bool? _eventSource = null;

    /// <summary>
    /// Name for logging in the event viewer,
    /// </summary>
    private const string EventViewSource = "myoddweb.classifier";

    /// <summary>
    /// All the options
    /// </summary>
    private Options _options;

    /// <summary>
    /// all the categories.
    /// </summary>
    private Categories _categories;

    /// <summary>
    /// Public accessor of the options.
    /// </summary>
    public Options Options => _options ?? (_options = new Options(this));

    public Categories Categories => _categories ?? (_categories = new Categories(this));

    /// <summary>
    /// The classification engine.
    /// </summary>
    public IClassify1 ClassifyEngine { get; private set; }

    internal uint? _minPercentage;

    private Microsoft.Office.Interop.Outlook.MAPIFolder _rootFolder;

    /// <summary>
    /// (re) Check all the categories all the time.
    /// This is on by default as we have the other default option "CheckIfUnownCategory" also set to on.
    /// The net effect of that would be to only check if we don't already know the value.
    /// </summary>
    public uint MinPercentage
    {
      get
      {
        return (uint)(_minPercentage ??
                       (_minPercentage = ( Convert.ToUInt32( GetConfigWithDefault("Option.MinPercentage", "75")))));
      }
      set
      {
        _minPercentage = value;
        SetConfig("Option.MinPercentage", Convert.ToString(value) );
      }
    }

    public Engine()
    {
      if (!InitialiseEngine())
      {
        throw new Exception("I was unable to load the engine. Check the event log for errors.");
      }
    }

    public Engine(string directoryName, string databasePath)
    {
      if (!InitialiseEngine(directoryName, databasePath))
      {
        throw new Exception( "One or more parameters given are invalid." );
      }
    }

    ~Engine()
    {
      ReleaseEngine();
    }
    
    public void Release()
    {
      ReleaseEngine();
    }

    /// <summary>
    /// Initialise the engine and load all the resources neeed.
    /// Will load the database and so on to get the plugin ready for use.
    /// </summary>
    /// <returns>boolean success or not.</returns>
    private bool InitialiseEngine()
    {
      //  reset it all.
      ClassifyEngine = null;

      // the paths we will be using.
      var directoryName = AppDomain.CurrentDomain.BaseDirectory;

      //  the database path
      // %appdata%\MyOddWeb\Classifier
      var myAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyOddWeb\\Classifier");
      System.IO.Directory.CreateDirectory(myAppData);
      var databasePath = Path.Combine( myAppData, "database.classifier" );

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
    private bool InitialiseEngine(string directoryName, string databasePath)
    {
      var dllInteropPath = Path.Combine( directoryName, "x86\\Classifier.Interop.dll");
      var dllEnginePath = Path.Combine(directoryName, "x86\\Classifier.Engine.dll" );
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
          LogEventError($"Unable to load the interop file. '{dllInteropPath}'.");
          return false;
        }
      }
      catch (ArgumentException ex)
      {
        LogEventError($"The interop file name/path does not appear to be valid. '{dllInteropPath}'.{Environment.NewLine}{Environment.NewLine}{ex.Message}");
        return false;
      }
      catch (FileNotFoundException ex)
      {
        LogEventError($"Unable to load the interop file. '{dllInteropPath}'.{Environment.NewLine}{Environment.NewLine}{ex.Message}");
        return false;
      }

      // look for the interop interface
      ClassifyEngine = TypeLoader.LoadTypeFromAssembly<Classifier.Interfaces.IClassify1>(asm);
      if (null == ClassifyEngine)
      {
        // could not locate the interface.
        LogEventError($"Unable to load the IClasify1 interface in the interop file. '{dllInteropPath}'.");
        return false;
      }

      // initialise the engine itself.
      return ClassifyEngine.Initialise(EventViewSource, dllEnginePath, databasePath);
    }

    /// <summary>
    /// Release the engine and do all the cleanup needed.
    /// Normally closed when the app is closing down.
    /// </summary>
    private void ReleaseEngine()
    {
      //  do we have an engine to release?
      if (null == ClassifyEngine)
      {
        return;
      }

      // release it then.
      ClassifyEngine.Release();

      // and free the memory
      ClassifyEngine = null;
    }

    private bool InstallAndValidateSource()
    {
      if (null != _eventSource)
      {
        return (bool) _eventSource;
      }

      try
      {
        if (!System.Diagnostics.EventLog.SourceExists(EventViewSource))
        {
          System.Diagnostics.EventLog.CreateEventSource(EventViewSource, null);
        }

        // set the value.
        _eventSource = System.Diagnostics.EventLog.SourceExists(EventViewSource);
      }
      catch (System.Security.SecurityException)
      {
        _eventSource = false;
      }

      // one last check.
      return InstallAndValidateSource();
    }
    
    public void LogEventError(string sEvent)
    {
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = EventViewSource };
      appLog.WriteEntry(sEvent, System.Diagnostics.EventLogEntryType.Error);
    }

    public void LogEventWarning(string sEvent)
    {
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = EventViewSource };
      appLog.WriteEntry(sEvent, System.Diagnostics.EventLogEntryType.Warning);
    }

    public void LogEventInformation(string sEvent)
    {
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = EventViewSource };
      appLog.WriteEntry(sEvent, System.Diagnostics.EventLogEntryType.Information);
    }

    /// <summary>
    /// Get the current version number of the engine.
    /// </summary>
    /// <returns>int the engine version number</returns>
    public int GetEngineVersionNumber()
    {
      return ClassifyEngine.GetEngineVersion();
    }

    /// <summary>
    /// Get the current version number of the engine.
    /// </summary>
    /// <returns>Version the engine version number</returns>
    public Version GetEngineVersion()
    {
      //  get the version
      var engineVersion = GetEngineVersionNumber();
      var major = (int)(engineVersion / 1000000.0);

      engineVersion -= (major * 1000000);
      var minor = (int)(engineVersion / 1000.0);

      engineVersion -= (minor * 1000);
      var build = engineVersion;
      return new Version( major, minor, build, 0 );
    }

    public string GetConfig(string configName )
    {
      string configValue;
      if (!ClassifyEngine.GetConfig(configName, out configValue))
      {
        throw new KeyNotFoundException("The value could not be found!");
      }
      return configValue;
    }

    /// <summary>
    /// Same as GetConfig( ... ) but if the value does not exist we will return the default.
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public string GetConfigWithDefault(string configName, string defaultValue )
    {
      try
      {
        return GetConfig(configName);
      }
      catch (KeyNotFoundException)
      {
        return defaultValue;
      }
    }

    public bool SetConfig(string configName, string configValue)
    {
      return ClassifyEngine?.SetConfig(configName, configValue) ?? false;
    }

    public bool Train(string categoryName, string textToCategorise, string uniqueIdentifier, int weight )
    {
      if (weight <= 0)
      {
        throw new ArgumentException( "The weight cannot be 0 or less!" );
      }
      return ClassifyEngine?.Train(categoryName, textToCategorise, uniqueIdentifier, weight ) ?? false;
    }

    public bool UnTrain( string uniqueIdentifier, string textToCategorise)
    {
      return ClassifyEngine?.UnTrain( uniqueIdentifier, textToCategorise) ?? false;
    }

    public int GetCategory(string categoryName)
    {
      return ClassifyEngine?.GetCategory(categoryName ) ?? -1;
    }

    public int GetCategoryFromUniqueId(string uniqueIdentifier)
    {
      return ClassifyEngine?.GetCategoryFromUniqueId( uniqueIdentifier ) ?? -1;
    }

    public int Categorize(string categoryText, uint minPercentage, out List<WordCategory> wordsCategory, out Dictionary<int, double > categoryProbabilities)
    {
      wordsCategory = new List<WordCategory>();
      categoryProbabilities = new Dictionary<int, double>();

      // the category min percentage cannot be more than 100%.
      // it also cannot be less than 0, but we use a uint.
      if (minPercentage > 100)
      {
        throw new ArgumentException("The categotry minimum range cannot be more than 100%.");
      }
      return ClassifyEngine?.Categorize(categoryText, minPercentage, out wordsCategory, out categoryProbabilities ) ?? -1;
    }

    public int Categorize(string categoryText, uint minPercentage )
    {
      // the category min percentage cannot be more than 100%.
      // it also cannot be less than 0, but we use a uint.
      if (minPercentage > 100)
      {
        throw new ArgumentException("The categotry minimum range cannot be more than 100%.");
      }

      return ClassifyEngine?.Categorize(categoryText, minPercentage) ?? -1;
    }

    public int Categorize(Dictionary< Categories.MailStringCategories, string> categoryList)
    {
      return Categorize(string.Join(";", categoryList.Select(x => x.Value) ), MinPercentage );
    }

    public Dictionary<int, string> GetCategories( )
    {
      var categories = new Dictionary<int, string>();
      if (ClassifyEngine?.GetCategories(out categories) < 0 )
      {
        return new Dictionary<int, string>();
      }
      return categories;
    }

    public bool RenameCategory(string oldCategory, string newCategory)
    {
      return ClassifyEngine?.RenameCategory(oldCategory, newCategory) ?? false;
    }

    public bool DeleteCategory(string categoryName)
    {
      return ClassifyEngine.DeleteCategory(categoryName);
    }

    public void SetRootFolder(Microsoft.Office.Interop.Outlook.MAPIFolder rootFolder)
    {
      // set the root folder.
      _rootFolder = rootFolder;
    }

    public Microsoft.Office.Interop.Outlook.MAPIFolder GetRootFolder()
    {
      return _rootFolder;
    }

    public int CreateMagnet(string randomName, int ruleType, int categoryId)
    {
      return ClassifyEngine.CreateMagnet(randomName, ruleType, categoryId );
    }

    public bool DeleteMagnet(int magnetId)
    {
      return ClassifyEngine.DeleteMagnet(magnetId);
    }

    /// <summary>
    /// Update a magnet
    /// </summary>
    /// <param name="magnetId">The magnet id</param>
    /// <param name="magnetName">The magnet name</param>
    /// <param name="ruleType">The rule type we are updating to</param>
    /// <param name="categoryTarget">The target category when the rule is matched.</param>
    /// <returns></returns>
    public bool UpdateMagnet(int magnetId, string magnetName, int ruleType, int categoryTarget )
    {
      return ClassifyEngine.UpdateMagnet(magnetId, magnetName, ruleType, categoryTarget );
    }

    /// <summary>
    /// Update an existing magnet
    /// We only update it if the values do not match exactly.
    /// </summary>
    /// <param name="currentMagnet">The current magnet we might update</param>
    /// <param name="magnetName">The updated name</param>
    /// <param name="ruleType">The updated rule type</param>
    /// <param name="categoryTarget">the updated category target.</param>
    /// <returns>boolean success or not.</returns>
    public bool UpdateMagnet(Magnet currentMagnet, string magnetName, int ruleType, int categoryTarget)
    {
      // sanity check does the value exist?
      if( null == currentMagnet )
      {
        return false;
      }

      // does it already match what we have?
      if (currentMagnet.Category == categoryTarget && currentMagnet.Rule == ruleType && currentMagnet.Name == magnetName )
      {
        //  nothing to do.
        return true;
      }

      // looks like we might do an update, do it now.
      return UpdateMagnet(currentMagnet.Id, magnetName, ruleType, categoryTarget);
    }

    /// <summary>
    /// Get our complete list of magnets.
    /// </summary>
    /// <returns>List of magnets or null</returns>
    public List<Magnet> GetMagnets()
    {
      List<Magnet> magnets;
      return -1 == ClassifyEngine.GetMagnets(out magnets) ? null : magnets;
    }
  }
}
