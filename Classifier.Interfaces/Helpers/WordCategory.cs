namespace Classifier.Interfaces.Helpers
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
}
