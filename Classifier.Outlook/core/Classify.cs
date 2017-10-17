using System;
using System.Collections.Generic;
using Classifier.Interfaces.Helpers;
using myoddweb.classifier.interfaces;
using Classifier.Interfaces;
using System.Linq;

namespace myoddweb.classifier.core
{
  public sealed class Classify : IClassify
  {
    /// <summary>
    /// The classification engine.
    /// </summary>
    private readonly IClassify1 _classifyEngine;

    /// <summary>
    /// The options
    /// </summary>
    private readonly IOptions _options;

    public Classify(IClassify1 classifyEngine, IOptions options)
    {
      _classifyEngine = classifyEngine;
      _options = options;
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
      return _classifyEngine?.Categorize(categoryText, minPercentage, out wordsCategory, out categoryProbabilities) ?? -1;
    }

    public int Categorize(string categoryText, uint minPercentage)
    {
      // the category min percentage cannot be more than 100%.
      // it also cannot be less than 0, but we use a uint.
      if (minPercentage > 100)
      {
        throw new ArgumentException("The categotry minimum range cannot be more than 100%.");
      }

      return _classifyEngine?.Categorize(categoryText, minPercentage) ?? -1;
    }

    public int Categorize(Dictionary<MailStringCategories, string> categoryList)
    {
      return Categorize(string.Join(";", categoryList.Select(x => x.Value)), _options.MinPercentage);
    }

    public bool Train(string categoryName, string textToCategorise, string uniqueIdentifier, int weight)
    {
      if (weight <= 0)
      {
        throw new ArgumentException("The weight cannot be 0 or less!");
      }
      return _classifyEngine?.Train(categoryName, textToCategorise, uniqueIdentifier, weight) ?? false;
    }

    public bool UnTrain(string uniqueIdentifier, string textToCategorise)
    {
      return _classifyEngine?.UnTrain(uniqueIdentifier, textToCategorise) ?? false;
    }
  }
}
