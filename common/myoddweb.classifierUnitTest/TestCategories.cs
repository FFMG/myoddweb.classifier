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
      // we should have no categories.
      var theCategories = TheEngine.Categories.List;
      Assert.IsNotNull(theCategories);
      Assert.AreEqual(0, theCategories.Count());
    }

    [Test]
    public void TestReloadCategories()
    {
      // we should have no categories.
      var theCategories = TheEngine.Categories.List;
      Assert.AreEqual(0, theCategories.Count());

      // create 2 categories by getting them.
      var categorySpam = TheEngine.Categories.GetCategory("Spam");
      var categoryHam = TheEngine.Categories.GetCategory("Ham");
      var categoryJam = TheEngine.Categories.GetCategory("Jam");

      // should still be 0
      theCategories = TheEngine.Categories.List;
      Assert.AreEqual(0, theCategories.Count());

      TheEngine.Categories.ReloadCategories();

      // we should now have 3
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
      var categoryId = TheEngine.Categories.GetCategory(categoryName);

      // rename it.
      Assert.IsFalse(TheEngine.Categories.RenameCategory(categoryName, ""));
    }

    [Test]
    public void TestRenameCategoryWithEmptyStringWithSpaces()
    {
      // create a category
      var categoryName = RandomString(10);
      var categoryId = TheEngine.Categories.GetCategory(categoryName);

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
      var categoryId = TheEngine.Categories.GetCategory(categoryName);

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
