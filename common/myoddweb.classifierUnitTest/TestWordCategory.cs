using Classifier.Interfaces.Helpers;
using NUnit.Framework;

namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  public class TestWordCategory
  {
    [Test]
    public void ctor_default_values()
    {
      var wordCategory = new WordCategory();
      Assert.That(wordCategory.Word == "" );
      Assert.That(wordCategory.Category == -1);
      Assert.That(wordCategory.Probability == 0);
    }

    [Test]
    public void Word_ValueSetProperly_inConstructor()
    {
      var word = TestCommon.RandomString(10);
      var wordCategory = new WordCategory
      {
        Word = word
      };
      Assert.That(wordCategory.Word == word);
      Assert.That(wordCategory.Category == -1);
      Assert.That(wordCategory.Probability == 0);
    }

    [Test]
    public void Word_ValueSetProperly()
    {
      var word = TestCommon.RandomString(10);
      var wordCategory = new WordCategory();
      wordCategory.Word = word;
      Assert.That(wordCategory.Word == word);
      Assert.That(wordCategory.Category == -1);
      Assert.That(wordCategory.Probability == 0);
    }

    [Test]
    public void Category_ValueSetProperly_inConstructor()
    {
      var category = TestCommon.RandomInt();
      var wordCategory = new WordCategory
      {
        Category = category
      };
      Assert.That(wordCategory.Word == "");
      Assert.That(wordCategory.Category == category);
      Assert.That(wordCategory.Probability == 0);
    }

    [Test]
    public void Category_ValueSetProperly()
    {
      var category = TestCommon.RandomInt();
      var wordCategory = new WordCategory();
      wordCategory.Category = category;
      Assert.That(wordCategory.Word == "");
      Assert.That(wordCategory.Category == category);
      Assert.That(wordCategory.Probability == 0);
    }

    [Test]
    public void Probability_ValueSetProperly_inConstructor()
    {
      var probability = TestCommon.RandomDouble();
      var wordCategory = new WordCategory
      {
        Probability = probability
      };
      Assert.That(wordCategory.Word == "");
      Assert.That(wordCategory.Category == -1);
      Assert.That(wordCategory.Probability == probability);
    }

    [Test]
    public void Rule_ValueSetProperly()
    {
      var probability = TestCommon.RandomDouble();
      var wordCategory = new WordCategory();
      wordCategory.Probability = probability;
      Assert.That(wordCategory.Word == "");
      Assert.That(wordCategory.Category == -1);
      Assert.That(wordCategory.Probability == probability);
    }
  }
}
