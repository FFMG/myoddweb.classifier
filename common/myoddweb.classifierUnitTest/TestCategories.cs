using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace myoddweb.classifierUnitTest
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestFixture]
  public class TestCategories : TestCommon
  {
    [TearDown]
    public void TestCleanup()
    {
      ReleaseEngine(false);
    }

    [OneTimeTearDown]
    public void ClassCleanup()
    {
      ReleaseEngine(true);
    }

    [Test]
    public void TestEmptyCategoriesIsNotNull()
    {
      // we should have no categories but the IEnumrable should not be null.
      var theCategories = TheEngine.Categories.List;
      Assert.IsNotNull(theCategories);
      Assert.AreEqual(0, theCategories.Count());
    }

    [Test]
    public void FindCategoryById_ReturnsNullWhenMinusOne()
    {
      Assert.IsNull(TheEngine.Categories.FindCategoryById(-1));
    }
    
    [Test]
    public void FindCategoryById_ReturnsNullWhenEmpty()
    {
      Assert.IsNull(TheEngine.Categories.FindCategoryById(123) );
    }

    [Test]
    public void FindCategoryById_ReturnsNullWhenNotFound()
    {
      // create 3 categories by getting them.
      var categorySpam = TheEngine.Categories.GetCategory("Spam");
      var categoryHam = TheEngine.Categories.GetCategory("Ham");
      var categoryJam = TheEngine.Categories.GetCategory("Jam");

      var x = categorySpam;
      while( x == categorySpam || x == categoryHam || x == categoryJam )
      {
        x = (int)RandomId();
      }
      Assert.IsNull(TheEngine.Categories.FindCategoryById(x));
    }

    [Test]
    public void FindCategoryById_ReturnsCorrectValue()
    {
      // create 3 categories by getting them.
      var categorySpam = TheEngine.Categories.GetCategory("Spam");
      var categoryHam = TheEngine.Categories.GetCategory("Ham");

      var cat1 = TheEngine.Categories.FindCategoryById(categorySpam);
      Assert.IsNotNull(cat1);
      Assert.That(cat1.Id == categorySpam);
      Assert.That(cat1.Name == "Spam");

      var cat2 = TheEngine.Categories.FindCategoryById(categoryHam);
      Assert.IsNotNull(cat2);
      Assert.That(cat2.Id == categoryHam);
      Assert.That(cat2.Name == "Ham");
    }

    [Test]
    public void GetCategory_WhenTheValueExistsAlreadyWeDoNotAddMore()
    {
      // create 3 categories by getting them.
      var categorySpam = TheEngine.Categories.GetCategory("Spam");
      // we don't need the id of those 2
      TheEngine.Categories.GetCategory("Ham");
      TheEngine.Categories.GetCategory("Jam");

      var cat1 = TheEngine.Categories.FindCategoryById(categorySpam);
      Assert.That(cat1.Id == categorySpam);
      Assert.That(cat1.Name == "Spam");

      // add spam again
      var categorySpam2 = TheEngine.Categories.GetCategory("Spam");
      
      var cat2 = TheEngine.Categories.FindCategoryById(categorySpam);

      // the two values must be the same.
      Assert.That(cat2.Id == categorySpam);
      Assert.That(cat2.Id == categorySpam2);
      Assert.That(cat2.Name == "Spam");

      // we should only have 3 values anyway
      Assert.That(3 == TheEngine.Categories.Count);
    }

    [Test]
    public void TestReloadCategories()
    {
      // we should have no categories.
      var theCategories = TheEngine.Categories.List;
      Assert.AreEqual(0, theCategories.Count());

      // create 3 categories by getting them.
      var categorySpam = TheEngine.Categories.GetCategory("Spam");
      var categoryHam = TheEngine.Categories.GetCategory("Ham");
      var categoryJam = TheEngine.Categories.GetCategory("Jam");

      // should still be 3
      theCategories = TheEngine.Categories.List;
      Assert.AreEqual(3, theCategories.Count());

      var expectedCategories = new Dictionary<int, string>()
      {
        {categorySpam, "Spam"},
        {categoryHam, "Ham"},
        {categoryJam, "Jam"}
      };

      foreach (var category in theCategories )
      {
        Assert.IsTrue(expectedCategories.ContainsKey((int) category.Id));
        Assert.AreEqual(expectedCategories[(int) category.Id], category.Name);
      }
    }

    [Test]
    public void TestRenameCategoryWithEmptyString()
    {
      // create a category
      var categoryName = RandomString(10);

      // rename it.
      Assert.IsFalse(TheEngine.Categories.RenameCategory(categoryName, ""));
    }

    [Test]
    public void TestRenameCategoryWithEmptyStringWithSpaces()
    {
      // create a category
      var categoryName = RandomString(10);

      // rename it.
      Assert.IsFalse(TheEngine.Categories.RenameCategory(categoryName, "       "));
    }

    [Test]
    public void TestRenameCategory()
    {
      // create a category
      var categoryName = RandomString(10);
      var categoryId = TheEngine.Categories.GetCategory(categoryName);

      // rename it.
      var categoryNewName = RandomString(10);
      Assert.IsTrue( TheEngine.Categories.RenameCategory(categoryName, categoryNewName));

      // look for it.
      var categoryNewId = TheEngine.Categories.GetCategory(categoryNewName);
      Assert.AreEqual( categoryNewId, categoryId );

      var categories = TheEngine.Categories.GetCategories();
      foreach (var category in categories)
      {
        Assert.AreNotSame(category.Value, categoryNewName );
      }
    }

    [Test]
    public void TestDeleteCategory()
    {
      // create a category
      var categoryName = RandomString(10);
      TheEngine.Categories.GetCategory(categoryName);

      var categories = TheEngine.Categories.GetCategories();
      Assert.IsTrue(categories.Count > 0 );

      // delete it.
      Assert.IsTrue(TheEngine.Categories.DeleteCategory(categoryName));

      // look for it.
      var categoriesNow = TheEngine.Categories.GetCategories();
      Assert.IsTrue(categories.Count == (categoriesNow.Count+1));
      foreach (var category in categoriesNow)
      {
        Assert.AreNotSame(category.Value, categoryName);
      }
    }

    [Test]
    public void TestDeleteCategoryWithEmptyName()
    {
      // delete it.
      Assert.IsFalse(TheEngine.Categories.DeleteCategory(""));
    }

    [Test]
    public void TestDeleteCategoryWithEmptyNameWithSpaces()
    {
      // delete it.
      Assert.IsFalse(TheEngine.Categories.DeleteCategory("    "));
    }
  }
}
