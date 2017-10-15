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
    /// all the categories.
    /// </summary>
    protected Categories _categories;

    /// <summary>
    /// All the options
    /// </summary>
    protected IOptions _options;

    /// <summary>
    /// The configuration handler.
    /// </summary>
    protected IConfig _config;

    /// <summary>
    /// The magnets.
    /// </summary>
    protected IMagnets _magnets;

    /// <summary>
    /// All the folders.
    /// </summary>
    protected IFolders _folders;

    /// <summary>
    /// The logger
    /// </summary>
    protected ILogger _logger;

    /// <summary>
    /// The classify engne.
    /// </summary>
    protected IClassify _classify;

    /// <summary>
    /// The classifucation engine.
    /// </summary>
    public virtual IClassify Classify => _classify ?? (_classify = new Classify( ClassifyEngine, Options ));

    /// <summary>
    /// The logger.
    /// </summary>
    public virtual ILogger Logger => _logger ?? (_logger = new Logger(ClassifyEngine, Options));

    /// <summary>
    /// The logger.
    /// </summary>
    public virtual IConfig Config => _config ?? (_config = new Config(ClassifyEngine));

    /// <summary>
    /// Get the magnets
    /// </summary>
    public virtual IMagnets Magnets => _magnets ?? (_magnets = new Magnets(ClassifyEngine));

    /// <summary>
    /// Public accessor of the options.
    /// </summary>
    public virtual IOptions Options => _options ?? (_options = new Options(Config));

    /// <summary>
    /// The categories manager
    /// </summary>
    public virtual Categories Categories => _categories ?? (_categories = new Categories(this));

    /// <summary>
    /// The classification engine.
    /// </summary>
    public virtual IClassify1 ClassifyEngine { get; protected set; }

    /// <summary>
    /// Get all the folders.
    /// </summary>
    public virtual IFolders Folders => _folders ?? (_folders = new Folders());

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

    public int GetCategory(string categoryName)
    {
      return ClassifyEngine?.GetCategory(categoryName ) ?? -1;
    }

    public int GetCategoryFromUniqueId(string uniqueIdentifier)
    {
      return ClassifyEngine?.GetCategoryFromUniqueId( uniqueIdentifier ) ?? -1;
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
  }
}
