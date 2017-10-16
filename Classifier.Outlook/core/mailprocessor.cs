﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myoddweb.viewer.utils;
using myoddweb.classifier.utils;
using System.Linq;
using System.Threading;
using Outlook = Microsoft.Office.Interop.Outlook;
using myoddweb.classifier.interfaces;
using System.Net.Mail;

namespace myoddweb.classifier.core
{
  public class MailProcessor
  {
    /// <summary>
    /// Unique identitider to all messages that will contain our unique key.
    /// </summary>
    private const string IdentifierKey = "Classifier.Identifier";

    public class CategorizeResponse
    {
      public int CategoryId { get; set; }

      public bool WasMagnetUsed { get; set; }
    }

    private const string ConfigName = "Processor.LastEmail";

    /// <summary>
    /// Our timer
    /// </summary>
    private System.Timers.Timer _ticker;

    /// <summary>
    /// The engine that will help us to process the mail.
    /// </summary>
    private readonly IEngine _engine;

    /// <summary>
    /// The current session
    /// </summary>
    private readonly Outlook._NameSpace _session;

    /// <summary>
    /// The mail items we want to move and the categories we are moving them to.
    /// </summary>
    private List<string> _mailItems;

    /// <summary>
    /// Our lock...
    /// </summary>
    private ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

    /// <summary>
    /// Get the last time we processed an email
    /// Or the current time we we don't have a valid value.
    /// </summary>
    public DateTime LastProcessed {
      get
      {
        Lock.EnterReadLock();
        try
        {
          var last = Convert.ToInt32(_engine.Config.GetConfigWithDefault(ConfigName, "-1"));
          if (-1 == last)
          {
            return DateTime.Now;
          }
          return Helpers.UnixToDateTime(last);
        }
        finally
        {
          Lock.ExitReadLock();
        }
      }
    }

    public MailProcessor(IEngine engine, Outlook._NameSpace session )
    {
      // can't be null
      if (engine == null)
      {
        throw new ArgumentNullException(nameof(engine));
      }

      if (session == null)
      {
        throw new ArgumentNullException(nameof(session));
      }

      _engine = engine;
      _session = session;
      _mailItems = new List<string>();
    }

    /// <summary>
    /// Add a mail item to be moved to the folder.
    /// </summary>
    /// <param name="entryIdItem">The item we are moving.</param>
    public void Add(string entryIdItem)
    {
      Add(new List<string> { entryIdItem });
    }

    /// <summary>
    /// Add a range of mail entry ids to our list.
    /// </summary>
    /// <param name="ids"></param>
    public void Add(List<string> ids)
    {
      Lock.EnterWriteLock();
      try
      {
        AddInLock(ids);
      }
      finally
      {
        Lock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Add a range of mail entry ids to our list.
    /// We are inside a lock.
    /// </summary>
    /// <param name="ids"></param>
    private void AddInLock(List<string> ids)
    { 
      //  anything to do?
      if ( !ids.Any() )
      {
        return;
      }

      //  add this item to our list.
      _mailItems.AddRange(ids);

      //  if the delay is set to 0 then do everything now.
      if (0 == _engine.Options.ClassifyDelaySeconds)
      {
        HandleAllItemsInLock().Wait();
      }
      else
      {
        // start the timer
        StartTimerInLock();
      }
    }

    /// <summary>
    /// Start a timer inside a lock.
    /// </summary>
    private void StartTimerInLock()
    {
      //  stop it
      StopTimerInLock();

      // recreate the timer, we cannot use a value of 0 in the timer. 
      _ticker = new System.Timers.Timer(0 == _engine.Options.ClassifyDelaySeconds ? 1 : _engine.Options.ClassifyDelayMilliseconds);
      _ticker.Elapsed += async (sender, e) => await HandleTimer();
      _ticker.AutoReset = true;
      _ticker.Enabled = true;
    }
    
    /// <summary>
    /// Stop the timers while we are inside a lock
    /// </summary>
    private void StopTimerInLock()
    {
      if (_ticker == null)
      {
        return;
      }

      _ticker.Enabled = false;
      _ticker.Stop();
      _ticker.Dispose();
      _ticker = null;
    }

    private async Task<bool> HandleTimer()
    {
      Lock.EnterWriteLock();
      try
      {
        if( _ticker == null )
        {
          return false;
        }
        return await HandleTimerInLock().ConfigureAwait( false );
      }
      finally
      {
        Lock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Fired when the timer even it called.
    /// </summary>
    /// <returns></returns>
    private async Task<bool> HandleTimerInLock()
    {
      // we are inside the lock
      // so we can stop the timer
      StopTimerInLock();

      // handle all the mail items
      await HandleAllItemsInLock().ConfigureAwait( false );

      // we are done,
      return true;
    }

    /// <summary>
    /// Handle all the items that are in the queue.
    /// </summary>
    /// <returns></returns>
    private async Task HandleAllItemsInLock()
    {
      // wait for all the tasks now.
      await Task.WhenAll(_mailItems.Select(HandleItemInLock).Cast<Task>().ToArray());

      // clear the list.
      _mailItems = new List<string>();
    }

    /// <summary>
    /// Update the last actually process email.
    /// </summary>
    /// <param name="mailItem"></param>
    private void UpdateLastProcessedEmailInLock(Outlook._MailItem mailItem)
    {
      // the current time
      var when = Helpers.DateTimeToUnix(mailItem.ReceivedTime);

      // get the last processed time
      var last = Convert.ToInt32(_engine.Config.GetConfigWithDefault(ConfigName, "0"));

      // did we handle something older?
      if (last > when)
      {
        return;
      }
      _engine.Config.SetConfig(ConfigName, Convert.ToString(when));
    }

    /// <summary>
    /// Handle the email
    /// </summary>
    /// <param name="entryIdItem">The email we want to handle.</param>
    /// <returns></returns>
    private async Task<bool> HandleItemInLock( string entryIdItem)
    {
      Outlook._MailItem mailItem = null;
      try
      {
        // new email has arrived, we need to try and classify it.
        mailItem = _session.GetItemFromID(entryIdItem, System.Reflection.Missing.Value) as Outlook._MailItem;
      }
      catch (System.Runtime.InteropServices.COMException e)
      {
        _engine.Logger.LogError(e.ToString());
      }

      if (mailItem == null)
      {
        _engine.Logger.LogWarning($"Could not locate mail item {entryIdItem} to move.");
        return false;
      }

      // did we send this email?
      if( MailWasSentByUs(mailItem))
      {
        _engine.Logger.LogWarning($"Mail item {entryIdItem} was sent by us and will not be classified.");
        return false;
      }

      // either way, this is a valid 'processed' email
      UpdateLastProcessedEmailInLock(mailItem);

      // the message note.
      if (!IsUsableClassNameForClassification(mailItem.MessageClass))
      {
        return false;
      }

      // start the watch
      var watch = StopWatch.Start(_engine.Logger);

      // look for the category
      var guessCategoryResponse = await CategorizeAsync(mailItem).ConfigureAwait(false);

      // 
      var categoryId = guessCategoryResponse.CategoryId;
      var wasMagnetUsed = guessCategoryResponse.WasMagnetUsed;

      // did we find a category?
      if (-1 == categoryId)
      {
        _engine.Logger.LogVerbose($"I could not classify the new message {entryIdItem} into any categories. ('{mailItem.Subject}')");
        watch.Checkpoint("I could not classify the new message into any categories: (in: {0})");
        return false;
      }

      // 
      watch.Checkpoint($"I classified the new message category : {categoryId} (in: {{0}})");

      //
      // Do we want to train this
      var options = _engine.Options;
      if (options.ReAutomaticallyTrainMessages || (wasMagnetUsed && options.ReAutomaticallyTrainMagnetMessages))
      {
        // get the weight
        var weight = (wasMagnetUsed ? options.MagnetsWeight : 1);

        // we can now classify it.
        await ClassifyAsync(mailItem, (uint)categoryId, weight).ConfigureAwait(false);
      }

      try
      {
        // get the posible folder.
        var folder = _engine.Categories.FindFolderByCategoryId(categoryId);
        if (null == folder)
        {
          // 
          _engine.Logger.LogWarning($"Could not locate folder for category {categoryId}, cannot move item.");

          //  the user does not want to move to another folder.
          return false;
        }

        // don't move it if we don't need to.
        var currentFolder = (Outlook.MAPIFolder)mailItem.Parent;
 
        // if they are not the same, we can move it.
        if (currentFolder.EntryID == folder.Id() )
        {
          _engine.Logger.LogVerbose($"No need to move mail, '{mailItem.Subject}', to folder, '{folder.Name()}', already in folder");
          return true;
        }

        // if this is an ignored conversation, we will not move it.
        if (IsIgnoredConversation(mailItem))
        {
          _engine.Logger.LogVerbose($"Mail, '{mailItem.Subject}' is part of an ignored conversation and will not be moved." );
          return true;
        }

        // this is where we want to move to.
        var itemToFolder = (folder as OutlookFolder)?.Folder;
        if( null == itemToFolder )
        {
          throw new Exception($"The folder {folder.Name()} does not seem to be of type 'Folder' and cannot be used.");
        }

        // try and move 
        mailItem.Move(itemToFolder);

        // and log it.
        _engine.Logger.LogVerbose($"Moved mail, '{mailItem.Subject}', to folder, '{folder.Name()}'");
      }
      catch (Exception ex)
      {
        _engine.Logger.LogError(ex.ToString());
        return false;
      }
      return true;
    }

    /// <summary>
    /// Check if a certain email was sent by us.
    /// </summary>
    /// <param name="mailItem">The mail item we are looking at.</param>
    /// <returns></returns>
    private bool MailWasSentByUs(Outlook._MailItem mailItem)
    {
      // if the received by name is null, then it means we did not receive it
      // and if we did not receive it then we must have sent it...
      return mailItem?.ReceivedByName == null;
    }

    /// <summary>
    /// Check if a mail thread is ignored or not
    /// If it is ignored then there is nothing for us to do with it.
    /// Normally another rule kicks in and deletes it.
    /// </summary>
    /// <param name="mailItem"></param>
    /// <returns></returns>
    private bool IsIgnoredConversation(Outlook._MailItem mailItem)
    {
      // does the folder allow conversations?
      var folder = mailItem.Parent as Outlook.MAPIFolder;
      var store = folder?.Store;
      if (store?.IsConversationEnabled != true)
      {
        return false;
      }

      // get that conversation
      Outlook._Conversation conv = mailItem.GetConversation();
      if (conv == null)
      {
        return false;
      }

      // get the delete rule.
      var delete = conv.GetAlwaysDelete(_session.DefaultStore);
      if( Outlook.OlAlwaysDeleteConversation.olAlwaysDelete == delete )
      {
        return true;
      }

      // 
      return false;
    }

    /// <summary>
    /// Try and categorise an email.
    /// </summary>
    /// <param name="mailItem">The mail item we are working with</param>
    /// <returns></returns>
    public async Task<CategorizeResponse> CategorizeAsync(Outlook._MailItem mailItem)
    {
      bool magnetWasUsed;
      var categoryId = await Task.FromResult(Categorize(mailItem, out magnetWasUsed)).ConfigureAwait(false);
      return new CategorizeResponse { CategoryId = categoryId, WasMagnetUsed = magnetWasUsed };
    }

    /// <summary>
    /// Try and categorise an email.
    /// </summary>
    /// <param name="mailItem">The mail item we are working with</param>
    /// <param name="magnetWasUsed">If we used a magnet or not</param>
    /// <returns></returns>
    private int Categorize(Outlook._MailItem mailItem, out bool magnetWasUsed)
    {
      //  try and use a magnet if we can
      var magnetCategory = CategorizeUsingMagnets(mailItem);

      // set if we used a magnet or not.
      magnetWasUsed = (magnetCategory != -1);

      // otherwise, use the engine direclty.
      return magnetWasUsed ? magnetCategory : _engine.Classify.Categorize(GetStringFromMailItem(mailItem));
    }

    /// <summary>
    /// Try and use a magnet to short-circuit the classification.
    /// </summary>
    /// <param name="mailItem"></param>
    /// <returns></returns>
    private int CategorizeUsingMagnets(Outlook._MailItem mailItem)
    {
      // we need to get the magnets and see if any one of them actually applies to us.
      var magnets = _engine.Magnets.GetMagnets();

      // the email address of the sender.
      string fromEmailAddress = null;

      // the email addresses of the recepients.
      List<string> toEmailAddresses = null;

      // going around all our magnets.
      foreach (var magnet in magnets)
      {
        // get the magnet text
        var text = magnet.Name;

        // do we actually have a magnet?
        if (string.IsNullOrEmpty(text))
        {
          continue;
        }

        // what is that rule for?
        switch ((RuleTypes)magnet.Rule)
        {
          case RuleTypes.FromEmail:
            fromEmailAddress = fromEmailAddress ?? GetSmtpAddressForSender(mailItem);
            if (string.Compare(fromEmailAddress ?? "", text, StringComparison.CurrentCultureIgnoreCase) == 0)
            {
              // we have a match for this email address.
              return magnet.Category;
            }
            break;

          case RuleTypes.FromEmailHost:
            fromEmailAddress = fromEmailAddress ?? GetSmtpAddressForSender(mailItem);
            if (fromEmailAddress != null)
            {
              try
              {
                var address = new MailAddress(fromEmailAddress);
                if (string.Compare(address.Host, text, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                  // we have a match for this email address.
                  return magnet.Category;
                }
              }
              catch (FormatException e)
              {
                _engine.Logger.LogError(e.ToString());
              }
            }
            break;

          case RuleTypes.ToEmail:
            toEmailAddresses = toEmailAddresses ?? GetSmtpAddressForRecipients(mailItem);
            foreach (var toEmailAddress in toEmailAddresses)
            {
              if (string.Compare(toEmailAddress ?? "", text, StringComparison.CurrentCultureIgnoreCase) == 0)
              {
                // we have a match for this email address.
                return magnet.Category;
              }
            }
            break;

          case RuleTypes.ToEmailHost:
            toEmailAddresses = toEmailAddresses ?? GetSmtpAddressForRecipients(mailItem);
            foreach (var toEmailAddress in toEmailAddresses)
            {
              try
              {
                var address = new MailAddress(toEmailAddress);
                if (string.Compare(address.Host, text, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                  // we have a match for this email address.
                  return magnet.Category;
                }
              }
              catch (FormatException e)
              {
                _engine.Logger.LogError(e.ToString());
              }
            }
            break;

          default:
            _engine.Logger.LogError($"Unknown magnet rule : {magnet.Rule}.");
            break;
        }
      }

      // if we are here, we did not find any magnets.
      return -1;
    }

    /// <summary>
    /// Get the email address of recepients.
    /// @see https://msdn.microsoft.com/en-us/library/office/ff868695.aspx
    /// </summary>
    /// <param name="mail"></param>
    private static List<string> GetSmtpAddressForRecipients(Outlook._MailItem mail)
    {
      const string prSmtpAddress = "http://schemas.microsoft.com/mapi/proptag/0x39FE001E";
      var recips = mail.Recipients;

      return (from Outlook.Recipient recip in recips select recip.PropertyAccessor.GetProperty(prSmtpAddress) as string).ToList();
    }


    /// <summary>
    /// Given the mail item we try and get the email address of the sender.
    /// </summary>
    /// <param name="mail"></param>
    /// <returns>string or null if the address does not exist.</returns>
    public static string GetSmtpAddressForSender(Outlook._MailItem mail)
    {
      if (mail == null)
      {
        throw new ArgumentNullException();
      }

      if (mail.SenderEmailType != "EX")
      {
        return mail.SenderEmailAddress;
      }

      var sender = mail.Sender;
      if (sender == null)
      {
        return null;
      }

      //Now we have an AddressEntry representing the Sender
      if (sender.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry
          || sender.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeRemoteUserAddressEntry)
      {
        //Use the ExchangeUser object PrimarySMTPAddress
        var exchUser = sender.GetExchangeUser();
        if (exchUser != null)
        {
          return exchUser.PrimarySmtpAddress;
        }
      }

      const string prSmtpAddress = @"http://schemas.microsoft.com/mapi/proptag/0x39FE001E";
      return sender.PropertyAccessor.GetProperty(prSmtpAddress) as string;
    }

    /// <summary>
    /// Given a mail item, we try and build an array of strings.
    /// </summary>
    /// <param name="mailItem">The mail item that has the information we are after.</param>
    /// <returns>List list of items</returns>
    public static Dictionary<MailStringCategories, string> GetStringFromMailItem(Outlook._MailItem mailItem)
    {
      if (null == mailItem)
      {
        // @todo we should never get this far.
        return new Dictionary<MailStringCategories, string>();
      }

      if (!IsUsableClassNameForClassification(mailItem.MessageClass))
      {
        // @todo we should never get this far.
        return new Dictionary<MailStringCategories, string>();
      }

      var mailItems = new Dictionary<MailStringCategories, string>
      {
        {MailStringCategories.Bcc, mailItem.BCC},
        {MailStringCategories.To, mailItem.To},
        {MailStringCategories.Address, GetSmtpMailAddressForSender(mailItem)?.Address},
        {MailStringCategories.SenderName, mailItem.SenderName},
        {MailStringCategories.Cc, mailItem.CC},
        {MailStringCategories.Subject, mailItem.Subject},
        {MailStringCategories.Smtp, string.Join(" ", GetSmtpAddressForRecipients(mailItem))}
      };

      //  add the body of the email.
      switch (mailItem.BodyFormat)
      {
        case Outlook.OlBodyFormat.olFormatHTML:
          mailItems.Add(MailStringCategories.HtmlBody, mailItem.HTMLBody);
          break;

        case Outlook.OlBodyFormat.olFormatRichText:
          var byteArray = mailItem.RTFBody as byte[];
          if (byteArray != null)
          {
            var convertedRtf = new System.Text.ASCIIEncoding().GetString(byteArray);
            mailItems.Add(MailStringCategories.RtfBody, convertedRtf);
          }
          else
          {
            mailItems.Add(MailStringCategories.Body, mailItem.Body);
          }
          break;

        // case Outlook.OlBodyFormat.olFormatUnspecified:
        // case Outlook.OlBodyFormat.olFormatPlain:
        default:
          mailItems.Add(MailStringCategories.Body, mailItem.Body);
          break;
      }

      //  done
      return mailItems;
    }

    /// <summary>
    /// Given the mail item we try and get the email address of the sender.
    /// </summary>
    /// <param name="mail">the mail item that has the address</param>
    /// <returns>MailAddress or null if it does not exist.</returns>
    public static MailAddress GetSmtpMailAddressForSender(Outlook._MailItem mail)
    {
      try
      {
        string address = GetSmtpAddressForSender(mail);
        if (null == address)
        {
          return null;
        }
        return new MailAddress(address);
      }
      catch (FormatException)
      {
        return null;
      }
    }

    /// <summary>
    /// Given a mail item class name, we check if this is one we could classify.
    /// </summary>
    /// <param name="className">The classname we are checking</param>
    /// <returns>boolean if we can/could classify this mail item or not.</returns>
    static public bool IsUsableClassNameForClassification(string className)
    {
      switch (className)
      {
        //  https://msdn.microsoft.com/en-us/library/ee200767(v=exchg.80).aspx
        case "IPM.Note":
        case "IPM.Note.SMIME.MultipartSigned":
        case "IPM.Note.SMIME":
        case "IPM.Note.Receipt.SMIME":
          return true;
      }

      // no, we cannot use it.
      return false;
    }

    /// <summary>
    /// Try and classify a mail assyncroniously.
    /// </summary>
    /// <param name="mailItem">The mail we want to classify.</param>
    /// <param name="id">the category we are setting it to.</param>
    /// <param name="weight">The classification weight we will be using.</param>
    /// <returns></returns>
    public async Task<Errors> ClassifyAsync(Outlook._MailItem mailItem, uint id, uint weight)
    {
      return await ClassifyAsync(GetUniqueIdentifierString(mailItem),
                                  GetStringFromMailItem(mailItem),
                                  id,
                                  weight).ConfigureAwait(false);
    }

    /// <summary>
    /// Clasiffy a list of string with a unique id and a list of string.
    /// </summary>
    /// <param name="uniqueEntryId">The unique entry id</param>
    /// <param name="listOfItems">The list of strings we want to classify.</param>
    /// <param name="categoryId">The category we are classifying to.</param>
    /// <param name="weight">The category weight to use.</param>
    /// <returns>myoddweb.classifier.Errors the result of the operation</returns>
    private async Task<Errors> ClassifyAsync(string uniqueEntryId, Dictionary<MailStringCategories, string> listOfItems, uint categoryId, uint weight)
    {
      return await Task.FromResult(Classify(uniqueEntryId, listOfItems, categoryId, weight)).ConfigureAwait(false);
    }

    /// <summary>
    /// Clasiffy a list of string with a unique id and a list of string.
    /// </summary>
    /// <param name="uniqueEntryId">The unique entry id</param>
    /// <param name="listOfItems">The list of strings we want to classify.</param>
    /// <param name="categoryId">The category we are classifying to.</param>
    /// <param name="weight">The category weight to use.</param>
    /// <returns>myoddweb.classifier.Errors the result of the operation</returns>
    private Errors Classify(string uniqueEntryId, Dictionary<MailStringCategories, string> listOfItems, uint categoryId, uint weight)
    {
      if ( _engine.Categories == null)
      {
        return Errors.CategoryNoEngine;
      }

      // does this id even exists?
      var category = _engine.Categories.FindCategoryById( (int)categoryId );
      if (category == null)
      {
        return Errors.CategoryNotFound;
      }

      // make one big string out of it.
      var contents = string.Join(";", listOfItems.Select(x => x.Value));

      // classify it.
      if (!_engine.Classify.Train(category.Name, contents, uniqueEntryId, (int)weight))
      {
        //  did not work.
        return Errors.CategoryTrainning;
      }

      // this worked, the item was added/classified.
      return Errors.Success;
    }

    private static string GetUniqueIdentifierString(Outlook._MailItem mailItem)
    {
      // does it already exist?
      if (mailItem.UserProperties[IdentifierKey] == null)
      {
        // no, it does not we need to add it.
        mailItem.UserProperties.Add(IdentifierKey, Outlook.OlUserPropertyType.olText, false, Outlook.OlFormatText.olFormatTextText);

        // set the current entry id, the number itself is immaterial.
        mailItem.UserProperties[IdentifierKey].Value = mailItem.EntryID;
        mailItem.Save();
      }

      // we can now return the value as we know it.
      return mailItem.UserProperties[IdentifierKey].Value;
    }

    /// <summary>
    /// Get the category for the document id.
    /// </summary>
    /// <param name="mailItem">the mail item we are looking for.</param>
    /// <returns>Category|null</returns>
    public Category GetCategoryFromMailItem(Outlook._MailItem mailItem)
    {
      // get the unique identifier
      var uniqueIdentifier = GetUniqueIdentifierString(mailItem);

      // then look for it in the engine.
      return _engine.Categories.FindCategoryById(_engine.Categories.GetCategoryFromUniqueId(uniqueIdentifier));
    }

    /// <summary>
    /// Get all the mail addresses for the recipients.
    /// </summary>
    /// <param name="mail">The mail item</param>
    /// <returns></returns>
    public static List<MailAddress> GetSmtpMailAddressForRecipients(Outlook._MailItem mail)
    {
      var mailAddresses = new List<MailAddress>();
      var addresses = GetSmtpAddressForRecipients(mail);
      foreach (var address in addresses)
      {
        try
        {
          mailAddresses.Add(new MailAddress(address));
        }
        catch (FormatException)
        {
          // ignore invalid formats
        }
      }
      return mailAddresses;
    }
  }
}
