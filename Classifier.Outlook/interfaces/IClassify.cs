using Classifier.Interfaces.Helpers;
using myoddweb.classifier.core;
using System.Collections.Generic;

namespace myoddweb.classifier.interfaces
{
  public interface IClassify
  {
    /// <summary>
    /// Categorise a text
    /// </summary>
    /// <param name="categoryText"></param>
    /// <param name="minPercentage"></param>
    /// <returns></returns>
    int Categorize(string categoryText, uint minPercentage);

    /// <summary>
    /// Categorize a given text 
    /// </summary>
    /// <param name="categoryText"></param>
    /// <param name="minPercentage"></param>
    /// <param name="wordsCategory"></param>
    /// <param name="categoryProbabilities"></param>
    /// <returns></returns>
    int Categorize(string categoryText, uint minPercentage, out List<WordCategory> wordsCategory, out Dictionary<int, double> categoryProbabilities);

    /// <summary>
    /// Categorize a list of words sorted by string types.
    /// </summary>
    /// <param name="categoryList"></param>
    /// <returns></returns>
    int Categorize(Dictionary<CategoriesCollection.MailStringCategories, string> categoryList);

    /// <summary>
    /// Train a given text to a category
    /// </summary>
    /// <param name="categoryName"></param>
    /// <param name="textToCategorise"></param>
    /// <param name="uniqueIdentifier"></param>
    /// <param name="weight"></param>
    /// <returns></returns>
    bool Train(string categoryName, string textToCategorise, string uniqueIdentifier, int weight);

    /// <summary>
    /// Un train a previously trainned text, (using the unique id).
    /// </summary>
    /// <param name="uniqueIdentifier"></param>
    /// <param name="textToCategorise"></param>
    /// <returns></returns>
    bool UnTrain(string uniqueIdentifier, string textToCategorise);
  }
}
