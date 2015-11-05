using System.Collections.Generic;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier.core
{
  public class Folders
  {
    /// <summary>
    /// The list of folders as we know it.
    /// </summary>
    private List<Folder> _folders = null;

    private readonly Outlook.MAPIFolder _rootFolder;
    public Folders(Outlook.MAPIFolder rootFolder )
    {
      // get all the folders.
      _rootFolder = rootFolder;
    }

    public List<Folder> GetFolders()
    {
      // the folders.
      if (null != _folders)
      {
        return _folders;
      }

      // create the folders.
      _folders = new List<Folder>();

      // enumerates
      EnumerateFolders(_rootFolder, _folders);

      // return the created folders.
      return _folders;
    }

    // Uses recursion to enumerate Outlook subfolders.
    private void EnumerateFolders(Outlook.MAPIFolder folder, ICollection<Folder> folders )
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

        // child folder item
        var childFolder = item as Outlook.Folder;

        // add this folder to the list.
        folders.Add( new Folder(childFolder, PrettyFolderPath(childFolder.FolderPath) ) );

        // Call EnumerateFolders using childFolder.
        EnumerateFolders(childFolder, folders );
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
