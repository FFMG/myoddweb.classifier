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
      // if not then we are not going to display it.
      var mailItem = GetMailItemFromControl(control);
      return null == mailItem ? "" : GetContentWithPosibleMailItemAsync(mailItem).GetAwaiter().GetResult();
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

      // if we have no categories then something is 'broken'
      // so we do not want our menu to show.
      if (_engine.Categories.Count == 0)
      {
        return false;
      }

      //  if we have a valid item, then we don't return null.
      return (GetMailItemFromControl(control) != null);
    }
  }
}
