using myoddweb.classifier.core;
using myoddweb.classifier.interfaces;
using Moq;
using NUnit.Framework;

namespace myoddweb.classifierUnitTest64
{
  [TestFixture]
  public class TestOutlookLogger
  {
    /// <summary>
    /// Name for logging in the event viewer,
    /// </summary>
    private const string EventViewSource = "myoddweb.classifier";


    [Test]
    public void ctor_parent_can_be_null()
    {
      Assert.DoesNotThrow( () => new OutlookLogger(EventViewSource, null ));
    }

    [Test]
    public void loginformation_makesure_that_parent_is_called()
    {
      var ilogger = new Mock<ILogger>();
      var logged = new OutlookLogger(EventViewSource, ilogger.Object);
      logged.LogInformation( "Hello" );

      // but was it called?
      ilogger.Verify(m => m.LogInformation( "Hello" ), Times.Once);
      ilogger.Verify(m => m.LogWarning("Hello"), Times.Never);
      ilogger.Verify(m => m.LogError("Hello"), Times.Never);
      ilogger.Verify(m => m.LogVerbose("Hello"), Times.Never);
    }

    [Test]
    public void logerror_makesure_that_parent_is_called()
    {
      var ilogger = new Mock<ILogger>();
      var logged = new OutlookLogger(EventViewSource, ilogger.Object);
      logged.LogError("Hello");

      // but was it called?
      ilogger.Verify(m => m.LogInformation("Hello"), Times.Never);
      ilogger.Verify(m => m.LogWarning("Hello"), Times.Never);
      ilogger.Verify(m => m.LogError("Hello"), Times.Once);
      ilogger.Verify(m => m.LogVerbose("Hello"), Times.Never);
    }

    [Test]
    public void logwarning_makesure_that_parent_is_called()
    {
      var ilogger = new Mock<ILogger>();
      var logged = new OutlookLogger(EventViewSource, ilogger.Object);
      logged.LogWarning("Hello");

      // but was it called?
      ilogger.Verify(m => m.LogInformation("Hello"), Times.Never);
      ilogger.Verify(m => m.LogWarning("Hello"), Times.Once);
      ilogger.Verify(m => m.LogError("Hello"), Times.Never);
      ilogger.Verify(m => m.LogVerbose("Hello"), Times.Never);
    }

    [Test]
    public void logverbose_makesure_that_parent_is_called()
    {
      var ilogger = new Mock<ILogger>();
      var logged = new OutlookLogger(EventViewSource, ilogger.Object);
      logged.LogVerbose("Hello");

      // but was it called?
      ilogger.Verify(m => m.LogInformation("Hello"), Times.Never);
      ilogger.Verify(m => m.LogWarning("Hello"), Times.Never);
      ilogger.Verify(m => m.LogError("Hello"), Times.Never);
      ilogger.Verify(m => m.LogVerbose("Hello"), Times.Once);
    }

    [Test]
    [TestCase(10)]
    [TestCase(0)]
    [TestCase(-10)]
    public void GetLogEntries_callsParent( int max )
    {
      var ilogger = new Mock<ILogger>();
      var logged = new OutlookLogger(EventViewSource, ilogger.Object);
      logged.GetLogEntries(max);
      ilogger.Verify(m => m.GetLogEntries(max), Times.Once);
    }

    [Test]
    [TestCase(10)]
    [TestCase(0)]
    [TestCase(-10)]
    public void ClearLogEntries_callsParent(long olderThan)
    {
      var ilogger = new Mock<ILogger>();
      var logged = new OutlookLogger(EventViewSource, ilogger.Object);
      logged.ClearLogEntries(olderThan);
      ilogger.Verify(m => m.ClearLogEntries(olderThan), Times.Once);
    }

    [Test]
    public void ClearLogEntries_withNoParentReturnsFalse()
    {
      var logged = new OutlookLogger(EventViewSource, null);
      Assert.IsFalse( logged.ClearLogEntries(10));
    }

    [Test]
    public void GetLogEntries_withNoParentReturnsNull()
    {
      var logged = new OutlookLogger(EventViewSource, null);
      Assert.IsNull(logged.GetLogEntries(10));
    }
  }
}
