using System;
using Classifier.Interfaces;
using myoddweb.classifier.core;

namespace myoddweb.classifier.interfaces
{
  public interface IEngine : IConfig, IClassify, IMagnets, ICategories
  {
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
    Options Options { get; }

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
