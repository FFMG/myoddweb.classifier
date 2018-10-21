using System;

namespace myoddweb.classifier.utils
{
  public class Helpers
  {
    public static long DateTimeToUnix(DateTime date)
    {
      return (long)(date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }

    /// <summary>
    /// Converts the given epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
    /// </summary>
    public static DateTime UnixToDateTime( long unixDateTime )
    {
      var timeInTicks = unixDateTime * TimeSpan.TicksPerSecond;
      return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(timeInTicks);
    }
  }
}
