using Microsoft.Office.Core;

namespace myoddweb.classifier.core
{
  public partial class CustomUI
  {
    /// <summary>
    /// Return tru of the menu is visible or not.
    /// </summary>
    /// <param name="control"></param>
    /// <returns>false if we have no valid message selected.</returns>
    public bool IsMenuFolderMenuVisible(IRibbonControl control)
    {
      // if we have no engine, then we have a problem somehwere.
      if (null == _engine)
      {
        return false;
      }

      // if we have no categories then something is 'broken'
      // so we do not want our menu to show.
      if (_engine.Categories.Count == 0)
      {
        return false;
      }

      //  if we have a valid item, then we don't return null.
      return (GetFolderContent(control) != null);
    }

    public string GetFolderContent(IRibbonControl control)
    {
      // if we have no categories then something is 'broken'
      // so we do not want our menu to show.
      if (_engine.Categories.Count == 0)
      {
        return "";
      }

      return GetContenteWithPosibleFolder(GetMultipleFoldersFromControl(control));
    }

    /// <summary>
    /// Display the main menu label.
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public string GetMenuFolderLabel(IRibbonControl control)
    {
      return "Myoddweb.Classifier";
    }
  }
}
