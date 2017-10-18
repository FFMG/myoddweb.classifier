using Classifier.Interfaces.Helpers;
using NUnit.Framework;

namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  public class TestMagnet
  {
    [Test]
    public void ctor_default_values()
    {
      var magnet = new Magnet();
      Assert.That( magnet.Id == -1 );
      Assert.That(magnet.Name == "");
      Assert.That(magnet.Category == -1);
      Assert.That(magnet.Rule == -1);
    }

    [Test]
    public void Id_ValueSetProperly_inConstructor()
    {
      var id = TestCommon.RandomInt();
      var magnet = new Magnet
      {
        Id = id
      };
      Assert.That(magnet.Id == id );
      Assert.That(magnet.Name == "");
      Assert.That(magnet.Category == -1);
      Assert.That(magnet.Rule == -1);
    }

    [Test]
    public void Id_ValueSetProperly()
    {
      var id = TestCommon.RandomInt();
      var magnet = new Magnet();
      magnet.Id = id;
      Assert.That(magnet.Id == id);
      Assert.That(magnet.Name == "");
      Assert.That(magnet.Category == -1);
      Assert.That(magnet.Rule == -1);
    }

    [Test]
    public void Category_ValueSetProperly_inConstructor()
    {
      var category = TestCommon.RandomInt();
      var magnet = new Magnet
      {
        Category = category
      };
      Assert.That(magnet.Id == -1);
      Assert.That(magnet.Name == "");
      Assert.That(magnet.Category == category);
      Assert.That(magnet.Rule == -1);
    }

    [Test]
    public void Category_ValueSetProperly()
    {
      var category = TestCommon.RandomInt();
      var magnet = new Magnet();
      magnet.Category = category;
      Assert.That(magnet.Id == -1);
      Assert.That(magnet.Name == "");
      Assert.That(magnet.Category == category );
      Assert.That(magnet.Rule == -1);
    }

    [Test]
    public void Rule_ValueSetProperly_inConstructor()
    {
      var rule = TestCommon.RandomInt();
      var magnet = new Magnet
      {
        Rule = rule
      };
      Assert.That(magnet.Id == -1);
      Assert.That(magnet.Name == "");
      Assert.That(magnet.Category == -1);
      Assert.That(magnet.Rule == rule);
    }

    [Test]
    public void Rule_ValueSetProperly()
    {
      var rule = TestCommon.RandomInt();
      var magnet = new Magnet();
      magnet.Rule = rule;
      Assert.That(magnet.Id == -1);
      Assert.That(magnet.Name == "");
      Assert.That(magnet.Category == -1);
      Assert.That(magnet.Rule == rule);
    }

    [Test]
    public void Name_ValueSetProperly_inConstructor()
    {
      var name = TestCommon.RandomString(10);
      var magnet = new Magnet
      {
        Name = name
      };
      Assert.That(magnet.Id == -1);
      Assert.That(magnet.Name == name);
      Assert.That(magnet.Category == -1);
      Assert.That(magnet.Rule == -1);
    }

    [Test]
    public void Name_ValueSetProperly()
    {
      var name = TestCommon.RandomString(10);
      var magnet = new Magnet();
      magnet.Name = name;
      Assert.That(magnet.Id == -1);
      Assert.That(magnet.Name == name);
      Assert.That(magnet.Category == -1);
      Assert.That(magnet.Rule == -1);
    }
  }
}
