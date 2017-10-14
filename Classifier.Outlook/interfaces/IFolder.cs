namespace myoddweb.classifier.interfaces
{
  public interface IFolder
  {
    /// <summary>
    /// The unique folder id.
    /// </summary>
    /// <returns>string the unique folder id.</returns>
    string Id();

    /// <summary>
    /// The folder name
    /// </summary>
    /// <returns>string the folder name.</returns>
    string Name();

    /// <summary>
    /// The folder path, either the full path or the 'pretty' path.
    /// </summary>
    /// <param name="prettyDisplay">boolean if we want the pretty path or the full path.</param>
    /// <returns>string the path</returns>
    string Path(bool prettyDisplay);
  }
}
