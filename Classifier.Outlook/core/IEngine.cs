namespace myoddweb.classifier.core
{
  public interface IEngine
  {
    /// <summary>
    /// Public accessor of the options.
    /// </summary>
    Options Options { get; }

    /// <summary>
    /// Class to manage the categories.
    /// </summary>
    Categories Categories { get; }

    /// <summary>
    /// Same as GetConfig( ... ) but if the value does not exist we will return the default.
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    string GetConfigWithDefault(string configName, string defaultValue);

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
  }
}
