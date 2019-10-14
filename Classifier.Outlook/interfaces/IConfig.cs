namespace myoddweb.classifier.interfaces
{
  public interface IConfig
  {
    /// <summary>
    /// Get a configuration value and throw if the value is not found/
    /// </summary>
    /// <param name="configName"></param>
    /// <returns></returns>
    string GetConfig(string configName);

    /// <summary>
    /// Same as GetConfig( ... ) but if the value does not exist we will return the default.
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    T GetConfigWithDefault<T>(string configName, T defaultValue);

    /// <summary>
    /// Save a configuration value
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="configValue"></param>
    /// <returns></returns>
    bool SetConfig<T>(string configName, T configValue);
  }
}
