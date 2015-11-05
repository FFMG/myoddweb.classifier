using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier
{
  public class Folder
  {
    /// <summary>
    /// The outlook folder.
    /// </summary>
    private readonly Outlook.Folder _folder = null;

    /// <summary>
    /// Get access to the outlook folder.
    /// </summary>
    public Outlook.Folder OutlookFolder { get { return _folder; } }

    /// <summary>
    /// The cleanup name of the folder without the root folder.
    /// </summary>
    private readonly string _prettyFolderPath = null;

    public Folder(Outlook.Folder folder, string prettyFolderPath)
    {
      _folder = folder;
      _prettyFolderPath = prettyFolderPath;
    }

    /// <summary>
    /// The folder path, either the full path or the 'pretty' path.
    /// </summary>
    /// <param name="prettyDisplay">boolean if we want the pretty path or the full path.</param>
    /// <returns>string the path</returns>
    public string Path( bool prettyDisplay )
    {
      return false == prettyDisplay ? _folder.FolderPath : _prettyFolderPath;
    }

    /// <summary>
    /// The unique folder id.
    /// </summary>
    /// <returns>string the unique folder id.</returns>
    public string Id()
    {
      return _folder.EntryID;
    }
  }
}
