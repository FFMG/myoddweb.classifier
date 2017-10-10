using System;
using System.Collections.Generic;
using Classifier.Interfaces;
using Classifier.Interfaces.Helpers;

namespace myoddweb.classifier.core
{
  public interface IEngine : ILogger, IConfig, IClassify
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
    /// Create a magnet.
    /// </summary>
    /// <param name="randomName"></param>
    /// <param name="ruleType"></param>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    int CreateMagnet(string randomName, int ruleType, int categoryId);

    /// <summary>
    /// Delete a magnet given a magnet id.
    /// </summary>
    /// <param name="magnetId"></param>
    /// <returns></returns>
    bool DeleteMagnet(int magnetId);

    /// <summary>
    /// Update a magnet
    /// </summary>
    /// <param name="magnetId">The magnet id</param>
    /// <param name="magnetName">The magnet name</param>
    /// <param name="ruleType">The rule type we are updating to</param>
    /// <param name="categoryTarget">The target category when the rule is matched.</param>
    /// <returns></returns>
    bool UpdateMagnet(int magnetId, string magnetName, int ruleType, int categoryTarget);

    /// <summary>
    /// Update an existing magnet
    /// We only update it if the values do not match exactly.
    /// </summary>
    /// <param name="currentMagnet">The current magnet we might update</param>
    /// <param name="magnetName">The updated name</param>
    /// <param name="ruleType">The updated rule type</param>
    /// <param name="categoryTarget">the updated category target.</param>
    /// <returns>boolean success or not.</returns>
    bool UpdateMagnet(Magnet currentMagnet, string magnetName, int ruleType, int categoryTarget);

    /// <summary>
    /// Get our complete list of magnets.
    /// </summary>
    /// <returns>List of magnets or null</returns>
    List<Magnet> GetMagnets();
     
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
