using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Classifier.Interfaces;
using myoddweb.classifier.utils;

namespace myoddweb.classifier.core
{
  public class Engine
  {
    /// <summary>
    /// The current list of folders.
    /// </summary>
    private Folders _folders;

    /// <summary>
    /// Name for logging in the event viewer,
    /// </summary>
    private const string EventViewSource = "myoddweb.classifier";

    /// <summary>
    /// The classification engine.
    /// </summary>
    private IClassify1 ClassifyEngine { get; set; }

    public Engine()
    {
      InitialiseEngine();
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

    public void SetFolders(Folders folders)
    {
      _folders = folders;
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
      var dllInteropPath = Path.Combine( directoryName, "Classifier.Interop.dll" );
      var dllEnginePath = Path.Combine(directoryName, "Classifier.Engine.dll" );

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
    
    public void LogEventError(string sEvent)
    {
      var appLog = new System.Diagnostics.EventLog { Source = EventViewSource };
      appLog.WriteEntry(sEvent, System.Diagnostics.EventLogEntryType.Error);
    }

    public void LogEventWarning(string sEvent)
    {
      var appLog = new System.Diagnostics.EventLog { Source = EventViewSource };
      appLog.WriteEntry(sEvent, System.Diagnostics.EventLogEntryType.Warning);
    }

    public void LogEventInformation(string sEvent)
    {
      var appLog = new System.Diagnostics.EventLog { Source = EventViewSource };
      appLog.WriteEntry(sEvent, System.Diagnostics.EventLogEntryType.Information);
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
      return ClassifyEngine.SetConfig(configName, configValue);
    }

    public bool Train(string categoryName, string uniqueIdentifier, string textToCategorise)
    {
      return ClassifyEngine.Train(categoryName, uniqueIdentifier, textToCategorise);
    }

    public bool UnTrain( string uniqueIdentifier, string textToCategorise)
    {
      return ClassifyEngine.UnTrain( uniqueIdentifier, textToCategorise);
    }

    public int GetCategory(string categoryName)
    {
      return ClassifyEngine.GetCategory(categoryName );
    }

    public int GetCategoryFromUniqueId(string uniqueIdentifier)
    {
      return ClassifyEngine.GetCategoryFromUniqueId( uniqueIdentifier );
    }

    public int Categorize(string categoryText)
    {
      return ClassifyEngine.Categorize(categoryText, 75/*%*/);
    }

    public int Categorize(List<string> categoryList)
    {
      return Categorize( string.Join( " ", categoryList ));
    }

    public Dictionary<int, string> GetCategories( )
    {
      var categories = new Dictionary<int, string>();
      var result = ClassifyEngine.GetCategories( out categories);
      return categories;
    }

    public bool RenameCategory(string oldCategory, string newCategory)
    {
      return ClassifyEngine.RenameCategory(oldCategory, newCategory);
    }

    public bool DeleteCategory(string categoryName)
    {
      return ClassifyEngine.DeleteCategory(categoryName);
    }

    public List<Folder> GetFolders()
    {
      return (_folders != null ? _folders.GetFolders() : new List<Folder>());
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
