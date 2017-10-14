using System;
using System.Collections.Generic;
using Classifier.Interfaces;
using myoddweb.classifier.utils;
using System.Linq;
using Classifier.Interfaces.Helpers;
using Newtonsoft.Json;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.core
{
  public class Engine : IEngine
  {
    /// <summary>
    /// All the options
    /// </summary>
    private IOptions _options;

    /// <summary>
    /// All the folders.
    /// </summary>
    private OutlookFolders _folders;

    /// <summary>
    /// The logger.
    /// </summary>
    public virtual ILogger Logger { get; set; } 

    /// <summary>
    /// all the categories.
    /// </summary>
    private Categories _categories;

    /// <summary>
    /// Public accessor of the options.
    /// </summary>
    public IOptions Options => _options ?? (_options = new Options(this));

    public Categories Categories => _categories ?? (_categories = new Categories(this));

    /// <summary>
    /// The classification engine.
    /// </summary>
    public IClassify1 ClassifyEngine { get; private set; }

    /// <summary>
    /// Get all the folders.
    /// </summary>
    public IFolders Folders => _folders ?? (_folders = new OutlookFolders(GetRootFolder()));

    private Microsoft.Office.Interop.Outlook.MAPIFolder _rootFolder;

    /// <summary>
    /// The engine constructor.
    /// </summary>
    /// <param name="classifyEngine">The classification engine</param>
    /// <param name="eventViewSource">The event log name.</param>
    public Engine( IClassify1 classifyEngine )
    {
      // save the classify engine.
      ClassifyEngine = classifyEngine;
    }

    ~Engine()
    {
      // release the engine
      Release();
    }

    public virtual void Release()
    {
      // release the engine
      ReleaseEngine();
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
      return Categorize(string.Join(";", categoryList.Select(x => x.Value) ), Options.MinPercentage );
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

    private Microsoft.Office.Interop.Outlook.MAPIFolder GetRootFolder()
    {
      return _rootFolder;
    }

    /// <summary>
    /// Create a magnet.
    /// </summary>
    /// <param name="randomName"></param>
    /// <param name="ruleType"></param>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public int CreateMagnet(string randomName, int ruleType, int categoryId)
    {
      return ClassifyEngine.CreateMagnet(randomName, ruleType, categoryId );
    }

    /// <summary>
    /// Delete a magnet given a magnet id.
    /// </summary>
    /// <param name="magnetId"></param>
    /// <returns></returns>
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
