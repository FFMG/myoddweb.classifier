namespace Classifier.Interfaces.Helpers
{
  public class LogEntry
  {
    public LogEntry()
    {
      Id = Unixtime = 0;
      Source = Entry = "";
    }

    /// <summary>
    /// The log entry id.
    /// </summary>
    public int Id { get; set; }

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
