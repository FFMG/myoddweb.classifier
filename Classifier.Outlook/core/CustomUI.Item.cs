using Microsoft.Office.Core;

namespace myoddweb.classifier.core
{
  public partial class CustomUI
  {
    /// <summary>
    /// GetContent callback
    /// </summary>
    /// <param name="control">The control.</param>
    /// <returns></returns>
    public string GetContent(IRibbonControl control)
    {
      // do we have a valid mail item?
      // if not, then display the default menu item
      var mailItem = GetMailItemFromControl(control);
      return null == mailItem ? BuildMenu( BuildCommonMenus() ) : GetContentWithPosibleMailItemAsync(mailItem).GetAwaiter().GetResult();
    }
    
    /// <summary>
    /// Display the main menu label.
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public string GetMainLabel(IRibbonControl control)
    {
      return "Myoddweb.Classifier";
    }

    /// <summary>
    /// Return tru of the menu is visible or not.
    /// </summary>
    /// <param name="control"></param>
    /// <returns>false if we have no valid message selected.</returns>
    public bool IsMenuVisible(IRibbonControl control)
    {
      // if we have no engine, then we have a problem somehwere.
      if (null == _engine)
      {
        return false;
      }

      // always display the menu
      // - if we have no categories, the menu will still display.
      // - if we have a valid item, then we don't return null.
      return (GetMailItemFromControl(control) != null);
    }
  }
}
