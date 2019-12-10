using System.Diagnostics;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.utils
{
  public class StopWatch
  {
    private readonly Stopwatch _stopwatch;

    private readonly ILogger _logger;

    private StopWatch( ILogger logger)
    {
      _stopwatch = Stopwatch.StartNew();
      _logger = logger;
    }

    ~StopWatch()
    {
      _stopwatch?.Stop();
    }

    public static StopWatch Start(ILogger logger)
    {
      return new StopWatch(logger);
    }

    public void Stop(string text)
    {
      _stopwatch.Stop();
      Debug.WriteLine( text, Seconds() );
    }

    public void Checkpoint(string text)
    {
      Debug.WriteLine(text, Seconds());
      _logger?.LogVerbose(string.Format(text, Seconds()));
    }

    private double Seconds()
    {
      return (double) _stopwatch.ElapsedMilliseconds/1000;
    }
  }
}
