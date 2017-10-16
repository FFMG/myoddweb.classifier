using System.Collections.Generic;
using Classifier.Interfaces.Helpers;
using myoddweb.classifier.interfaces;
using Classifier.Interfaces;

namespace myoddweb.classifier.core
{
  public class Magnets : IMagnets
  {
    /// <summary>
    /// The classification engine.
    /// </summary>
    private readonly IClassify1 _classifyEngine;

    public Magnets(IClassify1 classifyEngine)
    {
      _classifyEngine = classifyEngine;
    }

    /// <summary>
    /// Create a magnet.
    /// </summary>
    /// <param name="randomName"></param>
    /// <param name="ruleType"></param>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public int CreateMagnet(string randomName, int ruleType, int categoryId)
    {
      return _classifyEngine.CreateMagnet(randomName, ruleType, categoryId);
    }

    /// <summary>
    /// Delete a magnet given a magnet id.
    /// </summary>
    /// <param name="magnetId"></param>
    /// <returns></returns>
    public bool DeleteMagnet(int magnetId)
    {
      return _classifyEngine.DeleteMagnet(magnetId);
    }

    /// <summary>
    /// Update a magnet
    /// </summary>
    /// <param name="magnetId">The magnet id</param>
    /// <param name="magnetName">The magnet name</param>
    /// <param name="ruleType">The rule type we are updating to</param>
    /// <param name="categoryTarget">The target category when the rule is matched.</param>
    /// <returns></returns>
    public bool UpdateMagnet(int magnetId, string magnetName, int ruleType, int categoryTarget)
    {
      return _classifyEngine.UpdateMagnet(magnetId, magnetName, ruleType, categoryTarget);
    }

    /// <summary>
    /// Update an existing magnet
    /// We only update it if the values do not match exactly.
    /// </summary>
    /// <param name="currentMagnet">The current magnet we might update</param>
    /// <param name="magnetName">The updated name</param>
    /// <param name="ruleType">The updated rule type</param>
    /// <param name="categoryTarget">the updated category target.</param>
    /// <returns>boolean success or not.</returns>
    public bool UpdateMagnet(Magnet currentMagnet, string magnetName, int ruleType, int categoryTarget)
    {
      // sanity check does the value exist?
      if (null == currentMagnet)
      {
        return false;
      }

      // does it already match what we have?
      if (currentMagnet.Category == categoryTarget && currentMagnet.Rule == ruleType && currentMagnet.Name == magnetName)
      {
        //  nothing to do.
        return true;
      }

      // looks like we might do an update, do it now.
      return UpdateMagnet(currentMagnet.Id, magnetName, ruleType, categoryTarget);
    }

    /// <summary>
    /// Get our complete list of magnets.
    /// </summary>
    /// <returns>List of magnets or null</returns>
    public List<Magnet> GetMagnets()
    {
      List<Magnet> magnets;
      return -1 == _classifyEngine.GetMagnets(out magnets) ? null : magnets;
    }
  }
}
