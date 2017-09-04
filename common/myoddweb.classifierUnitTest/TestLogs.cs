using Microsoft.VisualStudio.TestTools.UnitTesting;
using myoddweb.classifier.core;

namespace myoddweb.classifierUnitTest
{
  [TestClass]
  public class TestLogs : TestCommon
  {
    public TestLogs()
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
    public void TestSetLogLevelInfo()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Information;
      Assert.AreEqual(Options.LogLevels.Information, TheEngine.Options.LogLevel);
    }

    [TestMethod]
    public void TestCanLogLevelInfo()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Information;
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Error));
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Warning));
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Verbose));
    }

    [TestMethod]
    public void TestSetLogLevelNone()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.None;
      Assert.AreEqual(Options.LogLevels.None, TheEngine.Options.LogLevel);
    }

    [TestMethod]
    public void TestCanLogLevelNone()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.None;
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Error));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Warning));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Verbose));
    }

    [TestMethod]
    public void TestSetLogLevelWarning()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Warning;
      Assert.AreEqual(Options.LogLevels.Warning, TheEngine.Options.LogLevel);
    }

    [TestMethod]
    public void TestCanLogLevelWarning()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Warning;
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Error));
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Warning));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Verbose));
    }

    [TestMethod]
    public void TestSetLogLevelVerbose()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Verbose;
      Assert.AreEqual(Options.LogLevels.Verbose, TheEngine.Options.LogLevel);
    }

    [TestMethod]
    public void TestCanLogLevelVerbose()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Verbose;
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Error));
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Warning));
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Information));
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Verbose));
    }

    [TestMethod]
    public void TestSetLogLevelError()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Error;
      Assert.AreEqual(Options.LogLevels.Error, TheEngine.Options.LogLevel);
    }
    
    [TestMethod]
    public void TestCanLogLevelError()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Error;
      Assert.IsTrue( TheEngine.Options.CanLog(Options.LogLevels.Error));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Warning));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Verbose));
    }
  }
}
