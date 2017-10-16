using myoddweb.classifier.interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier.core
{
  public class OutlookFolder : IFolder
  {
    /// <summary>
    /// Get access to the outlook folder.
    /// </summary>
    public readonly Outlook.MAPIFolder Folder;

    /// <summary>
    /// The cleanup name of the folder without the root folder.
    /// </summary>
    private readonly string _prettyFolderPath;

    public OutlookFolder(Outlook.MAPIFolder folder, string prettyFolderPath)
    {
      Folder = folder;
      _prettyFolderPath = prettyFolderPath;
    }

    /// <summary>
    /// The folder path, either the full path or the 'pretty' path.
    /// </summary>
    /// <param name="prettyDisplay">boolean if we want the pretty path or the full path.</param>
    /// <returns>string the path</returns>
    public string Path( bool prettyDisplay )
    {
      return false == prettyDisplay ? Folder.FolderPath : _prettyFolderPath;
    }

    /// <summary>
    /// The unique folder id.
    /// </summary>
    /// <returns>string the unique folder id.</returns>
    public string Id()
    {
      return Folder.EntryID;
    }

    /// <summary>
    /// The folder name
    /// </summary>
    /// <returns>string folder name.</returns>
    public string Name()
    {
      return Folder.Name;
    }
  }
}
