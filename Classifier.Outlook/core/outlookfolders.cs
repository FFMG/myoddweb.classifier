using myoddweb.classifier.interfaces;
using System.Collections.Generic;
using System.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier.core
{
  public sealed class OutlookFolders : IFolders
  {
    private readonly Outlook.MAPIFolder _rootFolder;

    /// <summary>
    /// The list of folders as we know it.
    /// </summary>
    private List<IFolder> _folders;

    public OutlookFolders(Outlook.MAPIFolder rootFolder )
    {
      // get all the folders.
      _rootFolder = rootFolder;
    }

    public IEnumerable<IFolder> GetFolders()
    {
      // the folders.
      if (null != _folders)
      {
        return _folders;
      }

      // create the folders.
      _folders = new List<IFolder>();

      // enumerates
      EnumerateFolders(_rootFolder, _folders);

      // return the created folders.
      return _folders;
    }

    public IFolder FindFolderById(string folderId)
    {
      if (string.IsNullOrEmpty(folderId))
      {
        return null;
      }
      return GetFolders().FirstOrDefault(e => e.Id() == folderId);
    }

    // Uses recursion to enumerate Outlook subfolders.
    private void EnumerateFolders(Outlook.MAPIFolder folder, ICollection<IFolder> folders )
    {
      var childFolders = folder.Folders;
      if (childFolders.Count == 0)
      {
        return;
      }

      foreach (Outlook.MAPIFolder item in childFolders)
      {
        // if this is not a mail item then we are not interested.
        // things like calendars and so on.
        if (item.DefaultItemType != Outlook.OlItemType.olMailItem)
        {
          continue;
        }

        // add this folder to the list.
        folders.Add(new OutlookFolder(item, PrettyFolderPath(item.FolderPath)));

        // Call EnumerateFolders using childFolder.
        EnumerateFolders(item, folders);
      }
    }

    private string PrettyFolderPath( string folderPath )
    {
      // root path
      var rootFolderPath = _rootFolder.FolderPath;

      // the full folder path without the root path
      var path = folderPath.Replace(rootFolderPath, "" );

      // remove the posible leading 
      path = path.TrimStart( '\\' );

      // remove other '\' items.
      path = path.Replace("\\", " > ");

      // return the cleanup path.
      return path;

    }
  }
}
