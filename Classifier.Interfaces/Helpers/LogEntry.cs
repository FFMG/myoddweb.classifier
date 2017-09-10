namespace Classifier.Interfaces.Helpers
{
  public class LogEntry
  {
    public LogEntry()
    {
      Unixtime = 0;
      Source = Entry = "";
    }

    /// <summary>
    /// The source of the log
    /// </summary>
    public string Source{ get; set; }

    /// <summary>
    /// The log entry
    /// </summary>
    public string Entry { get; set; }

    /// <summary>
    /// The unix date/time of the log entry
    /// </summary>
    public int Unixtime { get; set; }
  }
}
