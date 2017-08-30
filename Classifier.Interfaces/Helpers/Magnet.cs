namespace Classifier.Interfaces.Helpers
{
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
}
