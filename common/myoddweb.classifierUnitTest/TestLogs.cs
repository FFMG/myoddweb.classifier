using NUnit.Framework;
using myoddweb.classifier.core;
using myoddweb.classifier.utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  public class TestLogs : TestCommon
  {
    public TestLogs()
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

    private static string LogSource(Options.LogLevels level)
    {
      return $"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}.{level}";
    }

    [Test]
    public void TestSetLogLevelInfo()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Information;
      Assert.AreEqual(Options.LogLevels.Information, TheEngine.Options.LogLevel);
    }

    [Test]
    public void TestCanLogLevelInfo()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Information;
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Error));
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Warning));
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Verbose));
    }

    [Test]
    public void TestSetLogLevelNone()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.None;
      Assert.AreEqual(Options.LogLevels.None, TheEngine.Options.LogLevel);
    }

    [Test]
    public void TestCanLogLevelNone()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.None;
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Error));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Warning));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Verbose));
    }

    [Test]
    public void TestSetLogLevelWarning()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Warning;
      Assert.AreEqual(Options.LogLevels.Warning, TheEngine.Options.LogLevel);
    }

    [Test]
    public void TestCanLogLevelWarning()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Warning;
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Error));
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Warning));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Verbose));
    }

    [Test]
    public void TestSetLogLevelVerbose()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Verbose;
      Assert.AreEqual(Options.LogLevels.Verbose, TheEngine.Options.LogLevel);
    }

    [Test]
    public void TestCanLogLevelVerbose()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Verbose;
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Error));
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Warning));
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Information));
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Verbose));
    }

    [Test]
    public void TestSetLogLevelError()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Error;
      Assert.AreEqual(Options.LogLevels.Error, TheEngine.Options.LogLevel);
    }

    [Test]
    public void TestCanLogLevelError()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Error;
      Assert.IsTrue(TheEngine.Options.CanLog(Options.LogLevels.Error));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Warning));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(Options.LogLevels.Verbose));
    }

    [Test]
    public void TestLogOneErrorItem()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Error;

      var message = RandomString(10);
      TheEngine.Logger.LogError(message);

      var entries = TheEngine.Logger.GetLogEntries(10);
      Assert.AreEqual(1, entries.Count);

      var entry = JsonConvert.DeserializeObject<LogData>(entries[0].Entry);
      Assert.AreEqual(message, entry.Message);
      Assert.AreEqual(Options.LogLevels.Error, entry.Level );

      Assert.AreEqual(LogSource(Options.LogLevels.Error), entries[0].Source);
    }

    [Test]
    public void TestLogMultipleErrorItems()
    {
      TheEngine.Options.LogLevel = Options.LogLevels.Error;

      var messages = new List<string>();
      const int count = 10;

      for( var i = 0; i < count; ++i )
      {
        var message = RandomString(10);
        TheEngine.Logger.LogError(message);
        messages.Add(message);
      }

      // the list we are getting is from newest to oldest
      // so we have to reverse the list of messages.
      messages.Reverse();

      var get = count / 2;
      var entries = TheEngine.Logger.GetLogEntries(get);
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