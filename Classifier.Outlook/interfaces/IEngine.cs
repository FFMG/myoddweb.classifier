using System;

namespace myoddweb.classifier.interfaces
{
  public interface IEngine
  {
    /// <summary>
    /// The categories.
    /// </summary>
    ICategories Categories { get; }

    /// <summary>
    /// The classifucation engine.
    /// </summary>
    IClassify Classify { get; }

    /// <summary>
    /// Get the magnets
    /// </summary>
    IMagnets Magnets { get; }

    /// <summary>
    /// Get the configuration
    /// </summary>
    IConfig Config { get; }

    /// <summary>
    /// The logger.
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// All the folders
    /// </summary>
    IFolders Folders { get; }

    /// <summary>
    /// Public accessor of the options.
    /// </summary>
    IOptions Options { get; }
    
    /// <summary>
    /// Get the current version number of the engine.
    /// </summary>
    /// <returns>Version the engine version number</returns>
    Version GetEngineVersion();
  }
}
