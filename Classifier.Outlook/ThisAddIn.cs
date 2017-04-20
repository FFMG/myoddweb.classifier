using System.Diagnostics;
using myoddweb.classifier.core;
using myoddweb.viewer.utils;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier
{
  public partial class ThisAddIn
  {
    /// <summary>
    /// The engine that does the classification.
    /// </summary>
    private Engine _engine;

    /// <summary>
    /// all the categories.
    /// </summary>
    private Categories _categories;

    /// <summary>
    /// All the options
    /// </summary>
    private Options _options;

    private Outlook.Explorers _explorers;

    private Engine TheEngine => _engine ?? (_engine = new Engine());

    private Categories TheCategories => _categories ?? (_categories = new Categories(TheEngine));

    private Options TheOptions => _options ?? (_options = new Options(TheEngine));

    private void ThisAddIn_Startup(object sender, System.EventArgs e)
    {
      // tell the engine what the folders are.
      TheEngine.SetRootFolder(Application.Session.DefaultStore.GetRootFolder() );

      // get the explorers.
      _explorers = this.Application.Explorers;

      // new email arrives.
      Application.NewMailEx += Application_NewMailEx;
    }

    private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
    {
      // Note: Outlook no longer raises this event. If you have code that 
      //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
    }

    /// <summary>
    /// Create a new menu item to handle inbox items. 
    /// </summary>
    /// <returns>CustomUI</returns>
    protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
    {
      return new CustomUI(TheEngine, TheCategories, TheOptions);
    }

    #region VSTO generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InternalStartup()
    {
      this.Startup += new System.EventHandler(ThisAddIn_Startup);
      this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
    }

    #endregion

    private async void Application_NewMailEx(string entryIdItem)
    {
      Outlook.MailItem newMail;
      try
      {
        // new email has arrived, we need to try and classify it.
        newMail =
          _explorers.Application.Session.GetItemFromID(entryIdItem, System.Reflection.Missing.Value) as Outlook.MailItem;
      }
      catch (System.Runtime.InteropServices.COMException)
      {
        // Could not find that message anymore
        // @todo log this entry id could not be located.
        return;
      }

      if (newMail == null)
      {
        return;
      }

      // the message note.
      if( !Categories.IsUsableClassNameForClassification(newMail?.MessageClass) )
      {
        return;
      }

      // start the watch
      var watch = StopWatch.Start();

      // look for the category
      var guessCategoryResponse = await TheCategories.CategorizeAsync(newMail).ConfigureAwait(false);

      // 
      var categoryId = guessCategoryResponse.CategoryId;
      var wasMagnetUsed = guessCategoryResponse.WasMagnetUsed;

      // did we find a category?
      if (-1 == categoryId)
      {
        watch.Checkpoint("I could not classify the new message into any categories: (in: {0})");
        return;
      }

      // 
      watch.Checkpoint( $"I classified the new message category : {categoryId} (in: {{0}})");

      //
      // Do we want to train this
      if( _options.ReAutomaticallyTrainMessages || (wasMagnetUsed && _options.ReAutomaticallyTrainMagnetMessages ))
      {
        // get the weight
        var weight = (wasMagnetUsed ? _options.MagnetsWeight : 1);

        // we can now classify it.
        await TheCategories.ClassifyAsync(newMail, (uint) categoryId, weight ).ConfigureAwait(false);
      }

      // get the posible folder.
      var folder = TheCategories.FindFolderByCategoryId(categoryId);
      if (null == folder)
      {
        //  the user does not want to move to another folder.
        return;
      }

      try
      {
        // don't move it if we don't need to.
        var currentFolder = (Outlook.Folder) newMail.Parent;

        // if they are not the same, we can move it.
        if (currentFolder.EntryID != folder.OutlookFolder.EntryID)
        {
          // if this is an ignored conversation, we will not move it.
          if (!IsIgnored(newMail))
          {
            // try and move 
            newMail.Move(folder.OutlookFolder);
          }
        }
      }
      catch (System.Exception ex)
      {
        watch.Checkpoint( $"Could not move : {newMail.Subject}, {ex.StackTrace} {{0}}");
      }
    }

    private bool IsIgnored(Outlook._MailItem mailItem)
    {
      // does the folder allow conversations?
      var folder = mailItem.Parent as Outlook.Folder;
      var store = folder?.Store;
      if (store?.IsConversationEnabled != true)
      {
        return false;
      }

      // get that conversation
      var conv = mailItem.GetConversation();
      if (conv == null)
      {
        return false;
      }

      // 
      return false;
    }
  }
}
