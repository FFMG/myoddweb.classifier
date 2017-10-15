using System;
using System.Collections.Generic;
using Classifier.Interfaces.Helpers;
using myoddweb.classifier.interfaces;
using Classifier.Interfaces;
using System.Linq;

namespace myoddweb.classifier.core
{
  public class Classify : IClassify
  {
    /// <summary>
    /// The classification engine.
    /// </summary>
    private readonly IClassify1 ClassifyEngine;

    /// <summary>
    /// The options
    /// </summary>
    private readonly IOptions Options;

    public Classify(IClassify1 classifyEngine, IOptions options)
    {
      ClassifyEngine = classifyEngine;
      Options = options;
    }

    public int Categorize(string categoryText, uint minPercentage, out List<WordCategory> wordsCategory, out Dictionary<int, double> categoryProbabilities)
    {
      wordsCategory = new List<WordCategory>();
      categoryProbabilities = new Dictionary<int, double>();

      // the category min percentage cannot be more than 100%.
      // it also cannot be less than 0, but we use a uint.
      if (minPercentage > 100)
      {
        throw new ArgumentException("The categotry minimum range cannot be more than 100%.");
      }
      return ClassifyEngine?.Categorize(categoryText, minPercentage, out wordsCategory, out categoryProbabilities) ?? -1;
    }

    public int Categorize(string categoryText, uint minPercentage)
    {
      // the category min percentage cannot be more than 100%.
      // it also cannot be less than 0, but we use a uint.
      if (minPercentage > 100)
      {
        throw new ArgumentException("The categotry minimum range cannot be more than 100%.");
      }

      return ClassifyEngine?.Categorize(categoryText, minPercentage) ?? -1;
    }

    public int Categorize(Dictionary<CategoriesCollection.MailStringCategories, string> categoryList)
    {
      return Categorize(string.Join(";", categoryList.Select(x => x.Value)), Options.MinPercentage);
    }

    public bool Train(string categoryName, string textToCategorise, string uniqueIdentifier, int weight)
    {
      if (weight <= 0)
      {
        throw new ArgumentException("The weight cannot be 0 or less!");
      }
      return ClassifyEngine?.Train(categoryName, textToCategorise, uniqueIdentifier, weight) ?? false;
    }

    public bool UnTrain(string uniqueIdentifier, string textToCategorise)
    {
      return ClassifyEngine?.UnTrain(uniqueIdentifier, textToCategorise) ?? false;
    }
  }
}
