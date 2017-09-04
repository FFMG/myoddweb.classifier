using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Classifier.Interfaces;
using myoddweb.classifier.core;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier.forms
{
  public partial class MagnetMailItemForm : Form
  {
    public class ComboboxCategoryItem
    {
      public string Text { get; set; }
      public Category Item { get; set; }
      public override string ToString() { return Text; }
    }

    public class ComboboxRuleItem
    {
      public string Text { get; set; }
      public RuleTypes Type { get; set; }
      public string MagnetText { get; set; }
      public override string ToString() { return Text; }
    }

    /// <summary>
    /// The main classifier engine.
    /// </summary>
    private readonly Engine _engine;

    /// <summary>
    /// The current email we are checking.
    /// </summary>
    private readonly Outlook._MailItem _mailItem;

    public MagnetMailItemForm( Engine engine, Outlook._MailItem mailItem )
    {
      // 
      InitializeComponent();

      // the engine to create new categories.
      _engine = engine;

      // the mail item
      _mailItem = mailItem;
    }

    private void MagnetMailItemForm_Load(object sender, EventArgs e)
    {
      // rebuild the combos.
      RebuildCombos();
    }

    private void RebuildCombos()
    {
      // the categories
      RebuildCategoriesCombo();

      // the combos
      RebuildRulesCombos();
    }

    /// <summary>
    /// Given the current mail item we try and guess what the category/folder might be.
    /// </summary>
    /// <returns>number the posible category id or -1 if we don't know.</returns>
    private int GuessCategoryFromMailItem()
    {
      foreach (var category in _engine.Categories.List() )
      {
        // is that our current one?
        if (category?.FolderId == ((Outlook.Folder)_mailItem?.Parent).EntryID)
        {
          return (int)category.Id;
        }
      }

      // don't know
      return -1;
    }

    /// <summary>
    /// List all the categories.
    /// </summary>
    private void RebuildCategoriesCombo()
    { 
      // remove everything
      comboBoxCategories.Items.Clear();

      // and set the folder values.
      var items = new List<ComboboxCategoryItem>();

      // select the first item
      var selectedIndex = 0;

      // guess what the category could be
      var guessedCategory = GuessCategoryFromMailItem();

      // go around all the folders.
      foreach (var category in _engine.Categories.List() )
      {
        // is that our current one?
        if (category?.Id == guessedCategory)
        {
          selectedIndex = items.Count;
        }
        items.Add(new ComboboxCategoryItem { Text = category.Name, Item = category });
      }

      // the data source.
      comboBoxCategories.DataSource = items;

      //  select the first item
      comboBoxCategories.SelectedIndex = 0;

      // do we have any folders?
      if (_engine.Categories.Count == 0)
      {
        // there is nothing to select here, nothing much we can do really.
        // so we select the first item, (the 'n/a' one)
        comboBoxCategories.Enabled = false;
        comboBoxCategories.SelectedIndex = 0;
      }
      else
      {
        // select our current one.
        comboBoxCategories.SelectedIndex = selectedIndex;
      }
    }

    /// <summary>
    /// Rebuild all the magnets rules for each email addresses.
    /// </summary>
    private void RebuildRulesCombos()
    {
      // remove everything
      comboMagnetAndRules.Items.Clear();

      // and set the folder values.
      var items = new List<ComboboxRuleItem>();

      // keep track of the emails and/or domains
      var itemsAlreadyAdded = new List<string>();

      // the 'from' rules.
      var address = Categories.GetSmtpMailAddressForSender( _mailItem );
      if (address != null)
      {
        // add them to the combo.
        items.Add(new ComboboxRuleItem { Text = $"From address: '{address.Address}'", Type = RuleTypes.FromEmail, MagnetText = address.Address.ToLower() });
        items.Add(new ComboboxRuleItem { Text = $"From host: '@{address.Host}'", Type = RuleTypes.FromEmailHost, MagnetText = address.Host.ToLower() });

        // add those 2 to the list of 'done' items.
        itemsAlreadyAdded.Add( address.Address.ToLower());
        itemsAlreadyAdded.Add( address.Host.ToLower());
      }

      // the 'to' rules.
      var toAddresses = Categories.GetSmtpMailAddressForRecipients(_mailItem);
      foreach( var toAddress in toAddresses )
      {
        // check if this address already exists.
        var lowerCaseToAddress = toAddress.Address.ToLower();
        if( !itemsAlreadyAdded.Select( c => c == lowerCaseToAddress).Any() )
        { 
          items.Add(new ComboboxRuleItem { Text = $"To host: '{toAddress.Address}'", Type = RuleTypes.ToEmail, MagnetText = lowerCaseToAddress });
          itemsAlreadyAdded.Add(lowerCaseToAddress);
        }

        // check if this host already exists.
        var lowerCaseToAddressHost = toAddress.Address.ToLower();
        if (!itemsAlreadyAdded.Select( c => c == lowerCaseToAddressHost).Any() )
        {
          items.Add(new ComboboxRuleItem { Text = $"To host: '@{toAddress.Host}'", Type = RuleTypes.ToEmailHost, MagnetText = lowerCaseToAddressHost });
          itemsAlreadyAdded.Add(toAddress.Host.ToLower());
        }
      }

      // the data source.
      comboMagnetAndRules.DataSource = items;

      //  select the first item
      comboMagnetAndRules.SelectedIndex = 0;

      // do we have any folders?
      if (_engine.Categories.Count == 0)
      {
        // there is nothing to select here, nothing much we can do really.
        // so we select the first item, (the 'n/a' one)
        comboMagnetAndRules.Enabled = false;
      }
    }

    private void Cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void Apply_Click(object sender, EventArgs e)
    {
      // Get the selected rules item.
      var ruleItem = GetSelectedRuleItem();

      // if we have no rule item, we must move on.
      if( null == ruleItem )
      {
        MessageBox.Show("Please select a valid rule/magnet to apply.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // get the text and clean it up
      var magnetText = ruleItem.MagnetText;

      // is it empty?
      if (magnetText.Length == 0)
      {
        MessageBox.Show("The magnet name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Get the selected rule id.
      var ruleType = ruleItem.Type;

      // Get the selected rule id.
      var categoryId = GetSelectedCategoryId();

      // all the magnets.
      var allMagnets = _engine.GetMagnets();

      // does it already exist?
      var currentMagnet = allMagnets.FirstOrDefault(m => string.Equals(m.Name, magnetText, StringComparison.CurrentCultureIgnoreCase));
      if ( currentMagnet != null )
      {
        // the given magnet seem to exist already
        // we just need to update it now.
        _engine.LogEventInformation( $"Updating magnet {currentMagnet.Id}/{currentMagnet.Name}" );

        // try and do the update
        if (!_engine.UpdateMagnet(currentMagnet, magnetText, (int) ruleType, categoryId))
        {
          // there was a problem updating the data.
          _engine.LogEventError($"Unable to update magnet {currentMagnet.Id}/{currentMagnet.Name}");

          // display a message.
          MessageBox.Show("I was unable to update the given magnet!", "Magnets update", MessageBoxButtons.OK, MessageBoxIcon.Error);

          // try again?
          return;
        }

        // success
        MessageBox.Show("The magnet was successfuly updated!", "Magnets updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // everything seems to have worked, so we are done.
        DialogResult = DialogResult.OK;
        Close();

        // done
        return;
      }

      // create the magnet 
      var newMagnetId = _engine.CreateMagnet(magnetText, (int) ruleType, categoryId);
      if( -1 == newMagnetId )
      {
        // success
        _engine.LogEventError($"Unable to create a new magnet : {magnetText}.");

        // display a message.
        MessageBox.Show("I was unable to create the given magnet!", "Create magnets", MessageBoxButtons.OK, MessageBoxIcon.Error);

        // try again?
        return;
      }

      // log that it worked.
      _engine.LogEventInformation( $"Created a new magnet : {newMagnetId}/{magnetText}." );

      // success
      MessageBox.Show("The magnet was successfuly created!", "Magnets created", MessageBoxButtons.OK, MessageBoxIcon.Information);

      // and we are dome
      DialogResult = DialogResult.OK;
      Close();
    }

    private int GetSelectedCategoryId()
    {
      // did we select anything at all?
      if (comboBoxCategories.SelectedIndex == -1)
      {
        return -1;
      }

      // otherwise get the id
      return (int)((ComboboxCategoryItem)comboBoxCategories.SelectedItem).Item.Id;
    }

    private ComboboxRuleItem GetSelectedRuleItem()
    {
      // did we select anything at all?
      if (comboMagnetAndRules.SelectedIndex == -1)
      {
        return null;
      }

      // otherwise get the id
      return ((ComboboxRuleItem)comboMagnetAndRules.SelectedItem);
    }
  }
}
