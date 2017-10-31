using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using myoddweb.classifier.interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier.core
{
  public class ItemMove : IDisposable
  {
    /// <summary>
    /// The folders we are currently 'observing'
    /// </summary>
    private Dictionary<string, Outlook.Folder> _observedFolders = new Dictionary<string, Outlook.Folder>();

    /// <summary>
    /// The current mail processor.
    /// </summary>
    private readonly IMailProcessor _mailProcessor;

    /// <summary>
    /// The logger
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// All the otpions.
    /// </summary>
    private readonly IOptions _options;

    /// <summary>
    /// All the available categories.
    /// </summary>
    private readonly ICategories _categories;

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="categories"></param>
    /// <param name="mailProcessor">The mail processor</param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public ItemMove( ICategories categories, IMailProcessor mailProcessor, IOptions options, ILogger logger )
    {
      if (categories == null)
      {
        throw new ArgumentNullException(nameof(categories));
      }
      if (mailProcessor == null)
      {
        throw new ArgumentNullException(nameof(mailProcessor));
      }
      if (options == null)
      {
        throw new ArgumentNullException(nameof(options));
      }
      if (logger == null)
      {
        throw new ArgumentNullException(nameof(logger));
      }
      _mailProcessor = mailProcessor;
      _logger = logger;
      _categories = categories;
      _options = options;
    }

    public void Monitor( Outlook._Folders folders)
    {
      //  remove whatever we might be monitoring
      UnRegisterAllFolders();

      // then start monitoring
      RegisterAllFolders( folders );
    }

    public void Dispose()
    {
      // unregister everything.
      UnRegisterAllFolders();
    }

    /// <summary>
    /// Unregister all the folders been observed.
    /// </summary>
    private void UnRegisterAllFolders()
    {
      foreach (var folder in _observedFolders)
      {
        folder.Value.BeforeItemMove -= BeforeItemMove;
      }

      // reset everything
      _observedFolders = new Dictionary<string, Outlook.Folder>();
    }

    /// <summary>
    /// Register all the folders.
    /// </summary>
    /// <param name="folders"></param>
    private void RegisterAllFolders(IEnumerable folders)
    {
      foreach (var item in folders)
      {
        RegisterFolder(item as Outlook.Folder);
      }
    }

    /// <summary>
    /// Register a single folder for actions.
    /// </summary>
    /// <param name="folder"></param>
    private void RegisterFolder(Outlook.Folder folder)
    {
      if (null == folder)
      {
        return;
      }

      // remember to remove the new functions
      // @see UnRegisterAllFolders(...)
      _observedFolders[folder.EntryID] = folder;
      folder.BeforeItemMove += BeforeItemMove;

      // register all the sub folders of that folder.
      RegisterAllFolders(folder.Folders);
    }

    private void BeforeItemMove(object item, Outlook.MAPIFolder moveto, ref bool cancel)
    {
      try
      {
        var mailItem = (Outlook._MailItem)item;
        if (mailItem == null)
        {
          return;
        }

        // get the item id
        var itemId = mailItem.EntryID;

        // are we currently processing this item?
        if (_mailProcessor.IsProccessing(itemId))
        {
          return;
        }

        // is it something we even care about
        if (!_mailProcessor.IsUsableClassNameForClassification(mailItem.MessageClass))
        {
          return;
        }

        // is it permanently deleted?
        if (moveto == null)
        {
          _logger.LogInformation($"Mail '{mailItem.Subject}' was permanently deleted.");
          return;
        }

        // do we want to use the message for training?
        if (!_options.ReAutomaticallyTrainMoveMessages)
        {
          // log that we found nothing.
          _logger.LogInformation($"Mail '{mailItem.Subject}' was manually moved to folder '{moveto.Name}' but the option is set not to use this for training.");

          // done
          return;
        }

        // get the new folder id
        var folderId = moveto.EntryID;

        // and guess where we might be wanting to go.
        var posibleCategories = _categories.FindCategoriesByFolderId(folderId).ToList();
        if (!posibleCategories.Any())
        {
          // log that we found nothing.
          _logger.LogInformation($"Mail '{mailItem.Subject}' was manually moved to folder '{moveto.Name}' but I found no matching categories to classify it with.");

          // if we cannot guess a valid category
          // then there is nothing we can do.
          return;
        }

        // log it
        _logger.LogVerbose($"About to move mail '{mailItem.Subject}' to folder '{moveto.Name}' and I found {posibleCategories.Count} posible categories.");

        // we found one or more posible, valid categories.
        // if we have more than one, then we have to ask the user to pick.
        // but if we only have one, then no need to ask anything always select the one.
        if (posibleCategories.Count == 1)
        {
          // get the one item we selected.
          var category = posibleCategories.First();

          // we know this is a user selected item
          // so we can get the weight from the options.
          TasksController.Add( _mailProcessor.ClassifyAsync(itemId, category.Id, _options.UserWeight));

          // log it
          _logger.LogInformation($"Mail '{mailItem.Subject}' was manually moved to folder '{moveto.Name}' and classified as '{category.Name}'");
        }
      }
      catch (Exception e)
      {
        _logger.LogException(e);
      }
    }
  }
}
