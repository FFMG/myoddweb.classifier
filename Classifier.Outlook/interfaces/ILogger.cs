using Classifier.Interfaces.Helpers;
using System.Collections.Generic;

namespace myoddweb.classifier.interfaces
{
  public enum LogLevels
  {
    None,
    Error,
    Warning,
    Information,
    Verbose
  }

  public interface ILogger
  {
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
    /// Get up to 'max' log entries.
    /// </summary>
    /// <param name="max">The max number of log entries we want to get.</param>
    /// <returns></returns>
    List<LogEntry> GetLogEntries(int max);

    /// <summary>
    /// Log a message to the database
    /// </summary>
    /// <param name="source">Unique to the souce, something like "myapp.information", max 255 chars</param>
    /// <param name="entry">The entry we are logging, max 1024 chars.</param>
    /// <returns>the entry id.</returns>
    int Log(string source, string entry);

    /// <summary>
    /// Clear log entries that are older than a certain date
    /// </summary>
    /// <param name="olderThan"> the date we want to delte.</param>
    /// <returns>success or not</returns>
    bool ClearLogEntries(int olderThan);
  }
}
