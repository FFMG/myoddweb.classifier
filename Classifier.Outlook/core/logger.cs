using System;
using System.Collections.Generic;
using Classifier.Interfaces.Helpers;
using myoddweb.classifier.interfaces;
using Classifier.Interfaces;
using Newtonsoft.Json;
using myoddweb.classifier.utils;

namespace myoddweb.classifier.core
{
  public class Logger : ILogger
  {
    /// <summary>
    /// The classification engine.
    /// </summary>
    private readonly IClassify1 ClassifyEngine;

    /// <summary>
    /// The options
    /// </summary>
    private readonly IOptions Options;

    public Logger(IClassify1 classifyEngine, IOptions options )
    {
      ClassifyEngine = classifyEngine;
      Options = options;
    }

    /// <summary>
    /// Get up to 'max' log entries.
    /// </summary>
    /// <param name="max">The max number of log entries we want to get.</param>
    /// <returns></returns>
    public List<LogEntry> GetLogEntries(int max)
    {
      // get the log entries,
      List<LogEntry> entries;
      return -1 == ClassifyEngine.GetLogEntries(out entries, max) ? null : entries;
    }

    /// <summary>
    /// Log an error message
    /// </summary>
    /// <param name="message"></param>
    public virtual void LogError(string message)
    {
      // log it
      LogMessageToEngine(message, LogLevels.Error);
    }

    /// <summary>
    /// Log a warning message
    /// </summary>
    /// <param name="message"></param>
    public virtual void LogWarning(string message)
    {
      // log it
      LogMessageToEngine(message, LogLevels.Warning);
    }

    /// <summary>
    /// Log an information message
    /// </summary>
    /// <param name="message"></param>
    public virtual void LogInformation(string message)
    {
      // log it
      LogMessageToEngine(message, LogLevels.Information);
    }

    /// <summary>
    /// Log a verbose message
    /// </summary>
    /// <param name="message"></param>
    public virtual void LogVerbose(string message)
    {
      // log it
      LogMessageToEngine(message, LogLevels.Verbose);
    }


    /// <summary>
    /// Log a message to the engine
    /// </summary>
    /// <param name="message"></param>
    /// <param name="level"></param>
    private void LogMessageToEngine(string message, LogLevels level)
    {
      // can we log this?
      if (!Options.CanLog(level))
      {
        return;
      }

      //  create the json string.
      var lm = new LogData { Level = level, Message = message };
      string json = JsonConvert.SerializeObject(lm, Formatting.None);

      // log the string now.
      ClassifyEngine.Log(LogSource(level), json);
    }

    private static string LogSource(LogLevels level)
    {
      return $"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}.{level}";
    }
  }
}
