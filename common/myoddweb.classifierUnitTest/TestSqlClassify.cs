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
      Assert.IsTrue(TheEngine.Train(categoryName, uniqueEntryId, categoryText));
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

      Assert.IsTrue(TheEngine.Train(categoryName, uniqueEntryId, categoryText));
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

      Assert.IsTrue(TheEngine.Train(categoryName, uniqueEntryId, categoryText));

      // the category should/must exist.
      Assert.AreEqual(1, TheEngine.GetCategory(categoryName));
    }

    [TestMethod]
    public void TestSimpleCategorise()
    {
      var categoryName = RandomString(8);
      var categoryText = RandomStringWithSpaces(8);
      var uniqueEntryId = RandomString(100);

      Assert.IsTrue(TheEngine.Train(categoryName, uniqueEntryId, categoryText));

      // get the category
      var categoryId = TheEngine.GetCategory(categoryName);

      // the category should/must exist.
      Assert.AreEqual(categoryId, TheEngine.Categorize(categoryText));
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

      Assert.IsTrue( TheEngine.Train("spam", uniqueEntryId1, categoryText1)); // world appears once 1/5  = ~20%
      Assert.IsTrue( TheEngine.Train("ham" , uniqueEntryId2, categoryText2)); // world appears twice 4/5 = ~80%
      Assert.IsTrue( TheEngine.Train("ham" , uniqueEntryId3, categoryText3));
      Assert.IsTrue( TheEngine.Train("ham" , uniqueEntryId4, categoryText4));
      Assert.IsTrue( TheEngine.Train("ham" , uniqueEntryId5, categoryText5));

      // the word 'world' appears 4x in 'ham'
      // so the category must be 'ham'
      var thisCategory = TheEngine.Categorize("world is good");
      Assert.AreEqual(TheEngine.GetCategory("ham"), thisCategory);

      // try and classify #1 again into 'spam'
      Assert.IsTrue(TheEngine.Train("spam", uniqueEntryId1, categoryText1));

      // but that should not change anything in the classification
      thisCategory = TheEngine.Categorize("world is good");
      Assert.AreEqual(TheEngine.GetCategory("ham"), thisCategory);
    }
  }
}
