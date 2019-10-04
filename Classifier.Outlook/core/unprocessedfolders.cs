using myoddweb.classifier.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier.core
{
  public class UnProcessedFolders
  {
    /// <summary>
    /// What will be processing the emails themselves.
    /// </summary>
    private readonly IMailProcessor _mailprocessor;

    /// <summary>
    /// Used to log messages.
    /// </summary>
    private readonly ILogger _logger;

    public UnProcessedFolders( IMailProcessor mailprocessor, ILogger logger )
    {
      if (logger == null)
      {
        throw new ArgumentNullException(nameof(logger));
      }
      if (mailprocessor == null)
      {
        throw new ArgumentNullException(nameof(mailprocessor));
      }

      _logger = logger;
      _mailprocessor = mailprocessor;
    }

    /// <summary>
    /// Process all the un-processed emails in the given folder.
    /// </summary>
    /// <param name="folders"></param>
    /// <param name="limit"></param>
    /// <param name="includeSubfolders"></param>
    public Task ProcessAsync(Outlook._Folders folders, int limit, bool includeSubfolders)
    {
      // then parse all the folders.
      return Task.Run(() => Process(folders, limit, includeSubfolders));
    }

    /// <summary>
    /// Process all the un-processed emails in the given folder.
    /// </summary>
    /// <param name="folders"></param>
    /// <param name="limit"></param>
    /// <param name="includeSubfolders"></param>
    private void Process(IEnumerable folders, int limit, bool includeSubfolders)
    {
      // get the last time we processed an email and create a filter for it.
      var lastProccessed = _mailprocessor.LastProcessed;
      var filter = $"[ReceivedTime]>'{lastProccessed:g}'";

      // log the filter.
      _logger.LogVerbose( $"Looking for messages received after : '{lastProccessed:g}'" );

      // then parse all the folders.
      var ids = GetUnprocessedEmailsInFolders( folders, limit, includeSubfolders, filter);
      _mailprocessor.Add(ids);
    }

    /// <summary>
    /// Process all the un-processed emails in the given folder.
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="limit"></param>
    /// <param name="includeSubfolders"></param>
    public Task ProcessAsync(Outlook.MAPIFolder folder, int limit, bool includeSubfolders )
    {
      // then parse this folder.
      return Task.Run( () => Process(folder, limit, includeSubfolders ));
    }

    /// <summary>
    /// Process all the un-processed emails in the given folder.
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="limit"></param>
    /// <param name="includeSubfolders"></param>
    private void Process(Outlook.MAPIFolder folder, int limit, bool includeSubfolders)
    {
      // then parse all the folders.
      var ids = GetUnprocessedEmailsInFolder(folder, limit, includeSubfolders, "");
      _mailprocessor.Add(ids);
    }

    /// <summary>
    /// Look into all the folders for unprocessed emails.
    /// </summary>
    /// <param name="folders"></param>
    /// <param name="limit"></param>
    /// <param name="includeSubfolders"></param>
    /// <param name="restrictFolder"></param>
    /// <returns></returns>
    private IList<string> GetUnprocessedEmailsInFolders(IEnumerable folders, int limit, bool includeSubfolders, string restrictFolder)
    {
      var ids = new List<string>();
      try
      {
        // do we have any folders? should never be null...
        if (folders == null)
        {
          return ids;
        }

        if (ids.Count >= limit)
        {
          return ids;
        }

        foreach (Outlook.MAPIFolder folder in folders)
        {
          var subLimit = limit - ids.Count;
          ids.AddRange( GetUnprocessedEmailsInFolder(folder, subLimit, includeSubfolders, restrictFolder) );
          if (ids.Count >= limit)
          {
            break;
          }
        }
      }
      catch (Exception e)
      {
        _logger.LogError("There was an exception looking at unprocessed folders");
        _logger.LogException(e);
      }
      return ids;
    }

    /// <summary>
    /// Get a list of all the unprocess email ids
    /// We don't return the emails as anotehr thread could 'change' those values.
    /// </summary>
    /// <param name="folder">The folder we are working in</param>
    /// <param name="limit"></param>
    /// <param name="includeSubfolders"></param>
    /// <param name="restrictFolder">the filter we want to use to look in the folder.</param>
    /// <returns></returns>
    private IList<string> GetUnprocessedEmailsInFolder(Outlook.MAPIFolder folder, int limit, bool includeSubfolders, string restrictFolder)
    {
      // do the sub folders.
      var ids = new List<string>();
      if (includeSubfolders)
      {
        ids.AddRange( GetUnprocessedEmailsInFolders(folder.Folders, limit, true, restrictFolder) );
      }
      
      // is it a mail folder?
      if (folder.DefaultItemType != Outlook.OlItemType.olMailItem)
      {
        return ids;
      }

      var restrictedItems = string.IsNullOrWhiteSpace(restrictFolder) ? folder.Items : folder.Items.Restrict(restrictFolder);
      foreach (var item in restrictedItems)
      {
        // get the mail item
        if (!(item is Outlook._MailItem mailItem))
        {
          continue;
        }

        try
        {
          // add this to the mail processor...
          ids.Add(mailItem.EntryID);
          _logger.LogInformation($"Found unprocessed email...{mailItem.Subject}.");

          if (ids.Count >= limit)
          {
            break;
          }
        }
        catch (Exception e)
        {
          _logger.LogError( "There was an exception looking at unprocessed folder");
          _logger.LogException(e);
        }
      }
      return ids;
    }
  }
}
