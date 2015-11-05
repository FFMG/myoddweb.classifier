using System.Xml;

namespace myoddweb.classifier.core
{
  public class Category
  {
    public string Name { get; }

    public string XmlName { get; }

    public uint Id { get; }

    public string FolderId { get; }

    public Category( string name, uint id, string folderId )
    {
      Id = id;
      Name = name;
      XmlName = XmlEscape(name);
      FolderId = folderId;
    }

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
