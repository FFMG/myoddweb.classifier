using Classifier.Interfaces;
using myoddweb.classifier.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace myoddweb.classifier.core
{
  public sealed class Categories : ICategories
  {
    /// <summary>
    /// The classification engine.
    /// </summary>
    private readonly IClassify1 _classifyEngine;

    /// <summary>
    /// The configuration interface.
    /// </summary>
    private readonly IConfig _config;

    /// <summary>
    //  All the categories.
    /// </summary>
    private readonly CategoriesCollection _collection;

    /// <summary>
    /// Get all the categories
    /// </summary>
    public IEnumerable<Category> List => _collection.List();

    /// <summary>
    /// The number of items in the collection
    /// </summary>
    public int Count => _collection.Count;

    public Categories(IClassify1 classifyEngine, IConfig config)
    {
      _classifyEngine = classifyEngine;
      
      // the config
      _config = config;

      _collection = new CategoriesCollection();

      // we can now reload all the categories
      ReloadCategories();
    }

    public bool RenameCategory(string oldCategory, string newCategory)
    {
      return _classifyEngine?.RenameCategory(oldCategory, newCategory) ?? false;
    }

    public bool DeleteCategory(string categoryName)
    {
      return _classifyEngine.DeleteCategory(categoryName);
    }

    public Dictionary<int, string> GetCategories()
    {
      var categories = new Dictionary<int, string>();
      if (_classifyEngine?.GetCategories(out categories) < 0)
      {
        return new Dictionary<int, string>();
      }
      return categories;
    }

    /// <summary>
    /// Get a category by category name.
    /// NB: The category is created if needed.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public int GetCategory(string categoryName)
    {
      var id = _classifyEngine?.GetCategory(categoryName) ?? -1;
      if( id == -1 )
      {
        return -1;
      }

      // we must now reload everything in case we 
      // created a brand new category.
      ReloadCategories();

      // and return the id value
      return id;
    }

    public int GetCategoryFromUniqueId(string uniqueIdentifier)
    {
      return _classifyEngine?.GetCategoryFromUniqueId(uniqueIdentifier) ?? -1;
    }

    /// <inheritdoc />
    /// <summary>
    /// Find a category given a category id.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public Category FindCategoryById(int categoryId)
    {
      return _collection.FindCategoryById( categoryId );
    }

    /// <summary>
    /// Find a folder that belong to a category id.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public string FindFolderIdByCategoryId(int categoryId)
    {
      // get the category.
      var category = FindCategoryById(categoryId);

      // if we found a folder, then return it
      // otherwise return nothing at all.
      return category?.FolderId ?? "";
    }

    /// <inheritdoc />
    /// <summary>
    /// Find all the posible categories, given a folder id.
    /// </summary>
    /// <param name="folderId"></param>
    /// <returns></returns>
    public IEnumerable<Category> FindCategoriesByFolderId(string folderId)
    {
      return _collection.FindCategoriesByFolderId(folderId);
    }

    /// <summary>
    /// Reload all the categories from the list of categories.
    /// </summary>
    public void ReloadCategories()
    {
      // reload the categories.
      var categories = GetCategories();

      // use a temp list to update what we have.
      var listOfCategories = new List<Category>();

      // add all the items.
      foreach (var category in categories)
      {
        var folderId = GetFolderId(category.Value);

        //  we cast to uint as we know the value is not -1
        listOfCategories.Add(new Category(category.Value, (uint)category.Key, folderId));
      }

      // we now need to sort it and save it here...
      _collection.Set( listOfCategories.OrderBy(c => c.Name).ToList() );
    }

    private string GetFolderId(string categoryName)
    {
      try
      {
        // get the config name
        var configName = GetConfigName(categoryName);

        // and now ger the value.
        return _config.GetConfig(configName);
      }
      catch (KeyNotFoundException)
      {
        return "";
      }
    }

    public static string GetConfigName(string categoryName)
    {
      return $"category.folder.{categoryName}";
    }
  }
}
