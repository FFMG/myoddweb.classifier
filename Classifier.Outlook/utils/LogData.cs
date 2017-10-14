using myoddweb.classifier.core;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.utils
{
  public class LogData
  {
    /// <summary>
    /// The log level
    /// </summary>
    public LogLevels Level { get; set; }

    /// <summary>
    /// The message itself.
    /// </summary>
    public string Message { get; set; }
  }
}
