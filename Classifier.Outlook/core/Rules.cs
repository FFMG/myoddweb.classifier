using System.Collections;
using System.Collections.Generic;

namespace myoddweb.classifier.core
{
  public class Rules : IEnumerable<Rule>
  {
    private static List<Rule> _rules;

    public Rules()
    {
      _rules = new List<Rule>()
      {
        new Rule() { Description = "From Email", Type = RuleTypes.FromEmail },
        new Rule() { Description = "To Email", Type = RuleTypes.ToEmail },
        new Rule() { Description = "From Email host", Type = RuleTypes.FromEmailHost },
        new Rule() { Description = "To Email host", Type = RuleTypes.ToEmailHost }
      };
    }

    /// <summary>
    /// Enumerate all the rules we have,
    /// </summary>
    /// <returns>All the rules.</returns>
    public IEnumerator<Rule> GetEnumerator()
    {
      return _rules.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _rules.GetEnumerator();
    }

    /// <summary>
    /// Get a rule given a rule type.
    /// </summary>
    /// <param name="ruleType">the ruletype we are looking for.</param>
    /// <returns></returns>
    internal Rule Find(int ruleType )
    {
      foreach( var rule in _rules )
      {
        if( rule == ruleType )
        {
          return rule;
        }
      }
      return null;
    }
  }
}