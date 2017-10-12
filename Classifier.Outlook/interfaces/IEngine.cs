using System;
using Classifier.Interfaces;
using myoddweb.classifier.core;

namespace myoddweb.classifier.interfaces
{
  public interface IEngine : ILogger, IConfig, IClassify, IMagnets, ICategories
  {
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

    /// <summary>
    /// Get the root folder.
    /// @todo this should not be in this interface... it is to MS Outlook specific.
    /// </summary>
    /// <returns></returns>
    Microsoft.Office.Interop.Outlook.MAPIFolder GetRootFolder();
  }
}
