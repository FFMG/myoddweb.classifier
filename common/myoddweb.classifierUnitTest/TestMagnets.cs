using Classifier.Interfaces;
using Classifier.Interfaces.Helpers;
using Moq;
using myoddweb.classifier.core;
using NUnit.Framework;
using System;

namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  public class TestMagnets
  {
    [Test]
    public void Ctor_engineCannotBeNull()
    {
      Assert.Throws<ArgumentNullException>(() => new Magnets(null));
    }

    [Test]
    public void Ctor_engineNotNullDoesNotThrow()
    {
      var classify = new Mock<IClassify1>();
      Assert.DoesNotThrow(() => new Magnets(classify.Object));
    }

    [Test]
    [TestCase(10)]
    [TestCase(0)]
    [TestCase(-1)]
    public void CreateMagnet_ReturnTheSameValueFomrTheEngine( int value )
    {
      var classify = new Mock<IClassify1>();
      classify.Setup(c => c.CreateMagnet(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()) ).Returns( value );

      var magnets = new Magnets(classify.Object);
      Assert.That( value == magnets.CreateMagnet(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void DeleteMagnet_ReturnTheSameValueFomrTheEngine(bool value)
    {
      var classify = new Mock<IClassify1>();
      classify.Setup(c => c.DeleteMagnet(It.IsAny<int>())).Returns(value);

      var magnets = new Magnets(classify.Object);
      Assert.That(value == magnets.DeleteMagnet(It.IsAny<int>()));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void UpdateMagnet_ReturnTheSameValueFomrTheEngine(bool value)
    {
      var classify = new Mock<IClassify1>();
      classify.Setup(c => c.UpdateMagnet(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(value);

      var magnets = new Magnets(classify.Object);
      Assert.That(value == magnets.UpdateMagnet(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));
    }

    [Test]
    public void UpdateMagnet_IfNothingChangeThenItShouldNotBeCalled()
    {
      var classify = new Mock<IClassify1>();

      var cat = TestCommon.RandomInt();
      var rule = TestCommon.RandomInt();
      var name = TestCommon.RandomString(10);
      var magnet = new Magnet
      {
        Id = 0,
        Name = name,
        Rule = rule,
        Category = cat
      };

      var magnets = new Magnets(classify.Object);

      // nothing changed, but still.
      Assert.True( magnets.UpdateMagnet( magnet, name, rule, cat ));

      // it was never updated...
      classify.Verify( m => m.UpdateMagnet(magnet.Id, name, rule, cat), Times.Never );
    }

    [Test]
    public void UpdateMagnet_OnlyCategoryChanges()
    {
      var classify = new Mock<IClassify1>();

      var cat = TestCommon.RandomInt();
      var rule = TestCommon.RandomInt();
      var name = TestCommon.RandomString(10);
      var magnet = new Magnet
      {
        Id = 0,
        Name = name,
        Rule = rule,
        Category = cat
      };

      var magnets = new Magnets(classify.Object);

      // new category
      var cat1 = TestCommon.RandomInt();
      while( cat == cat1 )
      {
        cat1 = TestCommon.RandomInt();
      }

      classify.Setup(c => c.UpdateMagnet(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(true);
      Assert.True(magnets.UpdateMagnet(magnet, name, rule, cat1));

      // it was never updated...
      classify.Verify(m => m.UpdateMagnet(magnet.Id, name, rule, cat1), Times.Once);
    }

    [Test]
    public void UpdateMagnet_OnlyRuleChanges()
    {
      var classify = new Mock<IClassify1>();

      var cat = TestCommon.RandomInt();
      var rule = TestCommon.RandomInt();
      var name = TestCommon.RandomString(10);
      var magnet = new Magnet
      {
        Id = 0,
        Name = name,
        Rule = rule,
        Category = cat
      };

      var magnets = new Magnets(classify.Object);

      // new name
      var rule1 = TestCommon.RandomInt();
      while (rule == rule1)
      {
        rule1 = TestCommon.RandomInt();
      }

      classify.Setup(c => c.UpdateMagnet(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(true);
      Assert.True(magnets.UpdateMagnet(magnet, name, rule1, cat));

      // it was never updated...
      classify.Verify(m => m.UpdateMagnet(magnet.Id, name, rule1, cat), Times.Once);
    }

    [Test]
    public void UpdateMagnet_OnlyNameChanges()
    {
      var classify = new Mock<IClassify1>();

      var cat = TestCommon.RandomInt();
      var rule = TestCommon.RandomInt();
      var name = TestCommon.RandomString(10);
      var magnet = new Magnet
      {
        Id = 0,
        Name = name,
        Rule = rule,
        Category = cat
      };

      var magnets = new Magnets(classify.Object);

      // new name
      var name1 = TestCommon.RandomString(10);
      while( name == name1 )
      {
        name1 = TestCommon.RandomString(10);
      }

      classify.Setup(c => c.UpdateMagnet(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(true);
      Assert.True(magnets.UpdateMagnet(magnet, name1, rule, cat));

      // it was never updated...
      classify.Verify(m => m.UpdateMagnet(magnet.Id, name1, rule, cat), Times.Once);
    }

    [Test]
    public void UpdateMagnet_NullMagnetReturnsFalse()
    {
      var classify = new Mock<IClassify1>();

      var magnets = new Magnets(classify.Object);

      var cat = TestCommon.RandomInt();
      var rule = TestCommon.RandomInt();
      var name = TestCommon.RandomString(10);
      Assert.False(magnets.UpdateMagnet(null, name, rule, cat));

      // it was never updated...
      classify.Verify(m => m.UpdateMagnet(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }
  }
}