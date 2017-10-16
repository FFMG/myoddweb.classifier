using myoddweb.classifier.core;
using myoddweb.classifierUnitTest;
using NUnit.Framework;

namespace myoddweb.classifierUnitTest64
{
  [TestFixture]
  public class TestCategory : TestCommon
  {
    [Test]
    public void Ctor_ValuesAreSetProperly()
    {
      string name = RandomString(10);
      uint id = RandomId();
      string folderId = RandomString(10);
      var cat = new Category(name, id, folderId);
      Assert.That(cat.Name == name);
      Assert.That(cat.Id == id);
      Assert.That(cat.FolderId == folderId);
    }

    [Test]
    public void Compare_TwoThatAreTheSame()
    {
      string name = RandomString(10);
      uint id = RandomId();
      string folderId = RandomString(10);
      var cat1 = new Category(name, id, folderId);
      var cat2 = new Category(name, id, folderId);
      Assert.True(cat1 == cat2 );
      Assert.False(cat1 != cat2 );
    }

    [Test]
    public void Compare_TwoIdAreNotTheSame()
    {
      string name = RandomString(10);
      uint id1 = RandomId();
      uint id2 = id1;
      while( id1 == id2) {
        id1 = RandomId();
      }
      string folderId = RandomString(10);
      var cat1 = new Category(name, id1, folderId);
      var cat2 = new Category(name, id2, folderId);
      Assert.False(cat1 == cat2);
      Assert.True(cat1 != cat2);
    }

    [Test]
    public void Compare_TwoNamesAreNotTheSame()
    {
      string name1 = RandomString(10);
      string name2 = name1;
      while (name1 == name2)
      {
        name2 = RandomString(10);
      }
      uint id = RandomId();
      string folderId = RandomString(10);
      var cat1 = new Category(name1, id, folderId);
      var cat2 = new Category(name2, id, folderId);
      Assert.False(cat1 == cat2);
      Assert.True(cat1 != cat2);
    }

    [Test]
    public void Compare_TwoFolderIdsAreNotTheSame()
    {
      string name = RandomString(10);
      uint id = RandomId();
      string folderId1 = RandomString(10);
      string folderId2 = folderId1;
      while (folderId1 == folderId2)
      {
        folderId2 = RandomString(10);
      }
      var cat1 = new Category(name, id, folderId1);
      var cat2 = new Category(name, id, folderId2);
      Assert.False(cat1 == cat2);
      Assert.True(cat1 != cat2);
    }

    [Test]
    public void XmlName_TheNameDoesNotNeedToBeEscaped([Values("Hello", "World", "Sp  ace")]string name )
    {
      var cat = new Category(name, RandomId(), RandomString(10) );
      Assert.That(cat.XmlName == name);
    }

    [Test]
    [TestCase("<Hello>", "&lt;Hello&gt;")]
    [TestCase("3 && age", "3 &amp;&amp; age")]
    [TestCase("She said \"You're right\"", "She said \"You're right\"")]
    [TestCase("She said 'You're right'", "She said 'You're right'")]
    public void XmlName_TheNameNeedsToBeEscaped( string name, string xname )
    {
      var cat = new Category(name, RandomId(), RandomString(10));
      Assert.That(cat.XmlName == xname);
    }

  }
}
