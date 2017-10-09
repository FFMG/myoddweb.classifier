using System;
using System.Collections;
using System.Collections.Generic;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier.core
{
  public class UnProcessedFolders
  {
    private readonly MailProcessor _mailprocessor;

    private readonly IEngine _engine;

    public UnProcessedFolders( IEngine engine, MailProcessor mailprocessor )
    {
      if (engine == null)
      {
        throw new ArgumentNullException(nameof(engine));
      }
      if (mailprocessor == null)
      {
        throw new ArgumentNullException(nameof(mailprocessor));
      }

      _engine = engine;
      _mailprocessor = mailprocessor;
    }

    /// <summary>
    /// Process all the un-processed emails in the given folder.
    /// </summary>
    /// <param name="folders"></param>
    public void Process(Outlook._Folders folders)
    {
      // get the last time we processed an email and create a filter for it.
      var lastProccessed = _mailprocessor.LastProcessed;
      var filter = $"[ReceivedTime]>'{lastProccessed.ToString("g")}'";

      // then parse all the folders.
      var ids = GetUnprocessedEmailsInFolders( folders, filter);
      _mailprocessor.Add(ids);
    }

    /// <summary>
    /// Look into all the folders for unprocessed emails.
    /// </summary>
    /// <param name="folders"></param>
    /// <param name="restrictFolder"></param>
    /// <returns></returns>
    private List<string> GetUnprocessedEmailsInFolders(IEnumerable folders, string restrictFolder)
    {
      var ids = new List<string>();
      try
      {
        // do we have any folders? should never be null...
        if (folders == null)
        {
          return ids;
        }

        foreach (Outlook.MAPIFolder folder in folders)
        {
          ids.AddRange( GetUnprocessedEmailsInFolder(folder, restrictFolder) );
        }
      }
      catch (Exception e)
      {
        _engine.LogError($"There was an exception looking at unprocessed folders : {e}");
      }
      return ids;
    }

    /// <summary>
    /// Get a list of all the unprocess email ids
    /// We don't return the emails as anotehr thread could 'change' those values.
    /// </summary>
    /// <param name="folder">The folder we are working in</param>
    /// <param name="restrictFolder">the filter we want to use to look in the folder.</param>
    /// <returns></returns>
    private IEnumerable<string> GetUnprocessedEmailsInFolder(Outlook.MAPIFolder folder, string restrictFolder)
    {
      // do the sub folders.
      var ids = GetUnprocessedEmailsInFolders(folder.Folders, restrictFolder);

      // is it a mail folder?
      if (folder.DefaultItemType != Outlook.OlItemType.olMailItem)
      {
        return ids;
      }

      var restrictedItems = folder.Items.Restrict(restrictFolder);
      foreach (var item in restrictedItems)
      {
        // get the mail item
        var mailItem = item as Outlook._MailItem;
        if (mailItem == null)
        {
          continue;
        }

        try
        {
          // add this to the mail processor...
          ids.Add(mailItem.EntryID);

          _engine.LogInformation($"Found unprocessed email...{mailItem.Subject}.");
        }
        catch (Exception e)
        {
          _engine.LogError($"There was an exception looking at unprocessed folder : {e}");
        }
      }
      return ids;
    }
  }
}
