using System.Collections.Generic;
using Classifier.Interfaces;
using Classifier.Interfaces.Helpers;
using myoddweb.classifier.interfaces;
using Newtonsoft.Json;
using myoddweb.classifier.utils;

namespace myoddweb.classifier.core
{
  public class Logger : ILogger
  {
    /// <summary>
    /// The classification engine.
    /// </summary>
    private readonly IClassify1 _classifyEngine;

    /// <summary>
    /// The options
    /// </summary>
    private readonly IOptions _options;

    public Logger(IClassify1 classifyEngine, IOptions options )
    {
      _classifyEngine = classifyEngine;
      _options = options;
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
      if (!_options.CanLog(level))
      {
        return;
      }

      //  create the json string.
      var lm = new LogData { Level = level, Message = message };
      var json = JsonConvert.SerializeObject(lm, Formatting.None);

      // log the string now.
      Log(LogSource(level), json);
    }

    private static string LogSource(LogLevels level)
    {
      return $"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}.{level}";
    }

    /// <summary>
    /// Log a message to the database
    /// </summary>
    /// <param name="source">Unique to the souce, something like "myapp.information", max 255 chars</param>
    /// <param name="entry">The entry we are logging, max 1024 chars.</param>
    /// <returns>the entry id.</returns>
    public int Log(string source, string entry)
    {
      return _classifyEngine?.Log(source, entry) ?? -1;
    }

    /// <summary>
    /// Get a list of log entries.
    /// </summary>
    /// <param name="max">The max number of entries we are getting.</param>
    /// <returns>the entries or null if there was an error.</returns>
    public List<LogEntry> GetLogEntries(int max)
    {
      // the entries we gor
      var entries = new List<LogEntry>();

      // return null if there was an error, errors are logged.
      return -1 == (_classifyEngine?.GetLogEntries(out entries, max) ?? -1) ? null : entries;
    }

    /// <summary>
    /// Clear log entries that are older than a certain date
    /// </summary>
    /// <param name="olderThan"> the date we want to delte.</param>
    /// <returns>success or not</returns>
    public bool ClearLogEntries(int olderThan)
    {
      return _classifyEngine?.ClearLogEntries(olderThan) ?? false;
    }
  }
}
