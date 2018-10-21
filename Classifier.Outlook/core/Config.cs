using System;
using myoddweb.classifier.interfaces;
using Classifier.Interfaces;
using System.Collections.Generic;
using myoddweb.classifier.utils;

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

    /// <inheritdoc />
    public T GetConfigWithDefault<T>(string configName, T defaultValue)
    {
      try
      {
        // get the value
        var value = GetConfig(configName);

        // the value is saved as a string so we need to convert it to whatever.
        // otherwise we will just throw...
        return ConfigStringToType<T>(value);
      }
      catch (KeyNotFoundException)
      {
        return defaultValue;
      }
    }

    public bool SetConfig<T>(string configName, T configValue)
    {
      var s = TypeToConfigString(configValue);
      return _classifyEngine?.SetConfig(configName, s) ?? false;
    }

    /// <summary>
    /// Convert a given string to a type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    private static T ConfigStringToType<T>(string value)
    {
      // object
      if (typeof(T) == typeof(object))
      {
        return (T)Convert.ChangeType(value, typeof(T));
      }
      // string
      if (typeof(T) == typeof(string))
      {
        return (T)Convert.ChangeType(value, typeof(T));
      }
      // Int64
      if (typeof(T) == typeof(long))
      {
        return (T)Convert.ChangeType(Convert.ToInt64(value), typeof(T));
      }
      if (typeof(T) == typeof(ulong))
      {
        return (T)Convert.ChangeType(Convert.ToUInt64(value), typeof(T));
      }
      // Int32
      if (typeof(T) == typeof(int))
      {
        return (T)Convert.ChangeType(Convert.ToInt32(value), typeof(T));
      }
      if (typeof(T) == typeof(uint))
      {
        return (T)Convert.ChangeType(Convert.ToUInt32(value), typeof(T));
      }
      if (typeof(T) == typeof(DateTime))
      {
        return (T)Convert.ChangeType(new DateTime(Convert.ToInt64(value)), typeof(T));
      }
      throw new InvalidCastException($"Cannot convert value to {typeof(T)}");
    }

    /// <summary>
    /// Convert a given string to a type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string TypeToConfigString<T>(T value)
    {
      // object
      if (typeof(T) == typeof(object))
      {
        return (string)Convert.ChangeType(value, typeof(object));
      }
      // String
      if (typeof(T) == typeof(string))
      {
        return (string)Convert.ChangeType(value, typeof(string));
      }
      // Int64
      if (typeof(T) == typeof(long))
      {
        return $"{Convert.ChangeType(value, typeof(long))}";
      }
      if (typeof(T) == typeof(ulong))
      {
        return $"{Convert.ChangeType(value, typeof(ulong))}";
      }
      // Int32
      if (typeof(T) == typeof(int))
      {
        return $"{Convert.ChangeType(value, typeof(int))}";
      }
      if (typeof(T) == typeof(uint))
      {
        return $"{Convert.ChangeType(value, typeof(uint))}";
      }
      if (typeof(T) == typeof(DateTime))
      {
        var dt = (DateTime)Convert.ChangeType(value, typeof(DateTime));
        return $"{Helpers.DateTimeToUnix(dt)}";
      }
      throw new InvalidCastException($"Cannot convert value to {typeof(T)}");
    }

  }
}
