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

      // always display the menu
      // - if we have no categories, the menu will still display.
      // - if we have a valid item, then we don't return null.
      return (GetFolderContent(control) != null);
    }

    public string GetFolderContent(IRibbonControl control)
    {
      // do we have a valid folder item?
      // if not, then display the default menu item
      var folders = GetMultipleFoldersFromControl(control);
      return null == folders ? BuildMenu(BuildCommonMenus()) : GetContenteWithPosibleFolder(folders);
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
