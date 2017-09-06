using System;

namespace myoddweb.classifier.utils
{
  public class Helpers
  {
    public static int DateTimeToUnix(DateTime date)
    {
      return (int)(date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
  }
}
