using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace myoddweb.classifierUnitTest
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class TestCategories : TestCommon
  {
    [TestCleanup]
    public void TestCleanup()
    {
      ReleaseEngine(false);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      ReleaseEngine(true);
    }

    [TestMethod]
    public void TestReloadCategories()
    {
      // we should have no categories.
      var theCategories = new myoddweb.classifier.core.Categories(TheEngine );
      Assert.AreEqual(0, theCategories.Count);

      // create 2 categories by getting them.
      var categorySpam = TheEngine.GetCategory("Spam");
      var categoryHam = TheEngine.GetCategory("Ham");
      var categoryJam = TheEngine.GetCategory("Jam");

      // should still be 0
      Assert.AreEqual(0, theCategories.Count);

      theCategories.ReloadCategories();

      // we should now have 3
      Assert.AreEqual(3, theCategories.Count);

      var expectedCategories = new Dictionary<int, string>()
      {
        {categorySpam, "Spam"},
        {categoryHam, "Ham"},
        {categoryJam, "Jam"}
      };

      if (expectedCategories.Count != theCategories.Count)
      {
        return;
      }

      foreach (var category in theCategories.List() )
      {
        Assert.IsTrue(expectedCategories.ContainsKey((int) category.Id));
        Assert.AreEqual(expectedCategories[(int) category.Id], category.Name);
      }
    }

    [TestMethod]
    public void TestRenameCategoryWithEmptyString()
    {
      // create a category
      var categoryName = RandomString(10);
      var categoryId = TheEngine.GetCategory(categoryName);

      // rename it.
      Assert.IsFalse(TheEngine.RenameCategory(categoryName, ""));
    }

    [TestMethod]
    public void TestRenameCategoryWithEmptyStringWithSpaces()
    {
      // create a category
      var categoryName = RandomString(10);
      var categoryId = TheEngine.GetCategory(categoryName);

      // rename it.
      Assert.IsFalse(TheEngine.RenameCategory(categoryName, "       "));
    }

    [TestMethod]
    public void TestRenameCategory()
    {
      // create a category
      var categoryName = RandomString(10);
      var categoryId = TheEngine.GetCategory(categoryName);

      // rename it.
      var categoryNewName = RandomString(10);
      Assert.IsTrue( TheEngine.RenameCategory(categoryName, categoryNewName));

      // look for it.
      var categoryNewId = TheEngine.GetCategory(categoryNewName);
      Assert.AreEqual( categoryNewId, categoryId );

      var categories = TheEngine.GetCategories();
      foreach (var category in categories)
      {
        Assert.AreNotSame(category.Value, categoryNewName );
      }
    }

    [TestMethod]
    public void TestDeleteCategory()
    {
      // create a category
      var categoryName = RandomString(10);
      var categoryId = TheEngine.GetCategory(categoryName);

      var categories = TheEngine.GetCategories();
      Assert.IsTrue(categories.Count > 0 );

      // delete it.
      Assert.IsTrue(TheEngine.DeleteCategory(categoryName));

      // look for it.
      var categoriesNow = TheEngine.GetCategories();
      Assert.IsTrue(categories.Count == (categoriesNow.Count+1));
      foreach (var category in categoriesNow)
      {
        Assert.AreNotSame(category.Value, categoryName);
      }
    }

    [TestMethod]
    public void TestDeleteCategoryWithEmptyName()
    {
      // delete it.
      Assert.IsFalse(TheEngine.DeleteCategory(""));
    }

    [TestMethod]
    public void TestDeleteCategoryWithEmptyNameWithSpaces()
    {
      // delete it.
      Assert.IsFalse(TheEngine.DeleteCategory("    "));
    }
  }
}
