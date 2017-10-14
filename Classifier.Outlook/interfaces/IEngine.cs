using System;
using Classifier.Interfaces;

namespace myoddweb.classifier.interfaces
{
  public interface IEngine : IClassify, IMagnets, ICategories
  {
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
    /// The classification engine.
    /// </summary>
    IClassify1 ClassifyEngine { get; }
    
    /// <summary>
    /// Get the current version number of the engine.
    /// </summary>
    /// <returns>Version the engine version number</returns>
    Version GetEngineVersion();
  }
}
