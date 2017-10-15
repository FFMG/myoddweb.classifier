using System;
using System.Collections.Generic;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.core
{
  public class Folders : IFolders
  {
    /// <summary>
    /// The list of folders as we know it.
    /// </summary>
    protected List<IFolder> _folders = null;

    /// <summary>
    /// Just return a new list of folders.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<IFolder> GetFolders()
    {
      return _folders ?? new List<IFolder>();
    }
  }
}
