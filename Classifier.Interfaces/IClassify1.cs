
using System.Collections.Generic;

namespace Classifier.Interfaces
{
  public class WordCategory
  {
    public WordCategory()
    {
      Word = "";
      Category = -1;
      Probability = 0;
    }

    /// <summary>
    /// The word been categoried.
    /// </summary>
    public string Word;

    /// <summary>
    /// The word category
    /// </summary>
    public int Category;

    /// <summary>
    /// The word probability percentage.
    /// </summary>
    public double Probability;
  };

  public class Magnet
  {
    /// <summary>
    /// The constructor.
    /// </summary>
    public Magnet()
    {
      // reset all the values.
      Id = Rule = Category = -1;
      Name = "";
    }

    /// <summary>
    /// The magnet id
    /// </summary>
    public int Id;

    /// <summary>
    /// The magnet name
    /// </summary>
    public string Name;

    /// <summary>
    /// The magnet rule type, only specific to the client.
    /// </summary>
    public int Rule;

    /// <summary>
    /// The magnet category, (if the magnet is triggered).
    /// </summary>
    public int Category;
  }

  public interface IClassify1
  {
    /// <summary>
    /// Get the engine version number.
    /// </summary>
    /// <returns>int the engine version number.</returns>
    int GetEngineVersion();

    /// <summary>
    /// Inisialise all the values needed, create the classifier instance.
    /// </summary>
    /// <param name="eventViewSource">string when we are logging a defect this is the event viewer name.</param>
    /// <param name="enginePath">string the path to the classifier dll.</param>
    /// <param name="databasePath">string pasth to the database we will be using.</param>
    /// <returns>boolean success or not of the operation.</returns>
    bool Initialise( string eventViewSource, string enginePath, string databasePath);

    /// <summary>
    /// All the managed/unmaned code to release all the resources.
    /// </summary>
    void Release();

    /// <summary>
    /// Set a configuration value that will be used to run the software
    /// </summary>
    /// <param name="configName">string the name of the value we want to set.</param>
    /// <param name="configValue">string the value we want to pass.</param>
    /// <returns>boolean success or not.</returns>
    bool SetConfig(string configName, string configValue);

    /// <summary>
    /// Get a configuration value that will be saved.
    /// </summary>
    /// <param name="configName">string the name of the value we are looking for.</param>
    /// <param name="configValue">out string the result of the value.</param>
    /// <returns>boolean true if we found the value or false if not</returns>
    bool GetConfig(string configName, out string configValue);

    /// <summary>
    /// Train a text/document to be into a given category id
    /// </summary>
    /// <param name="categoryName">the name of the category we are adding it to.</param>
    /// <param name="textToCategorise">the text/document we want to categorise.</param>
    /// <param name="uniqueIdentifier">A unique value to help us ensure that we only train this document once. This value is unique to the app</param>
    /// <param name="weight">The weight of this document</param>
    /// <returns>success or not.</returns>
    bool Train( string categoryName, string textToCategorise, string uniqueIdentifier, int weight );

    /// <summary>
    /// UnTrain a text/document from a given category id
    /// </summary>
    /// <param name="uniqueIdentifier">A unique value to help us ensure that we only train this document once. This value is unique to the app</param>
    /// <param name="textToCategorise">the text/document we want to un train.</param>
    /// <returns>success or not.</returns>
    bool UnTrain( string uniqueIdentifier, string textToCategorise);

    /// <summary>
    /// Get the id of a category
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    int GetCategory(string categoryName);

    /// <summary>
    /// Given a unique identifier, try and get the known caytegory id.
    /// </summary>
    /// <returns>integer the category id or -1 if we don't know.</returns>
    int GetCategoryFromUniqueId(string uniqueIdentifier);

    /// <summary>
    /// Categorise a text/document. 
    /// </summary>
    /// <param name="textToCategorise">The text we want to categorise</param>
    /// <param name="minPercentage">The minimum percentage we want to allow.</param>
    /// <returns>either the id of the category or -1 if we do not know.</returns>
    int Categorize(string textToCategorise, uint minPercentage );

    /// <summary>
    /// Categorise a text/document and ca. 
    /// </summary>
    /// <param name="textToCategorise">The text we want to categorise</param>
    /// <param name="minPercentage">The minimum percentage we want to allow.</param>
    /// <param name="wordsCategory">Each works and their categories.</param>
    /// <param name="categoryProbabilities">Map or categories > probabilities.</param>
    /// <returns>either the id of the category or -1 if we do not know.</returns>
    int Categorize(string textToCategorise, uint minPercentage, out List<WordCategory> wordsCategory, out Dictionary<int, double> categoryProbabilities );

    /// <summary>
    /// Get all the categories currently on record.
    /// </summary>
    /// <param name="categories">out Dictionary<int, string> the list of id => name of categories.</param>
    /// <returns>int the number of items found or -1 if there was an error</returns>
    int GetCategories(out Dictionary<int, string> categories);

    /// <summary>
    /// Rename a category from an old name to a new one.
    /// If the new name already exists, we return false.
    /// Any other error we return false.
    /// </summary>
    /// <param name="oldName">string the old category name</param>
    /// <param name="newName">string the new category name</param>
    /// <returns>boolean success or error.</returns>
    bool RenameCategory(string oldName, string newName);

    /// <summary>
    /// Delete an existing category.
    /// </summary>
    /// <param name="categoryName">The category we want to remove.</param>
    /// <returns>boolean if there is an error or not.</returns>
    bool DeleteCategory(string categoryName);

    /// <summary>
    /// Get all the created magnets.
    /// </summary>
    /// <param name="magnets">list of magnets.</param>
    /// <returns>int the number of magnets or -1 if there was an error.</returns>
    int GetMagnets( out List<Magnet> magnets);

    /// <summary>
    /// Create a new magnet
    /// </summary>
    /// <param name="magnetName">The name of the magent we want to create.</param>
    /// <param name="ruleType">int an app defined rule type that tell the app what to do when the rule is matched.</param>
    /// <param name="categoryTarget">int the target category.</param>
    /// <returns>number the created magnet id.</returns>
    int CreateMagnet(string magnetName, int ruleType, int categoryTarget);

    /// <summary>
    /// Update a single magnet
    /// </summary>
    /// <param name="id">The id we want to update.</param>
    /// <param name="magnetName">The updated name.</param>
    /// <param name="ruleType">The updated type.</param>
    /// <param name="categoryTarget">The updated category target, (when the magnet is true)</param>
    /// <returns>boolean success or not.</returns>
    bool UpdateMagnet(int id, string magnetName, int ruleType, int categoryTarget);

    /// <summary>
    /// Delete a magnet from our list of magnets
    /// We return false if there was an error and/or if the magnet does not exist.
    /// </summary>
    /// <param name="id">The manet id we are trying to delete.</param>
    /// <returns>boolean if the item was delete or false if not.</returns>
    bool DeleteMagnet(int id);
  }
}
