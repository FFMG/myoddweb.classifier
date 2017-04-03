using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace myoddweb.viewer.utils
{
  public class StopWatch
  {
    private System.Diagnostics.Stopwatch _stopwatch;

    private StopWatch()
    {
      _stopwatch = Stopwatch.StartNew();
    }

    ~StopWatch()
    {
      _stopwatch?.Stop();
    }

    public static StopWatch Start()
    {
      return new StopWatch();
    }

    public void Stop(string text)
    {
      _stopwatch.Stop();
      Debug.WriteLine(text, Seconds() );
    }

    public void Checkpoint(string text)
    {
      Debug.WriteLine(text, Seconds());
    }

    private double Seconds()
    {
      return (double) _stopwatch.ElapsedMilliseconds/1000;
    }
  }
}
