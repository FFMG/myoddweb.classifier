using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using myoddweb.classifier.utils;

namespace myoddweb.classifierUnitTest64
{
  [TestClass]
  public class TestHelpers
  {
    [TestMethod]
    public void TestUnixTime1970()
    {
      var u = Helpers.DateTimeToUnix(new DateTime(1970, 1, 1, 0, 0, 0));
      Assert.AreEqual(0, u);
    }

    [TestMethod]
    public void TestUnixTime1970AndOneSecond()
    {
      var u = Helpers.DateTimeToUnix(new DateTime(1970, 1, 1, 0, 0, 1));
      Assert.AreEqual(1, u);
    }

    [TestMethod]
    public void TestUnixTimeExactDate()
    {
      var u = Helpers.DateTimeToUnix(new DateTime(2017, 9, 1, 0, 0, 0));
      Assert.AreEqual(1504224000, u);
    }

    [TestMethod]
    public void TestUnixTimeDob()
    {
      var u = Helpers.DateTimeToUnix(new DateTime(1974, 6, 26, 0, 0, 0));
      Assert.AreEqual(141436800, u);
    }
  }
}
