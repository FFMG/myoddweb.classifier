using myoddweb.classifier.core;

namespace myoddweb.classifier.utils
{
  public class LogData
  {
    /// <summary>
    /// The log level
    /// </summary>
    public Options.LogLevels Level { get; set; }

    /// <summary>
    /// The message itself.
    /// </summary>
    public string Message { get; set; }
  }
}
