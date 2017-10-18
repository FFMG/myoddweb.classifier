using Classifier.Interfaces.Helpers;
using NUnit.Framework;

namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  public class TestLogEntry
  {
    [Test]
    public void ctor_default_values()
    {
      var logEntry = new LogEntry();
      Assert.That(logEntry.Id == -1);
      Assert.That(logEntry.Unixtime == 0);
      Assert.That(logEntry.Source == "");
      Assert.That(logEntry.Entry == "");
    }

    [Test]
    public void Id_ValueSetProperly_inConstructor()
    {
      var id = TestCommon.RandomInt();
      var logEntry = new LogEntry
      {
        Id = id
      };
      Assert.That(logEntry.Id == id);
      Assert.That(logEntry.Unixtime == 0);
      Assert.That(logEntry.Source == "");
      Assert.That(logEntry.Entry == "");
    }

    [Test]
    public void Id_ValueSetProperly()
    {
      var id = TestCommon.RandomInt();
      var logEntry = new LogEntry();
      logEntry.Id = id;
      Assert.That(logEntry.Id == id);
      Assert.That(logEntry.Unixtime == 0);
      Assert.That(logEntry.Source == "");
      Assert.That(logEntry.Entry == "");
    }

    [Test]
    public void Unixtime_ValueSetProperly_inConstructor()
    {
      var unixtime = TestCommon.RandomInt();
      var logEntry = new LogEntry
      {
        Unixtime = unixtime
      };
      Assert.That(logEntry.Id == -1);
      Assert.That(logEntry.Unixtime == unixtime);
      Assert.That(logEntry.Source == "");
      Assert.That(logEntry.Entry == "");
    }

    [Test]
    public void Unixtime_ValueSetProperly()
    {
      var unixtime = TestCommon.RandomInt();
      var logEntry = new LogEntry();
      logEntry.Unixtime = unixtime;
      Assert.That(logEntry.Id == -1);
      Assert.That(logEntry.Unixtime == unixtime);
      Assert.That(logEntry.Source == "");
      Assert.That(logEntry.Entry == "");
    }
    
    [Test]
    public void Source_ValueSetProperly_inConstructor()
    {
      var source = TestCommon.RandomString(10);
      var logEntry = new LogEntry
      {
        Source = source
      };
      Assert.That(logEntry.Id == -1);
      Assert.That(logEntry.Unixtime == 0);
      Assert.That(logEntry.Source == source);
      Assert.That(logEntry.Entry == "");
    }

    [Test]
    public void Source_ValueSetProperly()
    {
      var source = TestCommon.RandomString(10);
      var logEntry = new LogEntry();
      logEntry.Source = source;
      Assert.That(logEntry.Id == -1);
      Assert.That(logEntry.Unixtime == 0);
      Assert.That(logEntry.Source == source);
      Assert.That(logEntry.Entry == "");
    }

    [Test]
    public void Entry_ValueSetProperly_inConstructor()
    {
      var entry = TestCommon.RandomString(10);
      var logEntry = new LogEntry
      {
        Entry = entry
      };
      Assert.That(logEntry.Id == -1);
      Assert.That(logEntry.Unixtime == 0);
      Assert.That(logEntry.Source == "");
      Assert.That(logEntry.Entry == entry);
    }

    [Test]
    public void Entry_ValueSetProperly()
    {
      var entry = TestCommon.RandomString(10);
      var logEntry = new LogEntry();
      logEntry.Entry = entry;
      Assert.That(logEntry.Id == -1);
      Assert.That(logEntry.Unixtime == 0);
      Assert.That(logEntry.Source == "");
      Assert.That(logEntry.Entry == entry);
    }
  }
}
