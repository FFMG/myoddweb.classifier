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

    /// <summary>
    /// Find a folder given the app specific folder id.
    /// </summary>
    /// <param name="folderId"></param>
    /// <returns></returns>
    IFolder FindFolderById(string folderId);
  }
}
