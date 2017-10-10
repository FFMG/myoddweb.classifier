using Classifier.Interfaces.Helpers;
using System.Collections.Generic;

namespace myoddweb.classifier.interfaces
{
  public interface IMagnets
  {
    /// <summary>
    /// Get our complete list of magnets.
    /// </summary>
    /// <returns>List of magnets or null</returns>
    List<Magnet> GetMagnets();

    /// <summary>
    /// Create a magnet.
    /// </summary>
    /// <param name="randomName"></param>
    /// <param name="ruleType"></param>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    int CreateMagnet(string randomName, int ruleType, int categoryId);

    /// <summary>
    /// Delete a magnet given a magnet id.
    /// </summary>
    /// <param name="magnetId"></param>
    /// <returns></returns>
    bool DeleteMagnet(int magnetId);

    /// <summary>
    /// Update a magnet
    /// </summary>
    /// <param name="magnetId">The magnet id</param>
    /// <param name="magnetName">The magnet name</param>
    /// <param name="ruleType">The rule type we are updating to</param>
    /// <param name="categoryTarget">The target category when the rule is matched.</param>
    /// <returns></returns>
    bool UpdateMagnet(int magnetId, string magnetName, int ruleType, int categoryTarget);

    /// <summary>
    /// Update an existing magnet
    /// We only update it if the values do not match exactly.
    /// </summary>
    /// <param name="currentMagnet">The current magnet we might update</param>
    /// <param name="magnetName">The updated name</param>
    /// <param name="ruleType">The updated rule type</param>
    /// <param name="categoryTarget">the updated category target.</param>
    /// <returns>boolean success or not.</returns>
    bool UpdateMagnet(Magnet currentMagnet, string magnetName, int ruleType, int categoryTarget);
  }
}
