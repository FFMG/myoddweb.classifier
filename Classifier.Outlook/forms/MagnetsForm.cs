using System;
using System.Windows.Forms;
using Classifier.Interfaces;
using myoddweb.classifier.core;
using Classifier.Interfaces.Helpers;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.forms
{
  public partial class MagnetsForm : Form
  {
    // the classifier engine
    private readonly IMagnets _magnets;

    // all the categories.
    private readonly Categories _categories;

    public MagnetsForm(Categories categories, IMagnets magnets)
    {
      _magnets = magnets;
      _categories = categories;
      InitializeComponent();
    }

    private void MagnetsForm_Load(object sender, EventArgs e)
    {
      // create the columns
      CreateColumns();

      // load the contents.
      ReloadMagnets();
    }

    private void CreateColumns()
    {
      // remove all the columns
      ListMagnets.Columns.Clear();

      // we cannot click the header
      ListMagnets.HeaderStyle = ColumnHeaderStyle.Nonclickable;

      // set up the widhts.
      var width = ListMagnets.Width;
      var widthOfMagnets = width / 7;
      var widthOfName = widthOfMagnets * 3;
      var widthOfRule = widthOfMagnets * 2;
      var widthOfCategory = widthOfMagnets * 2;

      // 
      ListMagnets.View = View.Details;
      ListMagnets.Columns.Add("Name", widthOfName, HorizontalAlignment.Left);
      ListMagnets.Columns.Add("Rule", widthOfRule, HorizontalAlignment.Left);
      ListMagnets.Columns.Add("Category", widthOfCategory, HorizontalAlignment.Left);
    }

    private void ReloadMagnets()
    {
      // only one item at a time.
      ListMagnets.MultiSelect = false;

      // remove everything
      ListMagnets.Items.Clear();

      // the current list of magnets.
      var magnets = _magnets.GetMagnets();

      // the rules.
      var rules = new Rules();

      // get the categories.
      ListMagnets.BeginUpdate();
      foreach (var magnet in magnets )
      {
        // look for the category
        var category =_categories.FindCategoryById(magnet.Category);

        // look for the rule.
        var rule = rules.Find(magnet.Rule);

        var item = new ListViewItem()
        {
          Text = magnet.Name,
          ToolTipText = magnet.Name,
          Tag = magnet,
          SubItems = { rule, (category == null ? "n/a" : category.Name)  }
        };
        ListMagnets.Items.Add(item);
      }
      ListMagnets.EndUpdate();

      // do we have anything at all?
      if (0 == magnets.Count)
      {
        DisableButtons();
      }
      else
      {
        // just select the first item in the list.
        ListMagnets.Items[0].Selected = true;
        ListMagnets.Select();

        // enable the buttons.
        EnableButtons();
      }
    }

    private void New_Click(object sender, EventArgs e)
    {
      // make a new category.
      var magnet = new MagnetForm(_magnets, _categories, null );

      // load the dialog box.
      if (magnet.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      // reload everything
      ReloadMagnets();
    }

    private void Ok_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void ListMagnets_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
      if (null == GetSelectedMagnet())
      {
        // nothing selected, so we have nothing to click.
        DisableButtons();
      }
      else
      {
        // something selected, so we can click the buttons
        EnableButtons();
      }
    }

    private Magnet GetSelectedMagnet()
    {
      if (ListMagnets.SelectedItems.Count == 0)
      {
        return null;
      }

      // just the first item
      var item = ListMagnets.SelectedItems[0];

      // get the category.
      return (Magnet)item.Tag;
    }

    private void DisableButtons()
    {
      Edit.Enabled = false;
      Delete.Enabled = false;
    }

    private void EnableButtons()
    {
      Edit.Enabled = true;
      Delete.Enabled = true;
    }

    private void Edit_Click(object sender, EventArgs e)
    {
      // make a new category.
      var magnet = new MagnetForm(_magnets, _categories, GetSelectedMagnet());

      // load the dialog box.
      if (magnet.ShowDialog() != DialogResult.OK)
      {
        return;
      }
      
      // reload everything
      ReloadMagnets();
    }

    private void Delete_Click(object sender, EventArgs e)
    {
      // get what we want to delete.
      var magnet = GetSelectedMagnet();
      if( null == magnet )
      {
        //  how did we get here?
        return;
      }

      // confirmation.
      if( DialogResult.No == MessageBox.Show( $"Are you sure you want to delete '{magnet.Name}'", 
                                              "Delete Magnet", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                            )
      {
        return;
      }

      // try and delete the category.
      if (!_magnets.DeleteMagnet(magnet.Id))
      {
        MessageBox.Show("There was an error deleting the magnet.", "Delete Magnet", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // reload the values.
      ReloadMagnets();
    }

    private void ListMagnets_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      // get the category.
      var category = GetSelectedMagnet();
      if (null == category)
      {
        return;
      }

      // make a new category.
      var magnetForm = new MagnetForm(_magnets, _categories, GetSelectedMagnet() );

      // load the dialog box.
      if (magnetForm.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      // reload everything
      ReloadMagnets();
    }
  }
}
