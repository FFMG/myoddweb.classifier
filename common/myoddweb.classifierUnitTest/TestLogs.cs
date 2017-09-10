using Microsoft.VisualStudio.TestTools.UnitTesting;
using myoddweb.classifier.core;
using myoddweb.classifier.utils;
using Newtonsoft.Json;
using System.Collections.Generic;

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

    private static string LogSource(Options.LogLevels level)
    {
      return $"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}.{level}";
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
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Error));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Warning));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Verbose));
    }

    [TestMethod]
    public void TestLogOneErrorItem()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Error;

      var message = RandomString(10);
      TheEngine.LogError(message);

      var entries = TheEngine.GetLogEntries(10);
      Assert.AreEqual(1, entries.Count);

      var entry = JsonConvert.DeserializeObject<LogData>(entries[0].Entry);
      Assert.AreEqual(message, entry.Message);
      Assert.AreEqual(Options.LogLevels.Error, entry.Level );

      Assert.AreEqual(LogSource(Options.LogLevels.Error), entries[0].Source);
    }

    [TestMethod]
    public void TestLogMultipleErrorItems()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Error;

      var messages = new List<string>();
      const int count = 10;

      for( var i = 0; i < count; ++i )
      {
        var message = RandomString(10);
        TheEngine.LogError(message);
        messages.Add(message);
      }

      // the list we are getting is from newest to oldest
      // so we have to reverse the list of messages.
      messages.Reverse();

      var get = count / 2;
      var entries = TheEngine.GetLogEntries(get);
      Assert.AreEqual(get, entries.Count);

      for( int i = 0; i < get; ++i )
      {
        var message = messages[i];
        var entry = JsonConvert.DeserializeObject<LogData>(entries[i].Entry);
        Assert.AreEqual(message, entry.Message);
        Assert.AreEqual(Options.LogLevels.Error, entry.Level);

        Assert.AreEqual(LogSource(Options.LogLevels.Error), entries[i].Source);
      }
    }
  }
}