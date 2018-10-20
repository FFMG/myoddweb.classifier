using NUnit.Framework;
using System;

namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  public class TestGeneral : TestCommon
  {
    public TestGeneral()
    {
      ReleaseEngine(false);
    }

    [TearDown]
    public void CleanupTest()
    {
      ReleaseEngine(false);
    }

    [OneTimeTearDown]
    public void ClassCleanup()
    {
      ReleaseEngine(true);
    }

    [Test]
    public void TestGetVersionNumber()
    {
      var versionNumber = TheEngine.GetEngineVersionNumber();
      Assert.AreEqual(1006005, versionNumber );
    }

    [Test]
    public void TestGetVersion()
    {
      var version = TheEngine.GetEngineVersion();
      Assert.AreEqual( new Version(1, 6, 5, 0), version);
    }
  }
}
