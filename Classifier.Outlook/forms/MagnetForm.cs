using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Classifier.Interfaces;
using myoddweb.classifier.core;
using Classifier.Interfaces.Helpers;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.forms
{
  public partial class MagnetForm : Form
  {
    public class ComboboxItem
    {
      public string Text { get; set; }
      public int Value { get; set; }
      public override string ToString() { return Text; }
    }

    private readonly IMagnets _magnets = null;

    private Magnet GivenMagnet { get; set; }

    private readonly CategoriesCollection _Categories;

    public MagnetForm(IMagnets magnets, CategoriesCollection categories, Magnet magnet  )
    {
      // 
      InitializeComponent();

      // the engine to create new categories.
      _magnets = magnets;

      // the category we working with.
      GivenMagnet = magnet;

      // the categories
      _Categories = categories;
    }

    private void MagnetForm_Load(object sender, EventArgs e)
    {
      // set the text
      textName.Text = GivenMagnet == null ? "" : GivenMagnet.Name;

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

    private void RebuildCategoriesCombo()
    { 
      // remove everything
      comboBoxCategories.Items.Clear();

      // and set the folder values.
      var items = new List<ComboboxItem>();

      // select the first item
      var selectedIndex = 0;

      // go around all the folders.
      foreach (var category in _Categories.List() )
      {
        // is that our current one?
        if (GivenMagnet?.Category == category.Id )
        {
          selectedIndex = items.Count;
        }
        items.Add(new ComboboxItem{ Text = category.Name, Value = (int)category.Id });
      }

      // the data source.
      comboBoxCategories.DataSource = items;

      // do we have any folders?
      if (_Categories.Count == 0)
      {
        // there is nothing to select here, nothing much we can do really.
        // so we select the first item, (the 'n/a' one)
        comboBoxCategories.SelectedIndex = 0;
        comboBoxCategories.Enabled = false;
      }
      else
      {
        // select our current one.
        comboBoxCategories.SelectedIndex = selectedIndex;
      }
    }

    private void RebuildRulesCombos()
    {
      // remove everything
      comboBoxRules.Items.Clear();

      // and set the folder values.
      var items = new List<ComboboxItem>();

      // select the first item
      var selectedIndex = 0;

      // go around all the folders.
      var rules = new Rules();
      foreach (var rule in rules )
      {
        // is that our current one?
        if (GivenMagnet?.Rule == rule )
        {
          selectedIndex = items.Count;
        }
        items.Add(new ComboboxItem { Text = rule, Value = rule });
      }

      // the data source.
      comboBoxRules.DataSource = items;

      // do we have any folders?
      if (items.Count == 0)
      {
        // there is nothing to select here, nothing much we can do really.
        // so we select the first item, (the 'n/a' one)
        comboBoxRules.SelectedIndex = 0;
        comboBoxRules.Enabled = false;
      }
      else
      {
        // select our current one.
        comboBoxRules.SelectedIndex = selectedIndex;
      }
    }

    private void Cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void Apply_Click(object sender, EventArgs e)
    {
      // get the text and clean it up
      var text = textName.Text;
      text = text.Trim();

      // is it empty?
      if (text.Length == 0)
      {
        MessageBox.Show("The magnet name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
        return;
      }

      // Get the selected rule id.
      var ruleId = GetSelectedRuleId();

      // Get the selected rule id.
      var categoryId = GetSelectedCategoryId();

      // all the magnets.
      var allMagnets = _magnets.GetMagnets();

      // does it already exist?
      var item = allMagnets.Where( m => string.Equals( m.Name, text, StringComparison.CurrentCultureIgnoreCase) && m.Rule == ruleId )
                              .Select( f => new Magnet(){ Id = f.Id, Name = f.Name, Rule = f.Rule })
                              .FirstOrDefault();
      if ( item != null )
      {
        if(GivenMagnet == null          // Given category is null, no we are trying to create a duplicate.
           ||
          (item.Id != GivenMagnet.Id ) // the new string matches an existing string.
          )
        {
          MessageBox.Show("The magnet name/rule given already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
      }

      // did we change anything at all?
      if (GivenMagnet != null && GivenMagnet == new Magnet()
      {
        Id = GivenMagnet.Id, 
        Name = text,
        Rule = ruleId,
        Category = categoryId
      } )
      {
        //  just ignore this.
        DialogResult = DialogResult.Ignore;
        Close();
        return;
      }

      //  we are updating a value
      if (GivenMagnet != null)
      {
        //  change the name of the magnet.
        _magnets.UpdateMagnet(GivenMagnet.Id, text, ruleId, categoryId);
      }
      else
      {
        // create the magnet 
        _magnets.CreateMagnet(text, ruleId, categoryId);
      }

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
      return ((ComboboxItem)comboBoxCategories.SelectedItem).Value;
    }

    private int GetSelectedRuleId()
    {
      // did we select anything at all?
      if (comboBoxRules.SelectedIndex == -1)
      {
        return -1;
      }

      // otherwise get the id
      return ((ComboboxItem)comboBoxRules.SelectedItem).Value;
    }
  }
}
