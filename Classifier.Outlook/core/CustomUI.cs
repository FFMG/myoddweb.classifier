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
using myoddweb.classifier.interfaces;
using myoddweb.classifier.utils;

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
    /// The engine that does the classification.
    /// </summary>
    private IEngine _engine;

    /// <summary>
    /// The mail processor
    /// </summary>
    private MailProcessor _mailProcessor;

    private Office.IRibbonUI _ribbon;

    public CustomUI()
    {
    }

    public void SetMailProcessor(MailProcessor mailProcessor)
    {
      if (mailProcessor == null)
      {
        throw new ArgumentNullException(nameof(mailProcessor));
      }
      _mailProcessor = mailProcessor;
    }

    public void SetEngine(IEngine engine)
    {
      if (engine == null)
      {
        throw new ArgumentNullException(nameof(engine));
      }
      _engine = engine;
    }

    #region IRibbonExtensibility Members

    public string GetCustomUI(string ribbonId)
    {
      return GetResourceText("myoddweb.classifier.Core.CustomUI.xml");
    }

    #endregion

    #region Ribbon Callbacks
    //Create callback methods here. For more information about adding callback methods, visit http://go.microsoft.com/fwlink/?LinkID=271226

    public void Ribbon_Load(Office.IRibbonUI ribbonUI)
    {
      this._ribbon = ribbonUI;
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

    private _MailItem GetMailItemFromRibbonControl(Office.IRibbonControl control)
    {
      var items = GetMultipleMailItemFromRibbonControl( control );
      return items?.First();
    }

    private List<_MailItem> GetMultipleMailItemFromRibbonControl(Office.IRibbonControl control)
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
          if( !_mailProcessor.IsUsableClassNameForClassification(item.MessageClass) )
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
    public async Task OnSelectCategory(Office.IRibbonControl control)
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
        MessageBox.Show(@"There was a problem updating the category, is the datbase locked?", @"Could not classify", MessageBoxButtons.OK, MessageBoxIcon.Error );
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
          var watch = StopWatch.Start(_engine.Logger);

          // log a message to indicate when we are trying to do.
          Debug.WriteLine($"Classifying message id {mailItem.EntryID} to category {categoryId}");

          // we have all the information we need
          // we can now categorize the mail.
          if ( await ClassifyMailAsync(mailItem, categoryId).ConfigureAwait(false) )
          {
            // stop the timer and say how long it took...
            watch.Stop( "  [Success] Classifying took {0}." );
            continue;
          }

          // debug
          Debug.WriteLine($"There was a problem setting the mail for message id {mailItem.EntryID}");

          //  how long this took.
          watch.Stop("  [Failed] Classifying took {0}.");

          // log the error
          _engine.Logger.LogError($"There was a problem setting the mail for message id {mailItem.EntryID} ('{mailItem.Subject}').");

          // no need to go further, something broke.
          return false;
        }
        catch
        {
          // log that this did not work.
          _engine.Logger.LogError($"I was unable to categorise mail {mailItem.EntryID} ('{mailItem.Subject}').");

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
    private async Task<bool> ClassifyMailAsync( _MailItem mailItem, uint id )
    {
      // we know this is a user selected item
      // so we can get the weight from the options.
      var entryIdItem = mailItem.EntryID;
      return await _mailProcessor.ClassifyAsync(entryIdItem, id, _engine.Options.UserWeight).ConfigureAwait(false);
    }

    /// <summary>
    /// Show the various options and allow the user to change certain settings. 
    /// </summary>
    /// <param name="control"></param>
    public void OnManageMore(Office.IRibbonControl control)
    {
      if (null == _engine.Options )
      {
        return;
      }

      using (var optionsForm = new OptionsForm( _engine, _engine.Categories ))
      {
        optionsForm.StartPosition = FormStartPosition.CenterScreen;
        optionsForm.ShowDialog( );
      }
    }

    public void OnDetails(Office.IRibbonControl control)
    {
      var items = GetMultipleMailItemFromRibbonControl(control);
      if (items == null || items.Count == 0 || items.Count > 1)
      {
        // we can only show the details of one email
        return;
      }

      // get the one and only item
      var mailItem = items.First();

      // show the displays
      var categoryList = _mailProcessor.GetStringFromMailItem(mailItem, _engine.Logger);
      var text = string.Join(";", categoryList.Select(x => x.Value));
      using (var detailsForm = new DetailsForm(_engine.Classify, _engine.Categories, text ))
      {
        detailsForm.ShowDialog();
      }
    }

    public void OnMagnet(Office.IRibbonControl control)
    {
      var items = GetMultipleMailItemFromRibbonControl(control);
      if (items == null || items.Count == 0 || items.Count > 1)
      {
        // we can only do the magnet of one email
        return;
      }

      // get the one and only item
      var mailItem = items.First();

      // update the magnets list.
      using (var magnetMailItemForm = new MagnetMailItemForm( _engine.Logger, _engine.Magnets, _engine.Categories, mailItem ))
      {
        magnetMailItemForm.ShowDialog();
      }
    }

    public string GetMultipleItemsContent(Office.IRibbonControl control)
    {
      // do we have a valid mail item?
      // if not then we are not going to display it.
      var mailItem = GetMailItemFromRibbonControl(control);
      return null == mailItem ? "" : GetContentWithPosibleMailItem(null).Result;
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
      return null == mailItem ? "" : GetContentWithPosibleMailItem( mailItem ).Result;
    }

    /// <summary>
    /// Guess the posible new category id 
    /// </summary>
    /// <param name="mailItem">the mail item itself.</param>
    /// <param name="categories">the categories tool we will use to re-categorise</param>
    /// <param name="currentCategoryId">The current value of the category</param>
    /// <returns>Categories.CategorizeResponse the new id or -1 if we don't know.</returns>
    protected async Task<MailProcessor.CategorizeResponse> GuessPosibleCategory( _MailItem mailItem, int currentCategoryId, IEnumerable<Category> categories )
    {
      var guessCategoryResponse = new MailProcessor.CategorizeResponse
      {
        CategoryId = 0,
        WasMagnetUsed = false
      };

      if ( !_engine.Options.ReCheckIfCtrlKeyIsDown || (Control.ModifierKeys & Keys.Control) != Keys.Control)
      {
        // if we do not want to check options, then we don't want to do that.
        if ( !_engine.Options.ReCheckCategories )
        {
          return guessCategoryResponse;
        }

        // if we currently have a category and we only want to check the
        // unknown categories, then we musn't check.
        if (currentCategoryId != -1 && _engine.Options.CheckIfUnKnownCategory)
        {
          return guessCategoryResponse;
        }
      }

      var currentCursor = Cursor.Current;
      Cursor.Current = Cursors.WaitCursor;

      // start the wath
      var watch = StopWatch.Start(_engine.Logger);

      try
      {
        // guess where it could be going to now.
        if (mailItem != null)
        {
          guessCategoryResponse = await _mailProcessor.CategorizeAsync(mailItem).ConfigureAwait(false);
        }
      }
      catch (Exception e)
      {
        _engine.Logger.LogError(e.ToString());

        // @todo we need to log that there was an issue.
        guessCategoryResponse = new MailProcessor.CategorizeResponse
        {
          CategoryId = 0,
          WasMagnetUsed = false
        };
      }

      // reset the cursor.
      Cursor.Current = currentCursor;

      // add a debug message.
      watch.Stop(guessCategoryResponse.CategoryId != currentCategoryId
        ? $"My new classifying guess for this message is : {guessCategoryResponse.CategoryId} (in {{0}})"
        : $"My classifying guess for this message is category remains the same : {guessCategoryResponse.CategoryId} (in {{0}})");

      // return what we found
      return guessCategoryResponse;
    }

    /// <summary>
    /// GetContent callback
    /// </summary>
    /// <param name="mailItem">The mail item.</param>
    /// <returns></returns>
    public async Task<string> GetContentWithPosibleMailItem( _MailItem mailItem )
    { 
      // create the menu xml
      var translationsXml = new StringBuilder(@"<menu xmlns=""http://schemas.microsoft.com/office/2009/07/customui"">");

      // if we have no categories then something is 'broken'
      // so we do not want our menu to show.
      if ( _engine.Categories.Count == 0 )
      {
        return "";
      }

      // do we know the current category?
      var currentCategory = mailItem == null ? null : _mailProcessor.GetCategoryFromMailItem(mailItem);

      // get the current category if?
      var currentCategoryId = currentCategory == null ? -1 : (int)currentCategory.Id;

      // try and guess the new category
      var guessCategoryResponse = await GuessPosibleCategory(mailItem, currentCategoryId, _engine.Categories.List).ConfigureAwait(false);

      // and create a menu for all of them.
      foreach (var category in _engine.Categories.List )
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
      if (_engine.Categories.Count > 0 )
      {
        translationsXml.Append(@"<menuSeparator id=""separator"" />");
      }

      // show the email details
      // but only if we have selected one email.
      if (mailItem != null)
      {
        translationsXml.Append(
          $@"<button id =""{"myoddweb.classifier_details"}"" label=""{"Details ..."}"" onAction=""{"OnDetails"}"" />");
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
      if (_engine.Categories.Count == 0 )
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
      if (_engine.Categories.Count == 0  )
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
