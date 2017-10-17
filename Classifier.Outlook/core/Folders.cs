using System.Collections.Generic;
using System.Linq;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.core
{
  public sealed class Folders : IFolders
  {
    /// <summary>
    /// The list of folders as we know it.
    /// </summary>
    private readonly List<IFolder> _folders = null;

    /// <summary>
    /// Just return a new list of folders.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IFolder> GetFolders()
    {
      return _folders ?? new List<IFolder>();
    }

    /// <summary>
    /// Find a folder given the app specific folder id.
    /// </summary>
    /// <param name="folderId"></param>
    /// <returns></returns>
    public IFolder FindFolderById(string folderId)
    {
      return string.IsNullOrEmpty(folderId) ? null : GetFolders().FirstOrDefault(e => e.Id() == folderId);
    }
  }
}
