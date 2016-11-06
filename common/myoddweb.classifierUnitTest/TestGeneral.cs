﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace myoddweb.classifierUnitTest
{
  [TestClass]
  public class TestGeneral : TestCommon
  {
    public TestGeneral()
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
      var versionNumber = TheEngine.GetEngineVersion();
      Assert.AreEqual(1001000, versionNumber );
    }
  }
}