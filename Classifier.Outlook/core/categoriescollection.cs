using myoddweb.classifier.interfaces;
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
    
    /// <summary>
    /// What we will be using to log information
    /// </summary>
    private readonly ILogger _logger;

    public CategoriesCollection( ILogger logger )
    {
      // the logger
      _logger = logger;
    }

    public Category FindCategoryById( int categoryId )
    {
      if( -1 == categoryId )
      {
        return null;
      }

      // find the fist item in the list that will match.
      return Categories.FirstOrDefault(e => e.Id == categoryId);
    }

    /// <summary>
    /// Get the sorted list of categories.
    /// </summary>
    /// <returns>IEnumerable the sorted list.</returns>
    public IEnumerable<Category> List()
    {
      return Categories ?? new List<Category>();
    }
    public void Set(IEnumerable<Category> values )
    {
      Categories = values;
    }


  }
}
