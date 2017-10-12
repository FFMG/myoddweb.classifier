using myoddweb.classifier.core;
using System.Collections.Generic;

namespace myoddweb.classifier.interfaces
{
  public interface ICategories
  {
    /// <summary>
    /// Class to manage the categories.
    /// </summary>
    Categories Categories { get; }

    /// <summary>
    /// Rename a category
    /// </summary>
    /// <param name="oldCategory"></param>
    /// <param name="newCategory"></param>
    /// <returns></returns>
    bool RenameCategory(string oldCategory, string newCategory);

    /// <summary>
    /// Delete a category by name.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    bool DeleteCategory(string categoryName);

    /// <summary>
    /// Get a category id given a text unique id.
    /// </summary>
    /// <param name="uniqueIdentifier">The unique id is client specific</param>
    /// <returns></returns>
    int GetCategoryFromUniqueId(string uniqueIdentifier);

    /// <summary>
    /// Get the category id given a name.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    int GetCategory(string categoryName);

    /// <summary>
    /// Get all the categories.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, string> GetCategories();
  }
}
