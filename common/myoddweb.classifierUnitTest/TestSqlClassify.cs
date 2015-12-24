using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace myoddweb.classifierUnitTest
{
  /// <summary>
  /// Summary description for TestSqlClassify
  /// </summary>
  [TestClass]
  public class TestSqlClassify : TestCommon
  {

    [ClassCleanup]
    public static void ClassCleanup()
    {
      ReleaseEngine(true);
    }

    [TestCleanup]
    public void TestCleanup()
    {
      ReleaseEngine(false);
    }

    [TestMethod]
    public void TestTrainSimpleText()
    {
      var categoryName = RandomString(8);
      var categoryText = RandomStringWithSpaces(8);
      var uniqueEntryId = RandomString(100);
      Assert.IsTrue(TheEngine.Train(categoryName, categoryText, uniqueEntryId, 1));
    }

    [TestMethod]
    public void TestUnTrainSimpleText()
    {
      // this text/category does not exist
      // so we cannot un-train anyting.
      var categoryText = RandomStringWithSpaces(8);
      var uniqueEntryId = RandomString(100);
      Assert.IsFalse(TheEngine.UnTrain( uniqueEntryId, categoryText));
    }

    [TestMethod]
    public void TestTrainAndUnTrainSimpleText()
    {
      var categoryName = RandomString(8);
      var categoryText = RandomStringWithSpaces(8);
      var uniqueEntryId = RandomString(100);

      Assert.IsTrue(TheEngine.Train(categoryName, categoryText, uniqueEntryId, 1));
      Assert.IsTrue(TheEngine.UnTrain( uniqueEntryId, categoryText));
    }

    [TestMethod]
    public void TestGetCategoryName()
    {
      // the category is created on the fly... #1 is alocated to it.
      var categoryName = RandomString(8);
      Assert.AreEqual(1, TheEngine.GetCategory(categoryName));
    }

    [TestMethod]
    public void TestCategoryCannotBeCreatedWithEmptyString()
    {
      Assert.AreEqual(-1, TheEngine.GetCategory(""));
      Assert.AreEqual(-1, TheEngine.GetCategory(""));  //  we ask twice, in case it was inserted.
    }

    [TestMethod]
    public void TestCategoryCannotBeCreatedWithEmptyStringWithSpaces()
    {
      var random = new Random();
      var result = Enumerable.Repeat(" ", random.Next(1, 20)).Aggregate((sum, next) => sum + next);

      Assert.AreEqual(-1, TheEngine.GetCategory(result));
      Assert.AreEqual(-1, TheEngine.GetCategory(result));  //  we ask twice, in case it was inserted.
    }

    [TestMethod]
    public void TestCategoryIsCreatedByTrainning()
    {
      var categoryName = RandomString(8);
      var categoryText = RandomStringWithSpaces(8);
      var uniqueEntryId = RandomString(100);

      Assert.IsTrue(TheEngine.Train(categoryName, categoryText, uniqueEntryId, 1));

      // the category should/must exist.
      Assert.AreEqual(1, TheEngine.GetCategory(categoryName));
    }

    [TestMethod]
    public void TestSimpleCategorise()
    {
      var categoryName = RandomString(8);
      var categoryText = RandomStringWithSpaces(8);
      var uniqueEntryId = RandomString(100);

      Assert.IsTrue(TheEngine.Train(categoryName, categoryText, uniqueEntryId, 1));

      // get the category
      var categoryId = TheEngine.GetCategory(categoryName);

      // the category should/must exist.
      Assert.AreEqual(categoryId, TheEngine.Categorize(categoryText, 0 ));
    }

    [TestMethod]
    public void TestGetCategories()
    {
      // we should have no categories to start with.
      var categories = TheEngine.GetCategories();
      Assert.AreEqual( 0, categories.Count);

      // çreate 2 categories by getting them.
      var categorySpam = TheEngine.GetCategory( "Spam");
      var categoryHam = TheEngine.GetCategory( "Ham");

      var expectedCategories = new Dictionary<int, string>()
      {
        {categorySpam, "Spam" },
        {categoryHam, "Ham" }
      };

      // now get it again
      categories = TheEngine.GetCategories();

      // make sure that it is valid.
      CompareCategories(expectedCategories, categories);

      // add one more category.
      var categoryJam = TheEngine.GetCategory("Jam");
      expectedCategories.Add(categoryJam, "Jam");

      // now get it again
      categories = TheEngine.GetCategories();

      // make sure that it is valid.
      CompareCategories(expectedCategories, categories);
    }

    private void CompareCategories(Dictionary<int, string> expectedCategories, Dictionary<int, string> categories)
    {
      // we should have more than one.
      Assert.AreEqual(expectedCategories.Count, categories.Count);
      if (expectedCategories.Count == categories.Count)
      {
        foreach (var expectedCategory in expectedCategories)
        {
          Assert.IsTrue(categories.ContainsKey(expectedCategory.Key));
          Assert.AreEqual(expectedCategory.Value, categories[expectedCategory.Key]);
        }
      }
    }

    [TestMethod]
    public void TestMultipleTrainSimple()
    {
      // this text/category does not exist
      // so we cannot un-train anyting.
      var uniqueEntryId1 = RandomString(100 );
      var uniqueEntryId2 = RandomString(100);
      var uniqueEntryId3 = RandomString(100);
      var uniqueEntryId4 = RandomString(100);
      var uniqueEntryId5 = RandomString(100);

      var categoryText1 = "hello world";
      var categoryText2 = "goodbye world";
      var categoryText3 = "byebye world";
      var categoryText4 = "au revoire world";
      var categoryText5 = "is that it world?";

      Assert.IsTrue( TheEngine.Train("spam", categoryText1, uniqueEntryId1, 1)); // world appears once 1/5  = ~20%
      Assert.IsTrue( TheEngine.Train("ham" , categoryText2, uniqueEntryId2, 1)); // world appears twice 4/5 = ~80%
      Assert.IsTrue( TheEngine.Train("ham" , categoryText3, uniqueEntryId3, 1));
      Assert.IsTrue( TheEngine.Train("ham" , categoryText4, uniqueEntryId4, 1));
      Assert.IsTrue( TheEngine.Train("ham" , categoryText5, uniqueEntryId5, 1));

      // the word 'world' appears 4x in 'ham'
      // so the category must be 'ham'
      var thisCategory = TheEngine.Categorize("world is good", 0 );
      Assert.AreEqual(TheEngine.GetCategory("ham"), thisCategory);

      // try and classify #1 again into 'spam'
      Assert.IsTrue(TheEngine.Train("spam", categoryText1, uniqueEntryId1, 1));

      // but that should not change anything in the classification
      thisCategory = TheEngine.Categorize("world is good", 0);
      Assert.AreEqual(TheEngine.GetCategory("ham"), thisCategory);
    }

    [TestMethod]
    public void TestThatTheWeightIsUpdated()
    {
      var categoryName1 = RandomString(8);
      var categoryName2 = RandomString(8);

      var uniqueEntryId11 = RandomString(100);
      var uniqueEntryId12 = RandomString(100);
      var uniqueEntryId21 = RandomString(100);

      // we add the same word twice to one category
      Assert.IsTrue(TheEngine.Train(categoryName1, "World", uniqueEntryId11, 1));
      Assert.IsTrue(TheEngine.Train(categoryName1, "World", uniqueEntryId12, 1));

      // then we add the same one once, to the other category
      Assert.IsTrue(TheEngine.Train(categoryName2, "World", uniqueEntryId21, 1));

      // get the category
      var categoryId1 = TheEngine.GetCategory(categoryName1);
      var categoryId2 = TheEngine.GetCategory(categoryName2);

      // our category text
      const string categoryText = "Hello World";

      // so because there are 2 words on one, the current category should be category one.
      Assert.AreEqual(categoryId1, TheEngine.Categorize(categoryText, 0));

      // we now want to change the second category entry
      // with a bigger weight.
      Assert.IsTrue(TheEngine.Train(categoryName2, "World", uniqueEntryId21, 3));

      // so we now have 2xWordx(1xWeight) in category 1
      //            and 1xWordx(3xWeight) in category 2
      // so category 2 should be the one.
      Assert.AreEqual(categoryId2, TheEngine.Categorize(categoryText, 0));

      // and if we revet back to 1xWeight then the category should go back to category 1
      Assert.IsTrue(TheEngine.Train(categoryName2, "World", uniqueEntryId21, 1));
      Assert.IsTrue(TheEngine.Train(categoryName1, "World", uniqueEntryId21, 3));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException)) ]
    public void TryAndCategoriseWithMoreThan100Percent()
    {
      // Cannot be more than 100%
      var random = new Random(Guid.NewGuid().GetHashCode());
      TheEngine.Categorize("Some text", (uint)random.Next(101, 200));
    }

  }
}
