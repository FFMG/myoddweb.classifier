using NUnit.Framework;
using myoddweb.classifier.utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using Classifier.Interfaces;
using myoddweb.classifier.core;
using myoddweb.classifier.interfaces;
using Moq;

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

    private static string LogSource(LogLevels level)
    {
      return $"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}.{level}";
    }

    [Test]
    public void TestSetLogLevelException()
    {
      TheEngine.Options.LogLevel = LogLevels.Exception;
      Assert.AreEqual(LogLevels.Exception, TheEngine.Options.LogLevel);
    }

    [Test]
    public void TestSetLogLevelInfo()
    {
      TheEngine.Options.LogLevel = LogLevels.Information;
      Assert.AreEqual(LogLevels.Information, TheEngine.Options.LogLevel);
    }
    
    [Test]
    public void TestCanLogLevelInfo()
    {
      TheEngine.Options.LogLevel = LogLevels.Information;
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Exception));
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Error));
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Warning));
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Verbose));
    }

    [Test]
    public void TestSetLogLevelNone()
    {
      TheEngine.Options.LogLevel = LogLevels.None;
      Assert.AreEqual(LogLevels.None, TheEngine.Options.LogLevel);
    }

    [Test]
    public void TestCanLogLevelNone()
    {
      TheEngine.Options.LogLevel = LogLevels.None;
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Exception));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Error));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Warning));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Verbose));
    }

    [Test]
    public void TestSetLogLevelWarning()
    {
      TheEngine.Options.LogLevel = LogLevels.Warning;
      Assert.AreEqual(LogLevels.Warning, TheEngine.Options.LogLevel);
    }

    [Test]
    public void TestCanLogLevelWarning()
    {
      TheEngine.Options.LogLevel = LogLevels.Warning;
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Exception));
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Error));
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Warning));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Verbose));
    }

    [Test]
    public void TestSetLogLevelVerbose()
    {
      TheEngine.Options.LogLevel = LogLevels.Verbose;
      Assert.AreEqual(LogLevels.Verbose, TheEngine.Options.LogLevel);
    }

    [Test]
    public void TestCanLogLevelVerbose()
    {
      TheEngine.Options.LogLevel = LogLevels.Verbose;
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Exception));
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Error));
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Warning));
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Information));
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Verbose));
    }

    [Test]
    public void TestSetLogLevelError()
    {
      TheEngine.Options.LogLevel = LogLevels.Error;
      Assert.AreEqual(LogLevels.Error, TheEngine.Options.LogLevel);
    }

    [Test]
    public void TestCanLogLevelError()
    {
      TheEngine.Options.LogLevel = LogLevels.Error;
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Exception));
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Error));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Warning));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Verbose));
    }

    [Test]
    public void TestCanLogLevelException()
    {
      TheEngine.Options.LogLevel = LogLevels.Exception;
      Assert.IsTrue(TheEngine.Options.CanLog(LogLevels.Exception));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Error));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Warning));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Information));
      Assert.IsFalse(TheEngine.Options.CanLog(LogLevels.Verbose));
    }

    [Test]
    public void TestLogOneErrorItem()
    {
      TheEngine.Options.LogLevel = LogLevels.Error;

      var message = RandomString(10);
      TheEngine.Logger.LogError(message);

      var entries = TheEngine.Logger.GetLogEntries(10);
      Assert.AreEqual(1, entries.Count);

      var entry = JsonConvert.DeserializeObject<LogData>(entries[0].Entry);
      Assert.AreEqual(message, entry.Message);
      Assert.AreEqual(LogLevels.Error, entry.Level );

      Assert.AreEqual(LogSource(LogLevels.Error), entries[0].Source);
    }

    [Test]
    public void TestLogMultipleErrorItems()
    {
      TheEngine.Options.LogLevel = LogLevels.Error;

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
        Assert.AreEqual(LogLevels.Error, entry.Level);

        Assert.AreEqual(LogSource(LogLevels.Error), entries[i].Source);
      }
    }

    [Test]
    public void LoggInformation_MakeSureOptionsIsCalledOnceOnly()
    {
      var iclassify = new Mock<IClassify1>();
      var ioption = new Mock<IOptions>();
      var logger = new Logger(iclassify.Object, ioption.Object);

      // 
      ioption.Setup(e => e.CanLog(LogLevels.Information)).Returns(false);
      logger.LogInformation("Hello");

      ioption.Verify(m => m.CanLog(LogLevels.Information), Times.Once);
      iclassify.Verify(m => m.Log(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void LoggWarning_MakeSureOptionsIsCalledOnceOnly()
    {
      var iclassify = new Mock<IClassify1>();
      var ioption = new Mock<IOptions>();
      var logger = new Logger(iclassify.Object, ioption.Object);

      // 
      ioption.Setup(e => e.CanLog(LogLevels.Warning)).Returns(false);
      logger.LogWarning("Hello");

      ioption.Verify(m => m.CanLog(LogLevels.Warning), Times.Once);
      iclassify.Verify(m => m.Log(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void LoggError_MakeSureOptionsIsCalledOnceOnly()
    {
      var iclassify = new Mock<IClassify1>();
      var ioption = new Mock<IOptions>();
      var logger = new Logger( iclassify.Object, ioption.Object );

      // 
      ioption.Setup(e => e.CanLog(LogLevels.Error)).Returns(false);
      logger.LogError( "Hello" );

      ioption.Verify( m => m.CanLog(LogLevels.Error), Times.Once );
      iclassify.Verify(m => m.Log(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void LoggError_CheckThatEngineIsCalled()
    {
      var iclassify = new Mock<IClassify1>();
      var ioption = new Mock<IOptions>();
      var logger = new Logger(iclassify.Object, ioption.Object);

      // 
      ioption.Setup(e => e.CanLog(LogLevels.Error)).Returns(true);
      logger.LogError("Hello");

      ioption.Verify(m => m.CanLog(LogLevels.Error), Times.Once);
      iclassify.Verify(m => m.Log(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void ClearLogEntries_CheckThatEngineIsCalled()
    {
      var iclassify = new Mock<IClassify1>();
      var ioption = new Mock<IOptions>();
      var logger = new Logger(iclassify.Object, ioption.Object);

      // 
      logger.ClearLogEntries(10);
      iclassify.Verify(m => m.ClearLogEntries(It.IsAny<long>()), Times.Once);
    }
  }
}