using System;
using System.Collections.Generic;
using Classifier.Interfaces;
using Classifier.Interfaces.Helpers;

namespace myoddweb.classifier.core
{
  public interface IEngine
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
    /// Same as GetConfig( ... ) but if the value does not exist we will return the default.
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    string GetConfigWithDefault(string configName, string defaultValue);

    /// <summary>
    /// Get the root folder.
    /// @todo this should not be in this interface... it is to MS Outlook specific.
    /// </summary>
    /// <returns></returns>
    Microsoft.Office.Interop.Outlook.MAPIFolder GetRootFolder();

    /// <summary>
    /// Save a configuration value
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="configValue"></param>
    /// <returns></returns>
    bool SetConfig(string configName, string configValue);

    /// <summary>
    /// Log a verbose message
    /// </summary>
    /// <param name="message"></param>
    void LogVerbose(string message);

    /// <summary>
    /// Log an error message
    /// </summary>
    /// <param name="message"></param>
    void LogError(string message);

    /// <summary>
    /// Log a warning message
    /// </summary>
    /// <param name="message"></param>
    void LogWarning(string message);

    /// <summary>
    /// Log an information message
    /// </summary>
    /// <param name="message"></param>
    void LogInformation(string message);

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
    /// Get up to 'max' log entries.
    /// </summary>
    /// <param name="max">The max number of log entries we want to get.</param>
    /// <returns></returns>
    List<LogEntry> GetLogEntries(int max);

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
