using System.Collections.Generic;
using Classifier.Interfaces.Helpers;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.core
{
  public class OutlookLogger : ILogger
  {
    /// <summary>
    /// Name for logging in the event viewer,
    /// </summary>
    private readonly string _eventViewSource;

    /// <summary>
    /// The parent logger
    /// </summary>
    private readonly ILogger _parent;

    /// <summary>
    /// If NULL we have not check for event source.
    /// Any other value, we will check for.
    /// </summary>
    private bool? _eventSource;

    public OutlookLogger( string eventSource, ILogger parent )
    {
      // the parent, (can be null)
      _parent = parent;

      // the even source
      _eventViewSource = eventSource;
    }

    private bool InstallAndValidateSource()
    {
      if (null != _eventSource)
      {
        return (bool)_eventSource;
      }

      try
      {
        if (!System.Diagnostics.EventLog.SourceExists(_eventViewSource))
        {
          System.Diagnostics.EventLog.CreateEventSource(_eventViewSource, null);
        }

        // set the value.
        _eventSource = System.Diagnostics.EventLog.SourceExists(_eventViewSource);
      }
      catch (System.Security.SecurityException)
      {
        _eventSource = false;
      }

      // one last check.
      return (bool)_eventSource;
    }

    public void LogException( System.Exception e )
    {
      _parent?.LogException(e);

      // log to the event log.
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = _eventViewSource };
      appLog.WriteEntry(e.ToString(), System.Diagnostics.EventLogEntryType.Error);
    }

    /// <inheritdoc />
    public void LogDebug(string message)
    {
      _parent?.LogDebug(message);

      // log to the event log.
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = _eventViewSource };
      appLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Information);
    }

    /// <inheritdoc />
    public void LogError(string message)
    {
      _parent?.LogError(message);

      // log to the event log.
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = _eventViewSource };
      appLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Error);
    }

    /// <summary>
    /// Log a warning message
    /// </summary>
    /// <param name="message"></param>
    public void LogWarning(string message)
    {
      _parent?.LogWarning(message);

      // log to the event log.
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = _eventViewSource };
      appLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Warning);
    }

    /// <summary>
    /// Log an information message
    /// </summary>
    /// <param name="message"></param>
    public void LogInformation(string message)
    {
      _parent?.LogInformation(message);

      // log to the event log.
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = _eventViewSource };
      appLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Information);
    }

    public List<LogEntry> GetLogEntries(int max)
    {
      return _parent?.GetLogEntries(max);
    }

    public int Log(string source, string entry)
    {
      return _parent?.Log(source, entry) ?? -1;
    }

    public bool ClearLogEntries(long olderThan)
    {
      return _parent?.ClearLogEntries(olderThan) ?? false;
    }

    /// <summary>
    /// Log a verbose message
    /// </summary>
    /// <param name="message"></param>
    public void LogVerbose(string message)
    {
      _parent?.LogVerbose(message);

      // log to the event log.
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = _eventViewSource };
      appLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Information);
    }
  }
}
