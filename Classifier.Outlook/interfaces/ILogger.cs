using Classifier.Interfaces.Helpers;
using System.Collections.Generic;

namespace myoddweb.classifier.interfaces
{
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
  }
}
