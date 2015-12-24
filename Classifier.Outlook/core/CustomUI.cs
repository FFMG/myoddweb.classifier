using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Outlook;
using myoddweb.classifier.forms;
using Exception = System.Exception;
using Office = Microsoft.Office.Core;

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.
namespace myoddweb.classifier.core
{
  [ComVisible(true)]
  public class CustomUI : Office.IRibbonExtensibility
  {
    /// <summary>
    /// The max number of items we will try to classify.
    /// </summary>
    private const int MaxNumberOfItemsToClassify = 100;

    /// <summary>
    /// all the categories.
    /// </summary>
    private readonly Categories _categories = null;

    /// <summary>
    /// All the options
    /// </summary>
    private readonly Options _options = null;

    /// <summary>
    /// The engine that does the classification.
    /// </summary>
    private readonly Engine _engine = null;

    private Office.IRibbonUI ribbon;

    public CustomUI(Engine engine, Categories categories, Options options )
    {
      // the engine.
      _engine = engine;

      // the categories.
      _categories = categories;

      // the options
      _options = options;
    }

    #region IRibbonExtensibility Members

    public string GetCustomUI(string ribbonID)
    {
      return GetResourceText("myoddweb.classifier.Core.CustomUI.xml");
    }

    #endregion

    #region Ribbon Callbacks
    //Create callback methods here. For more information about adding callback methods, visit http://go.microsoft.com/fwlink/?LinkID=271226

    public void Ribbon_Load(Office.IRibbonUI ribbonUI)
    {
      this.ribbon = ribbonUI;
    }

    #endregion

    #region Helpers

    private static string GetResourceText(string resourceName)
    {
      Assembly asm = Assembly.GetExecutingAssembly();
      string[] resourceNames = asm.GetManifestResourceNames();
      for (int i = 0; i < resourceNames.Length; ++i)
      {
        if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
        {
          using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
          {
            if (resourceReader != null)
            {
              return resourceReader.ReadToEnd();
            }
          }
        }
      }
      return null;
    }

    #endregion


    //
    //  https://naimhamadi.wordpress.com/2014/07/15/adding-a-custom-context-menu-item-to-outlook-2013/
    //

    private static _MailItem GetMailItemFromRibbonControl(Office.IRibbonControl control)
    {
      var items = GetMultipleMailItemFromRibbonControl( control );
      return items?.First();
    }

    private static List<_MailItem> GetMultipleMailItemFromRibbonControl(Office.IRibbonControl control)
    {
      var explorer = Globals.ThisAddIn.Application.ActiveExplorer();
      if (explorer?.Selection == null || explorer.Selection.Count <= 0)
      {
        return null;
      }

      var listItems = new List<_MailItem>();
      foreach (var selectionItem in explorer.Selection)
      {
        var item = selectionItem as _MailItem;
        if (null != item)
        {
          if( !Categories.IsUsableClassNameForClassification(item?.MessageClass) )
          {
            continue;
          }
          listItems.Add(item);
        }
      }
      return (listItems.Count == 0 ? null : listItems);
    }

    /// <summary>
    /// This is when a category is clicked.
    /// The user wants us to categorise this mail item.
    /// </summary>
    /// <param name="control"></param>
    public async void OnSelectCategory(Office.IRibbonControl control)
    {
      // get all the mail items.
      var mailItems = GetMultipleMailItemFromRibbonControl(control);
      // get the category id that was seleted.
      var regex = new Regex("myoddweb.classifier_manage_([0-9]+)");
      if (!regex.IsMatch(control.Id))
      {
        // @todo log this as an error in the regex!
        return;
      }

      //  get the id.
      var match = regex.Match(control.Id);
      var categoryId = Convert.ToUInt32(match.Groups[1].Value);

      // change the cursor for long operation
      var currentCursor = Cursor.Current;
      Cursor.Current = Cursors.WaitCursor;

      // do the operation.
      var resultOfOperation = await ClassifyItems(mailItems, categoryId).ConfigureAwait( false );

      // reset the cursor.
      Cursor.Current = currentCursor;

      // if there was a problem, we need to tell the user about it.
      if (resultOfOperation == false)
      {
        MessageBox.Show("There was a problem updating the category, is the datbase locked?", "Could not classify", MessageBoxButtons.OK, MessageBoxIcon.Error );
      }
    }

    /// <summary>
    /// Classify a multiple mail items at once.
    /// </summary>
    /// <param name="mailItems">The list of mail items we are trying to categorise.</param>
    /// <param name="categoryId">The id we want to categorise to.</param>
    /// <returns></returns>
    private async Task<bool> ClassifyItems(IEnumerable<_MailItem> mailItems, uint categoryId )
    { 
      // look around each items.
      foreach (var mailItem in mailItems.Take(MaxNumberOfItemsToClassify))
      {
        try
        {
          var watch = Stopwatch.StartNew();

          // log a message to indicate when we are trying to do.
          Debug.WriteLine($"Classifying message id {mailItem.EntryID} to category {categoryId}");

          // we have all the information we need
          // we can now categorize the mail.
          if (myoddweb.classifier.Errors.Success == await ClassifyMailAsync(mailItem, categoryId).ConfigureAwait(false) )
          {
            // stop the timer.
            watch.Stop();

            // and log how long this took
            Debug.WriteLine($"  [Success] Classifying took {(double)watch.ElapsedMilliseconds / 1000}.");
            continue;
          }

          // debug
          Debug.WriteLine($"There was a problem setting the mail for message id {mailItem.EntryID}");

          // stop the timer.
          watch.Stop();

          //  how long this took.
          Debug.WriteLine($"  [Failed] Classifying took {(double)watch.ElapsedMilliseconds / 1000}.");

          // log the error
          _engine.LogEventError($"There was a problem setting the mail for message id {mailItem.EntryID} ('{mailItem.Subject}').");

          // no need to go further, something broke.
          return false;
        }
        catch
        {
          // log that this did not work.
          _engine.LogEventError($"I was unable to categorise mail {mailItem.EntryID} ('{mailItem.Subject}').");

          // bail out.
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Categorise a mail item to a certain category it.
    /// </summary>
    /// <param name="mailItem">The mail item we wish to categorise</param>
    /// <param name="id">The category id number we want to set this to.</param>
    /// <returns>boolean success or not.</returns>
    async private Task<myoddweb.classifier.Errors> ClassifyMailAsync( _MailItem mailItem, uint id )
    {
      var categories = GetAllCategories();

      // we know this is a user selected item
      // so we can get the weight from the options.
      return await categories.ClassifyAsync(mailItem, id, _options.UserWeight);
    }

    /// <summary>
    /// Show the various options and allow the user to change certain settings. 
    /// </summary>
    /// <param name="control"></param>
    public void OnManageMore(Office.IRibbonControl control)
    {
      if (null == _options)
      {
        return;
      }

      using (var optionsForm = new OptionsForm( engine: _engine, options: _options, categories: _categories ))
      {
        optionsForm.ShowDialog();
      }
    }

    public void OnMagnet(Office.IRibbonControl control)
    {
      var items = GetMultipleMailItemFromRibbonControl(control);
      if (items == null || items.Count == 0 || items.Count > 1)
      {
        return;
      }

      // get the one and only item
      var mailItem = items.First();

      // update the magnets list.
      using (var magnetMailItemForm = new MagnetMailItemForm( _engine, mailItem, _categories ))
      {
        magnetMailItemForm.ShowDialog();
      }
    }

    public string GetMultipleItemsContent(Office.IRibbonControl control)
    {
      // do we have a valid mail item?
      // if not then we are not going to display it.
      var mailItem = GetMailItemFromRibbonControl(control);
      return null == mailItem ? "" : GetContentWithPosibleMailItem(null);
    }


    /// <summary>
    /// GetContent callback
    /// </summary>
    /// <param name="control">The control.</param>
    /// <returns></returns>
    public string GetContent(Office.IRibbonControl control)
    {
      // do we have a valid mail item?
      // if not then we are not going to display it.
      var mailItem = GetMailItemFromRibbonControl(control);
      return null == mailItem ? "" : GetContentWithPosibleMailItem( mailItem );
    }

    /// <summary>
    /// Guess the posible new category id 
    /// </summary>
    /// <param name="mailItem">the mail item itself.</param>
    /// <param name="categories">the categories tool we will use to re-categorise</param>
    /// <param name="currentCategoryId">The current value of the category</param>
    /// <returns>Categories.CategorizeResponse the new id or -1 if we don't know.</returns>
    protected Categories.CategorizeResponse GuessPosibleCategory( _MailItem mailItem, int currentCategoryId, Categories categories )
    {
      var guessCategoryResponse = new Categories.CategorizeResponse
      {
        CategoryId = 0,
        WasMagnetUsed = false
      };

      if ( !_options.ReCheckIfCtrlKeyIsDown || (Control.ModifierKeys & Keys.Control) != Keys.Control)
      {
        // if we do not want to check options, then we don't want to do that.
        if ( !_options.ReCheckCategories )
        {
          return guessCategoryResponse;
        }

        // if we currently have a category and we only want to check the
        // unknown categories, then we musn't check.
        if (currentCategoryId != -1 && _options.CheckIfUnownCategory)
        {
          return guessCategoryResponse;
        }
      }

      var currentCursor = Cursor.Current;
      Cursor.Current = Cursors.WaitCursor;

      try
      {
        // guess where it could be going to now.
        if (mailItem != null)
        {
          guessCategoryResponse = categories.CategorizeAsync(mailItem).Result;
        }
      }
      catch (Exception)
      {
        // @todo we need to log that there was an issue.
        guessCategoryResponse = new Categories.CategorizeResponse
        {
          CategoryId = 0,
          WasMagnetUsed = false
        };
      }

      // reset the cursor.
      Cursor.Current = currentCursor;


      // add a debug message.
      Debug.WriteLine(guessCategoryResponse.CategoryId != currentCategoryId
        ? $"My new classifying guess for this message is : {guessCategoryResponse.CategoryId}"
        : $"My classifying guess for this message is category remains the same : {guessCategoryResponse.CategoryId}");

      // return what we found
      return guessCategoryResponse;
    }

    /// <summary>
    /// GetContent callback
    /// </summary>
    /// <param name="mailItem">The mail item.</param>
    /// <returns></returns>
    public string GetContentWithPosibleMailItem( _MailItem mailItem )
    { 
      // create the menu xml
      var translationsXml = new StringBuilder(@"<menu xmlns=""http://schemas.microsoft.com/office/2009/07/customui"">");

      // get all the cateories.
      var categories = GetAllCategories();

      // if we have no categories then something is 'broken'
      // so we do not want our menu to show.
      if (null == categories)
      {
        return "";
      }

      // do we know the current category?
      var currentCategory = mailItem == null ? null : categories.GetCategoryFromMailItem(mailItem);

      // get the current category if?
      var currentCategoryId = currentCategory == null ? -1 : (int)currentCategory.Id;

      // try and guess the new category
      var guessCategoryResponse = GuessPosibleCategory(mailItem, currentCategoryId, categories);

      // and create a menu for all of them.
      foreach (var category in categories.List() )
      {
        var safeLabel = category.XmlName.Replace( "&amp;", "&amp;&amp;");
        var getImage = "";
        if (currentCategoryId == category.Id && guessCategoryResponse.CategoryId == category.Id)
        {
          // best guess selected image.
          getImage = @"getImage=""GetImageBoth""";
        }
        else if (currentCategoryId == category.Id)
        {
          getImage = @"getImage=""GetImageSelected""";
        }
        else if (guessCategoryResponse.CategoryId == category.Id)
        {
          getImage = @"getImage=""GetImageMaybe""";
        }
        else
        {
          //  no image
        }

        // best guess selected image.
        translationsXml.Append(
          $@"<button id =""myoddweb.classifier_manage_{category.Id}"" label=""{safeLabel}"" onAction=""OnSelectCategory"" {getImage}/>"
          );
      }

      // if we have existing categories, add a separator
      // otherwise we don't need to
      if (categories.Count > 0 )
      {
        translationsXml.Append(@"<menuSeparator id=""separator"" />");
      }

      // If the value is null, it means we have more than one mail item
      // we cannot set a magnet with multiple mails.
      // well, we could, but it is just to much to keep it simple to the user.
      if (mailItem != null)
      {
        translationsXml.Append(
          $@"<button id =""{"myoddweb.classifier_magnet"}"" label=""{"Magnet ..."}"" onAction=""{"OnMagnet"}"" />");
      }

      translationsXml.Append(
        $@"<button id =""{"myoddweb.classifier_settings"}"" label=""{"Options ..."}"" onAction=""{"OnManageMore"}"" getImage=""GetImageOptions""/>");

      translationsXml.Append(@"</menu>");

      var menu = translationsXml.ToString();
      return menu;
    }

    /// <summary>
    /// The bitmap we will be using when we know the selected category.
    /// </summary>
    /// <param name="control"></param>
    /// <returns>The 'valid' bitmap</returns>
    public Bitmap GetImageSelected(Office.IRibbonControl control)
    {
      return Properties.Resources.valid;
    }

    /// <summary>
    /// The bitmap we will be using when we know the selected category.
    /// </summary>
    /// <param name="control"></param>
    /// <returns>The 'valid' bitmap</returns>
    public Bitmap GetImageBoth(Office.IRibbonControl control)
    {
      return Properties.Resources.both;
    }

    public Bitmap GetImageOptions(Office.IRibbonControl control)
    {
      return Properties.Resources.settings;
    }

    public Bitmap GetImageClassify(Office.IRibbonControl control)
    {
      return Properties.Resources.classify;
    }
    
    public Bitmap GetImageMaybe(Office.IRibbonControl control)
    {
      return Properties.Resources.maybe;
    }

    private Categories GetAllCategories()
    {
      return _categories;
    }

    /// <summary>
    /// Return tru of the menu is visible or not.
    /// </summary>
    /// <param name="control"></param>
    /// <returns>false if we have no valid message selected.</returns>
    public bool IsMultipleItemsMenuVisible(Office.IRibbonControl control)
    {
      // if we have no engine, then we have a problem somehwere.
      if (null == _engine)
      {
        return false;
      }

      // if we have no categories then something is 'broken'
      // so we do not want our menu to show.
      if (null == GetAllCategories())
      {
        return false;
      }

      //  if we have a valid item, then we don't return null.
      return (GetMailItemFromRibbonControl(control) != null);
    }

    /// <summary>
    /// Return tru of the menu is visible or not.
    /// </summary>
    /// <param name="control"></param>
    /// <returns>false if we have no valid message selected.</returns>
    public bool IsMenuVisible(Office.IRibbonControl control)
    {
      // if we have no engine, then we have a problem somehwere.
      if (null == _engine)
      {
        return false;
      }

      // if we have no categories then something is 'broken'
      // so we do not want our menu to show.
      if (null == GetAllCategories() )
      {
        return false;
      }

      //  if we have a valid item, then we don't return null.
      return (GetMailItemFromRibbonControl(control) != null);
    }

    /// <summary>
    /// Display the main menu label.
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public string GetMainLabel(Office.IRibbonControl control)
    {
      return "Myoddweb.Classifier";
    }

    /// <summary>
    /// Display the main menu label.
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public string GetMultipleItemsLabel(Office.IRibbonControl control)
    {
      return "Myoddweb.Classifier";
    }
  }
}
