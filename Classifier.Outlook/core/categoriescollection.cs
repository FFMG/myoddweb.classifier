using System.Collections.Generic;
using System.Linq;

namespace myoddweb.classifier.core
{
  public class CategoriesCollection
  {
    /// <summary>
    /// Sorted list of categories.
    /// </summary>
    private IEnumerable<Category> Categories { get; set; }

    /// <summary>
    /// Get the number of items in the category
    /// </summary>
    public int Count => Categories?.Count() ?? 0;

    public Category FindCategoryById( int categoryId )
    {
      // find the fist item in the list that will match.
      return -1 == categoryId ? null : Categories?.FirstOrDefault(e => e.Id == categoryId);
    }

    /// <summary>
    /// Get the sorted list of categories.
    /// </summary>
    /// <returns>IEnumerable the sorted list.</returns>
    public IEnumerable<Category> List()
    {
      return Categories ?? new List<Category>();
    }

    /// <summary>
    /// Set the categories values.
    /// </summary>
    /// <param name="values"></param>
    public void Set(IEnumerable<Category> values )
    {
      Categories = values;
    }


  }
}
