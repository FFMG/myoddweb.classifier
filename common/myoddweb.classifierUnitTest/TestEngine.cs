using System;
using Classifier.Interfaces;
using myoddweb.classifier.core;
using Moq;
using NUnit.Framework;
namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  public class TestEngine
  {
    [Test]
    public void ctor_NullIsAllowed()
    {
      Assert.DoesNotThrow( () => new Engine( null ));
    }

    [Test]
    public void GetEngineVersionNumber_NullEngineReturnsZero()
    {
      var eng = new Engine(null);
      Assert.That( 0 == eng.GetEngineVersionNumber() );
    }

    [Test]
    [TestCase(-1)]
    [TestCase(1006003)]
    [TestCase(2005001)]
    public void GetEngineVersionNumber_GetProperVersionNumber( int version )
    {
      var classify = new Mock<IClassify1>();
      classify.Setup(e => e.GetEngineVersion()).Returns(version);
      var eng = new Engine(classify.Object);
      
      Assert.That(version == eng.GetEngineVersionNumber());
    }

    [Test]
    [TestCase(1, 0, 0, 1)]
    [TestCase(1006003, 1, 6, 3)]
    [TestCase(2005004, 2, 5, 4)]
    public void GetEngineVersion_GetProperVersionNumber(int versionnumber, int major, int minor, int build)
    {
      var classify = new Mock<IClassify1>();
      classify.Setup(e => e.GetEngineVersion()).Returns(versionnumber);
      var eng = new Engine(classify.Object);
      
      var version = eng.GetEngineVersion();
      var expected = new Version(major, minor, build, 0);

      Assert.That(version == expected );
    }

    [Test]
    [TestCase(-1)]
    [TestCase(-10)]
    [TestCase(-123)]
    public void GetEngineVersion_NegativeVersionNumberReturnsZero(int versionnumber)
    {
      var classify = new Mock<IClassify1>();
      classify.Setup(e => e.GetEngineVersion()).Returns(versionnumber);
      var eng = new Engine(classify.Object);

      var version = eng.GetEngineVersion();

      Assert.That(version == new Version(0, 0, 0, 0));
    }
  }
}
