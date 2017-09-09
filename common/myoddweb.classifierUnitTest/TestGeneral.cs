using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace myoddweb.classifierUnitTest
{
  [TestClass]
  public class TestGeneral : TestCommon
  {
    public TestGeneral()
    {
      ReleaseEngine(false);
    }

    [TestCleanup]
    public void CleanupTest()
    {
      ReleaseEngine(false);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      ReleaseEngine(true);
    }

    [TestMethod]
    public void TestGetVersionNumber()
    {
      var versionNumber = TheEngine.GetEngineVersionNumber();
      Assert.AreEqual(1006002, versionNumber );
    }

    [TestMethod]
    public void TestGetVersion()
    {
      var version = TheEngine.GetEngineVersion();
      Assert.AreEqual( new Version(1, 6, 2, 0), version);
    }
  }
}
