using myoddweb.classifier.core;
using System.Collections.Generic;

namespace myoddweb.classifier.interfaces
{
  public enum MailStringCategories
  {
    Bcc,
    To,
    Address,
    SenderName,
    Cc,
    Subject,
    Body,
    HtmlBody,
    RtfBody,
    Smtp
  }

  public interface ICategories
  {
    /// <summary>
    /// Get all the categories
    /// </summary>
    IEnumerable<Category> List { get; }

    /// <summary>
    /// The number of items in the collection
    /// </summary>
    int Count { get; }

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

    /// <summary>
    /// Find a category given a category id.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    Category FindCategoryById( int categoryId);

    /// <summary>
    /// Find a folder that belong to a category id.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    string FindFolderIdByCategoryId(int categoryId);

    /// <summary>
    /// Find all the posible categories, given a folder id.
    /// </summary>
    /// <param name="folderId"></param>
    /// <returns></returns>
    IEnumerable<Category> FindCategoriesByFolderId(string folderId);

    /// <summary>
    /// Reload all the categories from the list of categories.
    /// </summary>
    void ReloadCategories();
  }
}
