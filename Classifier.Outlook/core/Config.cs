using myoddweb.classifier.interfaces;
using Classifier.Interfaces;
using System.Collections.Generic;

namespace myoddweb.classifier.core
{
  public class Config : IConfig
  {
    /// <summary>
    /// The classification engine.
    /// </summary>
    private readonly IClassify1 _classifyEngine;

    public Config(IClassify1 classifyEngine)
    {
      _classifyEngine = classifyEngine;
    }

    public string GetConfig(string configName)
    {
      string configValue;
      if (!_classifyEngine.GetConfig(configName, out configValue))
      {
        throw new KeyNotFoundException("The value could not be found!");
      }
      return configValue;
    }

    /// <summary>
    /// Same as GetConfig( ... ) but if the value does not exist we will return the default.
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public string GetConfigWithDefault(string configName, string defaultValue)
    {
      try
      {
        return GetConfig(configName);
      }
      catch (KeyNotFoundException)
      {
        return defaultValue;
      }
    }

    public bool SetConfig(string configName, string configValue)
    {
      return _classifyEngine?.SetConfig(configName, configValue) ?? false;
    }
  }
}
