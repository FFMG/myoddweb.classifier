using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier.core
{
  public class Categories
  {
    public class CategorizeResponse
    {
      public int CategoryId { get; set; }

      public bool WasMagnetUsed { get; set; }
    }

    /// <summary>
    /// Sorted list of categories.
    /// </summary>
    private List<Category> ListOfCategories { get; set; }

    /// <summary>
    /// Get the number of items in the category
    /// </summary>
    public int Count => ListOfCategories?.Count ?? 0;
    
    /// <summary>
    /// The engine that does the classification.
    /// </summary>
    private readonly Engine _engine;

    /// <summary>
    /// Unique identitider to all messages that will contain our unique key.
    /// </summary>
    private const string IdentifierKey = "Classifier.Identifier";

    public Categories( Engine engine )
    {
      // the engine.
      _engine = engine;

      // (re)load the categories
      ReloadCategories();
    }

    /// <summary>
    /// Reload all the categories from the list of categories.
    /// </summary>
    public void ReloadCategories()
    {
      //  we don't seem to have a valid engine
      if (null == _engine)
      {
        return;
      }

      // reload the categories.
      var categories = _engine.GetCategories();

      // use a temp list to update what we have.
      var listOfCategories = new List<Category>();

      // add all the items.
      foreach( var category in categories )
      {
        var folderId = GetFolderId(category.Value);

        //  we cast to uint as we know the value is not -1
        listOfCategories.Add( new Category(category.Value, (uint)category.Key, folderId));
      }

      // we now need to sort it and save it here...
      ListOfCategories = listOfCategories.OrderBy(c => c.Name).ToList();
    }

    public static string GetConfigName(string categoryName)
    {
      return $"category.folder.{categoryName}";
    }

    private string GetFolderId(string categoryName)
    {
      try
      {
        // get the config name
        var configName = GetConfigName( categoryName );

        // and now ger the value.
        return _engine.GetConfig(configName);
      }
      catch (KeyNotFoundException)
      {
        return "";
      }
    }

    /// <summary>
    /// Clasiffy a list of string with a unique id and a list of string.
    /// </summary>
    /// <param name="uniqueEntryId">The unique entry id</param>
    /// <param name="listOfItems">The list of strings we want to classify.</param>
    /// <param name="categoryId">The category we are classifying to.</param>
    /// <param name="weight">The category weight to use.</param>
    /// <returns>myoddweb.classifier.Errors the result of the operation</returns>
    private async Task<Errors> ClassifyAsync(string uniqueEntryId, IEnumerable<string> listOfItems, uint categoryId, uint weight)
    {
      return await Task.FromResult(Classify(uniqueEntryId, listOfItems, categoryId, weight)).ConfigureAwait(false);
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
    /// Given the mail item we try and get the email address of the sender.
    /// </summary>
    /// <param name="mail">the mail item that has the address</param>
    /// <returns>MailAddress or null if it does not exist.</returns>
    public static MailAddress GetSmtpMailAddressForSender(Outlook._MailItem mail)
    {
      try
      {
        string address = GetSmtpAddressForSender( mail);
        if( null == address )
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

      const string PR_SMTP_ADDRESS = @"http://schemas.microsoft.com/mapi/proptag/0x39FE001E";
      return sender.PropertyAccessor.GetProperty( PR_SMTP_ADDRESS) as string;
    }

    /// <summary>
    /// Get the email address of recepients.
    /// @see https://msdn.microsoft.com/en-us/library/office/ff868695.aspx
    /// </summary>
    /// <param name="mail"></param>
    public static List<string> GetSmtpAddressForRecipients(Outlook._MailItem mail)
    {
      const string PR_SMTP_ADDRESS = "http://schemas.microsoft.com/mapi/proptag/0x39FE001E";
      var recips = mail.Recipients;

      return (from Outlook.Recipient recip in recips select recip.PropertyAccessor.GetProperty(PR_SMTP_ADDRESS) as string).ToList();
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
      foreach( var address in addresses )
      {
        try
        {
          mailAddresses.Add(new MailAddress(address));
        }
        catch( FormatException )
        {
          // ignore invalid formats
        }
      }
      return mailAddresses;
    }

    /// <summary>
    /// Given a mail item, we try and build an array of strings.
    /// </summary>
    /// <param name="mailItem">The mail item that has the information we are after.</param>
    /// <returns>List<string> list of items</returns>
    private static List<string> GetStringFromMailItem(Outlook._MailItem mailItem)
    {
      if (null == mailItem)
      {
        // @todo we should never get this far.
        return new List<string>();
      }

      if ( !Categories.IsUsableClassNameForClassification(mailItem?.MessageClass) )
      {
        // @todo we should never get this far.
        return new List<string>();
      }

      var mailItems = new List<string>()
      {
        mailItem.BCC,
        mailItem.To,
        GetSmtpMailAddressForSender( mailItem )?.Address,
        mailItem.SenderName,
        mailItem.CC,
        mailItem.Subject,
        mailItem.Body
      };

      // add all the recepients.
      mailItems.AddRange(GetSmtpAddressForRecipients(mailItem));

      //  add the body of the email.
      switch ( mailItem.BodyFormat )
      {
      case Outlook.OlBodyFormat.olFormatHTML:
        mailItems.Add(mailItem.HTMLBody);
        break;

      case Outlook.OlBodyFormat.olFormatRichText:
        var byteArray = mailItem.RTFBody as byte[];
        if(byteArray != null )
        { 
          var convertedRtf = new System.Text.ASCIIEncoding().GetString(byteArray);
          mailItems.Add(convertedRtf);
        }
        break;

      case Outlook.OlBodyFormat.olFormatUnspecified:
      case Outlook.OlBodyFormat.olFormatPlain:
      default:
        mailItems.Add(mailItem.Body);
        break;
      }

      //  done
      return mailItems;
    }

    /// <summary>
    /// Try and classify a mail assyncroniously.
    /// </summary>
    /// <param name="mailItem">The mail we want to classify.</param>
    /// <param name="id">the category we are setting it to.</param>
    /// <param name="weight">The classification weight we will be using.</param>
    /// <returns></returns>
    public async Task<Errors> ClassifyAsync(Outlook._MailItem mailItem, uint id, uint weight )
    {
      return await ClassifyAsync( GetUniqueIdentifierString( mailItem ),
                                  GetStringFromMailItem( mailItem ),
                                  id,
                                  weight ).ConfigureAwait(false);
    }

    /// <summary>
    /// Try and categorise an email.
    /// </summary>
    /// <param name="mailItem">The mail item we are working with</param>
    /// <param name="magnetWasUsed">If we used a magnet or not</param>
    /// <returns></returns>
    public async Task<CategorizeResponse> CategorizeAsync(Outlook._MailItem mailItem )
    {
      var magnetWasUsed = false;
      var categoryId = await Task.FromResult(Categorize(mailItem, out magnetWasUsed)).ConfigureAwait(false);
      return new CategorizeResponse { CategoryId = categoryId, WasMagnetUsed = magnetWasUsed};
    }

    /// <summary>
    /// Try and categorise an email.
    /// </summary>
    /// <param name="mailItem">The mail item we are working with</param>
    /// <param name="magnetWasUsed">If we used a magnet or not</param>
    /// <returns></returns>
    protected int Categorize(Outlook._MailItem mailItem, out bool magnetWasUsed )
    {
      //  try and use a magnet if we can
      var magnetCategory = CategorizeUsingMagnets(mailItem);

      // set if we used a magnet or not.
      magnetWasUsed = (magnetCategory != -1);

      // otherwise, use the engine direclty.
      return magnetWasUsed ? magnetCategory : _engine.Categorize(GetStringFromMailItem(mailItem));
    }

    /// <summary>
    /// Try and use a magnet to short-circuit the classification.
    /// </summary>
    /// <param name="mailItem"></param>
    /// <returns></returns>
    private int CategorizeUsingMagnets(Outlook._MailItem mailItem)
    {
      // we need to get the magnets and see if any one of them actually applies to us.
      var magnets = _engine.GetMagnets();

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
              catch (FormatException)
              {
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
                if (string.Compare(address.Host ?? "", text, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                  // we have a match for this email address.
                  return magnet.Category;
                }
              }
              catch( FormatException )
              {
              }
            }
            break;

          default:
            _engine.LogEventError( $"Unknown magnet rule : {magnet.Rule}." );
            break;
        }
      }

      // if we are here, we did not find any magnets.
      return -1;
    }

    /// <summary>
    /// Clasiffy a list of string with a unique id and a list of string.
    /// </summary>
    /// <param name="uniqueEntryId">The unique entry id</param>
    /// <param name="listOfItems">The list of strings we want to classify.</param>
    /// <param name="categoryId">The category we are classifying to.</param>
    /// <param name="weight">The category weight to use.</param>
    /// <returns>myoddweb.classifier.Errors the result of the operation</returns>
    private Errors Classify(string uniqueEntryId, IEnumerable<string> listOfItems, uint categoryId, uint weight)
    {
      if (_engine == null)
      {
        return Errors.CategoryNoEngine;
      }

      // does this id even exists?
      var category = ListOfCategories.Find( c => c.Id == categoryId );
      if(category == null )
      {
        return Errors.CategoryNotFound;
      }

      // make one big string out of it.
      var contents = string.Join( " ", listOfItems );

      // classify it.
      if ( !_engine.Train(category.Name, contents, uniqueEntryId, (int)weight ) )
      {
        //  did not work.
        return Errors.CategoryTrainning;
      }

      // this worked, the item was added/classified.
      return Errors.Success;
    }

    /// <summary>
    /// Get all the folders that we will be using as a flat list.
    /// </summary>
    /// <returns>List<Folder> the outlook folders</returns>
    public List<Folder> GetFolders()
    {
      return _engine.GetFolders();
    }

    public Folder FindFolderById(string folderId)
    {
      if (string.IsNullOrEmpty(folderId))
      {
        return null;
      }

      var folders = GetFolders();
      return folders.FirstOrDefault(e => e.Id() == folderId);
    }

    public Category FindCategoryById( int categoryId )
    {
      if( -1 == categoryId )
      {
        return null;
      }

      // find the fist item in the list that will match.
      return ListOfCategories.FirstOrDefault(e => e.Id == categoryId);
    }

    public Folder FindFolderByCategoryId(int categoryId)
    {
      // get the category.
      var category = FindCategoryById(categoryId);

      // find the fist item in the list that will match.
      return FindFolderById( category?.FolderId );
    }

    /// <summary>
    /// Get the category for the document id.
    /// </summary>
    /// <param name="mailItem">the mail item we are looking for.</param>
    /// <returns>Category|null</returns>
    public Category GetCategoryFromMailItem(Outlook._MailItem mailItem )
    {
      // get the unique identifier
      var uniqueIdentifier = GetUniqueIdentifierString(mailItem);

      // then look for it in the engine.
      return FindCategoryById( _engine.GetCategoryFromUniqueId(uniqueIdentifier) );
    }

    /// <summary>
    /// Get the sorted list of categories.
    /// </summary>
    /// <returns>IEnumerable the sorted list.</returns>
    public List<Category> List()
    {
      return ListOfCategories;
    }

    /// <summary>
    /// Given a mail item class name, we check if this is one we could classify.
    /// </summary>
    /// <param name="className">The classname we are checking</param>
    /// <returns>boolean if we can/could classify this mail item or not.</returns>
    static public bool IsUsableClassNameForClassification(string className)
    {
      switch ( className )
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
    
  }
}
