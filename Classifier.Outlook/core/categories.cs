using Classifier.Interfaces;
using myoddweb.classifier.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace myoddweb.classifier.core
{
  public class Categories : ICategories
  {
    /// <summary>
    /// The classification engine.
    /// </summary>
    private readonly IClassify1 ClassifyEngine;

    /// <summary>
    /// The configuration interface.
    /// </summary>
    private readonly IConfig _config;

    /// <summary>
    /// The actual folders
    /// </summary>
    private readonly IFolders _folders = null;

    public virtual CategoriesCollection Collection{ get; protected set; }

    /// <summary>
    /// Get all the categories
    /// </summary>
    public IEnumerable<Category> List { get { return Collection.List(); } }

    /// <summary>
    /// The number of items in the collection
    /// </summary>
    public int Count { get { return Collection.Count; } }
    
    public Categories(IClassify1 classifyEngine, IFolders folders, IConfig config, ILogger logger)
    {
      ClassifyEngine = classifyEngine;

      // the folders.
      _folders = folders;

      // the config
      _config = config;

      Collection = new CategoriesCollection( logger);

      // we can now reload all the categories
      ReloadCategories();
    }

    public bool RenameCategory(string oldCategory, string newCategory)
    {
      return ClassifyEngine?.RenameCategory(oldCategory, newCategory) ?? false;
    }

    public bool DeleteCategory(string categoryName)
    {
      return ClassifyEngine.DeleteCategory(categoryName);
    }

    public Dictionary<int, string> GetCategories()
    {
      var categories = new Dictionary<int, string>();
      if (ClassifyEngine?.GetCategories(out categories) < 0)
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
      var id = ClassifyEngine?.GetCategory(categoryName) ?? -1;
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
      return ClassifyEngine?.GetCategoryFromUniqueId(uniqueIdentifier) ?? -1;
    }

    /// <summary>
    /// Find a category given a category id.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public Category FindCategoryById(int categoryId)
    {
      return Collection.FindCategoryById( categoryId );
    }

    /// <summary>
    /// Find a folder that belong to a category id.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public IFolder FindFolderByCategoryId(int categoryId)
    {
      // get the category.
      var category = FindCategoryById(categoryId);

      // find the fist item in the list that will match.
      return FindFolderById(category?.FolderId);
    }

    /// <summary>
    /// Find a folder given the app specific folder id.
    /// </summary>
    /// <param name="folderId"></param>
    /// <returns></returns>
    public IFolder FindFolderById(string folderId)
    {
      if (string.IsNullOrEmpty(folderId))
      {
        return null;
      }
      return _folders?.GetFolders().FirstOrDefault(e => e.Id() == folderId);
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
      Collection.Set( listOfCategories.OrderBy(c => c.Name).ToList() );
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
