using System.Collections.Generic;

namespace myoddweb.classifier.interfaces
{
  public interface IFolders
  {
    /// <summary>
    /// Get all the folders.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IFolder> GetFolders();
  }
}
