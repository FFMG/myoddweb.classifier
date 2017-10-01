using System.Collections.Generic;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier.core
{
  internal class UnProcessedFolders
  {
    private readonly MailProcessor _mailprocessor;

    private readonly Engine _engine;

    public UnProcessedFolders( Engine engine, MailProcessor mailprocessor )
    {
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

    private List<string> GetUnprocessedEmailsInFolders(Outlook._Folders folders, string restrictFolder)
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
      catch (System.Exception e)
      {
        _engine.LogError($"There was an exception looking at unprocessed folders : {e}");
      }
      return ids;
    }

    private List<string> GetUnprocessedEmailsInFolder(Outlook.MAPIFolder folder, string restrictFolder)
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
        if (mailItem != null)
        {
          try
          {
            // add this to the mail processor...
            ids.Add(mailItem.EntryID);

            _engine.LogInformation($"Found unprocessed email...{mailItem.Subject}.");
          }
          catch (System.Exception e)
          {
            _engine.LogError($"There was an exception looking at unprocessed folder : {e}");
          }
        }
      }
      return ids;
    }
  }
}
