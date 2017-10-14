using System;
using System.Collections.Generic;
using Classifier.Interfaces;
using System.Linq;
using Classifier.Interfaces.Helpers;
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
    /// The configuration handler.
    /// </summary>
    private IConfig _config;

    /// <summary>
    /// The magnets.
    /// </summary>
    private IMagnets _magnets;

    /// <summary>
    /// All the folders.
    /// </summary>
    private OutlookFolders _folders;

    /// <summary>
    /// The logger.
    /// </summary>
    public virtual ILogger Logger { get; set; }

    /// <summary>
    /// The logger.
    /// </summary>
    public virtual IConfig Config => _config ?? (_config = new Config(ClassifyEngine));

    /// <summary>
    /// Get the magnets
    /// </summary>
    public IMagnets Magnets => _magnets ?? (_magnets = new Magnets(ClassifyEngine));

    /// <summary>
    /// all the categories.
    /// </summary>
    private Categories _categories;

    /// <summary>
    /// Public accessor of the options.
    /// </summary>
    public IOptions Options => _options ?? (_options = new Options(Config));

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
  }
}
