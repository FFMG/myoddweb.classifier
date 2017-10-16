using System.Xml;

namespace myoddweb.classifier.core
{
  public class Category
  {
    /// <summary>
    /// The given name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The excapted name
    /// </summary>
    private string _xmlName;

    /// <summary>
    /// The excapted name, if it does not exist, it will be created.
    /// </summary>
    public string XmlName => _xmlName ?? (_xmlName = XmlEscape(Name));

    /// <summary>
    /// The given id
    /// </summary>
    public uint Id { get; }

    /// <summary>
    /// The given folder unique indentifier.
    /// </summary>
    public string FolderId { get; }

    public Category( string name, uint id, string folderId )
    {
      Id = id;
      Name = name;
      FolderId = folderId;
    }

    /// <summary>
    /// Excape the given string.
    /// </summary>
    /// <param name="unescaped"></param>
    /// <returns></returns>
    protected static string XmlEscape(string unescaped)
    {
      var doc = new XmlDocument();
      var node = doc.CreateElement("root");
      node.InnerText = unescaped;
      return node.InnerXml;
    }

    #region Compare Category
    public static bool operator ==(Category lhs, Category rhs )
    {
      return (lhs?.Id == rhs?.Id &&
              lhs?.Name == rhs?.Name && 
              lhs?.FolderId == rhs?.FolderId
             );
    }

    public static bool operator !=(Category lhs, Category rhs)
    {
      return !(lhs == rhs);
    }

    protected bool Equals(Category other)
    {
      return string.Equals(Name, other.Name) && Id == other.Id && string.Equals(FolderId, other.FolderId);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.GetType() == GetType() && Equals((Category)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = Name?.GetHashCode() ?? 0;
        hashCode = (hashCode * 397) ^ (int)Id;
        hashCode = (hashCode * 397) ^ (FolderId?.GetHashCode() ?? 0);
        return hashCode;
      }
    }
#endregion
  }
}
