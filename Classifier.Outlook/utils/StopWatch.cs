using myoddweb.classifier.core;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace myoddweb.viewer.utils
{
  public class StopWatch
  {
    private Stopwatch _stopwatch;

    private readonly Engine _engine;

    private StopWatch( Engine engine )
    {
      _stopwatch = Stopwatch.StartNew();
      _engine = engine;
    }

    ~StopWatch()
    {
      _stopwatch?.Stop();
    }

    public static StopWatch Start(Engine engine)
    {
      return new StopWatch(engine);
    }

    public void Stop(string text)
    {
      _stopwatch.Stop();
      Debug.WriteLine( text, Seconds() );
    }

    public void Checkpoint(string text)
    {
      Debug.WriteLine(text, Seconds());
      _engine?.LogVerbose(string.Format(text, Seconds()));
    }

    private double Seconds()
    {
      return (double) _stopwatch.ElapsedMilliseconds/1000;
    }
  }
}
