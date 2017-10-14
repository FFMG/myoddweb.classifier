using Classifier.Interfaces;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.core
{
  public class OutlookLogger : Logger
  {
    /// <summary>
    /// Name for logging in the event viewer,
    /// </summary>
    private readonly string EventViewSource;

    /// <summary>
    /// If NULL we have not check for event source.
    /// Any other value, we will check for.
    /// </summary>
    private bool? _eventSource = null;

    public OutlookLogger( string eventSource, IClassify1 classifyEngine, IOptions options) : 
      base(classifyEngine, options)
    {
      EventViewSource = eventSource;
    }

    private bool InstallAndValidateSource()
    {
      if (null != _eventSource)
      {
        return (bool)_eventSource;
      }

      try
      {
        if (!System.Diagnostics.EventLog.SourceExists(EventViewSource))
        {
          System.Diagnostics.EventLog.CreateEventSource(EventViewSource, null);
        }

        // set the value.
        _eventSource = System.Diagnostics.EventLog.SourceExists(EventViewSource);
      }
      catch (System.Security.SecurityException)
      {
        _eventSource = false;
      }

      // one last check.
      return InstallAndValidateSource();
    }

    /// <summary>
    /// Log an error message
    /// </summary>
    /// <param name="message"></param>
    public override void LogError(string message)
    {
      // log to the event log.
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = EventViewSource };
      appLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Error);
    }

    /// <summary>
    /// Log a warning message
    /// </summary>
    /// <param name="message"></param>
    public override void LogWarning(string message)
    {
      // log to the event log.
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = EventViewSource };
      appLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Warning);
    }

    /// <summary>
    /// Log an information message
    /// </summary>
    /// <param name="message"></param>
    public override void LogInformation(string message)
    {
      // log to the event log.
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = EventViewSource };
      appLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Information);
    }

    /// <summary>
    /// Log a verbose message
    /// </summary>
    /// <param name="message"></param>
    public override void LogVerbose(string message)
    {
      base.LogVerbose(message);

      // log to the event log.
      if (!InstallAndValidateSource())
      {
        return;
      }

      var appLog = new System.Diagnostics.EventLog { Source = EventViewSource };
      appLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Information);
    }
  }
}
