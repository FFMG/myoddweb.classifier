using System;
using Classifier.Interfaces;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.core
{
  public class Engine : IEngine
  {
    /// <summary>
    /// all the categories.
    /// </summary>
    private ICategories _categories;

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
    private IFolders _folders;

    /// <summary>
    /// The logger
    /// </summary>
    private ILogger _logger;

    /// <summary>
    /// The classify engne.
    /// </summary>
    private IClassify _classify;

    /// <summary>
    /// The logger.
    /// </summary>
    public ILogger Logger => _logger ?? (_logger = new Logger(ClassifyEngine, Options));

    /// <summary>
    /// The logger.
    /// </summary>
    public IConfig Config => _config ?? (_config = new Config(ClassifyEngine));

    /// <summary>
    /// Get the magnets
    /// </summary>
    public IMagnets Magnets => _magnets ?? (_magnets = new Magnets(ClassifyEngine));

    /// <summary>
    /// Public accessor of the options.
    /// </summary>
    public IOptions Options => _options ?? (_options = new Options(Config));

    /// <summary>
    /// The categories manager
    /// </summary>
    public ICategories Categories => _categories ?? (_categories = new Categories( ClassifyEngine, Config ));

    /// <summary>
    /// The classification engine.
    /// </summary>
    private IClassify1 ClassifyEngine { get; set; }

    /// <summary>
    /// The classifucation engine.
    /// </summary>
    public IClassify Classify => _classify ?? (_classify = new Classify(ClassifyEngine, Options));

    /// <summary>
    /// Get all the folders.
    /// </summary>
    public IFolders Folders => _folders ?? (_folders = new Folders());

    /// <summary>
    /// The engine constructor.
    /// </summary>
    /// <param name="classifyEngine">The classification engine</param>
    public Engine( IClassify1 classifyEngine )
    {
      // save the classify engine.
      // ReSharper disable once VirtualMemberCallInConstructor
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
  }
}
