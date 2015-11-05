namespace myoddweb.classifier.core
{
  public enum RuleTypes
  {
    RuleTypesFirst,
    FromEmail = RuleTypesFirst,  //  the 'from' email address must match
    ToEmail,                     //  the 'to' email address must match
    FromEmailHost,               //  the host of the 'from' email must match
    ToEmailHost,                 //  the host of the 'to' email must match
    RuleTypesLast
  }

  public class Rule
  {
    /// <summary>
    /// The description we want to display this rule as.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The rule type to apply when this rule is matched.
    /// </summary>
    public RuleTypes Type { get; set; }

    /// <summary>
    /// Convert the rule to string
    /// </summary>
    /// <param name="r">the rule we want to contvert to string</param>
    public static implicit operator string (Rule r)
    {
      // return the description we have.
      return r.Description;
    }

    /// <summary>
    /// Convert the rule to a rule type enum.
    /// </summary>
    /// <param name="r">The rule we want to convert to a rule type.</param>
    public static implicit operator RuleTypes(Rule r)
    {
      // return the rule type
      return r.Type;
    }

    /// <summary>
    /// Convert the rule to an integer.
    /// </summary>
    /// <param name="r">The rule we want to convert to an int.</param>
    public static implicit operator int(Rule r)
    {
      // simply cast the enum to an int.
      return (int)r.Type;
    }
  }
}