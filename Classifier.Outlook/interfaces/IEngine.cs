using System;
using System.Collections.Generic;
using Classifier.Interfaces;
using myoddweb.classifier.core;

namespace myoddweb.classifier.interfaces
{
  public interface IEngine : ILogger, IConfig, IClassify, IMagnets
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
    /// Class to manage the categories.
    /// </summary>
    Categories Categories { get; }

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
     
    /// <summary>
    /// Get all the categories.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, string> GetCategories();

    /// <summary>
    /// Get the category id given a name.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    int GetCategory(string categoryName);

    /// <summary>
    /// Get a category id given a text unique id.
    /// </summary>
    /// <param name="uniqueIdentifier">The unique id is client specific</param>
    /// <returns></returns>
    int GetCategoryFromUniqueId(string uniqueIdentifier);

    /// <summary>
    /// Rename a category
    /// </summary>
    /// <param name="oldCategory"></param>
    /// <param name="newCategory"></param>
    /// <returns></returns>
    bool RenameCategory(string oldCategory, string newCategory);

    /// <summary>
    /// Delete a category by name.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    bool DeleteCategory(string categoryName);
  }
}
