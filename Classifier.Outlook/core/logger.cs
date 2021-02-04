using System.Collections.Generic;
using Classifier.Interfaces;
using Classifier.Interfaces.Helpers;
using myoddweb.classifier.interfaces;
using Newtonsoft.Json;
using myoddweb.classifier.utils;

namespace myoddweb.classifier.core
{
  public sealed class Logger : ILogger
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

    /// <inheritdoc />
    public void LogException(System.Exception exception)
    {
      while (true)
      {
        LogMessageToEngine(exception.ToString(), LogLevels.Exception );
        if (exception.InnerException != null)
        {
          exception = exception.InnerException;
          continue;
        }
        break;
      }
    }

    /// <inheritdoc />
    public void LogError(string message)
    {
      // log it
      LogMessageToEngine(message, LogLevels.Error);
    }

    /// <inheritdoc />
    public void LogWarning(string message)
    {
      // log it
      LogMessageToEngine(message, LogLevels.Warning);
    }

    /// <inheritdoc />
    public void LogInformation(string message)
    {
      // log it
      LogMessageToEngine(message, LogLevels.Information);
    }

    /// <inheritdoc />
    public void LogVerbose(string message)
    {
      // log it
      LogMessageToEngine(message, LogLevels.Verbose);
    }

    /// <inheritdoc />
    public void LogDebug(string message)
    {
      // log it
      LogMessageToEngine(message, LogLevels.Debug);
    }

    /// <inheritdoc />
    public int Log(string source, string entry)
    {
      return _classifyEngine?.Log(source, entry) ?? -1;
    }

    /// <inheritdoc />
    public List<LogEntry> GetLogEntries(int max)
    {
      // the entries we gor
      var entries = new List<LogEntry>();

      // return null if there was an error, errors are logged.
      return -1 == (_classifyEngine?.GetLogEntries(out entries, max) ?? -1) ? null : entries;
    }

    /// <inheritdoc />
    public bool ClearLogEntries(long olderThan)
    {
      return _classifyEngine?.ClearLogEntries(olderThan) ?? false;
    }

    #region Private Methods
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

    /// <summary>
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private static string LogSource(LogLevels level)
    {
      return $"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}.{level}";
    }
    #endregion
  }
}
