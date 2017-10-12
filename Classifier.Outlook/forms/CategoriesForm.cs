using System;
using System.Windows.Forms;
using myoddweb.classifier.core;
using myoddweb.classifier.interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier.forms
{
  public partial class CategoriesForm : Form
  {
    /// <summary>
    /// The categories.
    /// </summary>
    private readonly ICategories _categories;

    /// <summary>
    /// The categories.
    /// </summary>
    private readonly IConfig _config;

    /// <summary>
    /// The actual folders
    /// </summary>
    private Folders _folders = null;

    /// <summary>
    /// The outlook folder.
    /// </summary>
    public Outlook.MAPIFolder _rootFolder;

    public CategoriesForm(ICategories categories, IConfig config, Outlook.MAPIFolder rootFolder )
    {
      // the categories
      _categories = categories;
      
      // the config
      _config = config;

      // the outlook root project.
      _rootFolder = rootFolder;

      InitializeComponent();
    }

    private void CategoriesForm_Load(object sender, EventArgs e)
    {
      // create the columns
      CreateColumns();

      // load the contents.
      ReloadCategories();
    }

    private void CreateColumns()
    {
      // remove all the columns
      listCategories.Columns.Clear();

      // we cannot click the header
      listCategories.HeaderStyle = ColumnHeaderStyle.Nonclickable;

      // set up the widhts.
      var width = listCategories.Width;
      var widthOfCategories = width / 3;
      var widthOfRules = width - widthOfCategories;

      // 
      listCategories.View = View.Details;
      listCategories.Columns.Add("Category", widthOfCategories, HorizontalAlignment.Left);
      listCategories.Columns.Add("Rule", widthOfRules, HorizontalAlignment.Left);
    }

    private void ReloadCategories()
    {
      // only one item at a time.
      listCategories.MultiSelect = false;

      // remove everything
      listCategories.Items.Clear();

      // get the categories.
      listCategories.BeginUpdate();
      foreach (var category in _categories.Categories.List() )
      {
        var item = new ListViewItem()
        {
          Text = category.Name,
          ToolTipText = category.Name,
          Tag = category,
          SubItems = { GetRuleDescription(category) }
        };
        listCategories.Items.Add(item);
      }
      listCategories.EndUpdate();

      // do we have anything at all?
      if (0 == _categories.Categories.Count)
      {
        DisableButtons();
      }
      else
      {
        // just select the first item in the list.
        listCategories.Items[0].Selected = true;
        listCategories.Select();

        // enable the buttons.
        EnableButtons();
      }
    }

    private string GetRuleDescription(Category category)
    {
      var folder = _categories.Categories.FindFolderById(category.FolderId);
      if (null == folder)
      {
        // no action.
        return "n/a";
      }

      // return the pretty name
      return $"Move to folder {folder.Path(true)}";
    }

    private void New_Click(object sender, EventArgs e)
    {
      // make a new category.
      var category = new CategoryForm(_categories, _config, _rootFolder, null );

      // load the dialog box.
      if (category.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      // reload all the categories.
      _categories.Categories.ReloadCategories();

      // reload everything
      ReloadCategories();
    }

    private void Ok_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void listCategories_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
      if (null == GetSelectedCategory())
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

    private Category GetSelectedCategory()
    {
      if (listCategories.SelectedItems.Count == 0)
      {
        return null;
      }

      // just the first item
      var item = listCategories.SelectedItems[0];

      // get the category.
      return (Category)item.Tag;
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
      var category = new CategoryForm(_categories, _config, _rootFolder, GetSelectedCategory());

      // load the dialog box.
      if (category.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      // reload all the categories.
      _categories.Categories.ReloadCategories();

      // reload everything
      ReloadCategories();
    }

    private void Delete_Click(object sender, EventArgs e)
    {
      // get what we want to delete.
      var category = GetSelectedCategory();
      if( null == category )
      {
        //  how did we get here?
        return;
      }

      // confirmation.
      if( DialogResult.No == MessageBox.Show( $"Are you sure you want to delete '{category.Name}'", 
                                              "Delete Category", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                            )
      {
        return;
      }

      // try and delete the category.
      if (!_categories.DeleteCategory(category.Name))
      {
        MessageBox.Show("There was an error deleting the category.", "Delete Category", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // reload all the categories.
      _categories.Categories.ReloadCategories();

      // reload the values.
      ReloadCategories();
    }

    private void listCategories_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      // get the category.
      var category = GetSelectedCategory();
      if (null == category)
      {
        return;
      }

      // make a new category.
      var categoryForm = new CategoryForm(_categories, _config, _rootFolder, GetSelectedCategory() );

      // load the dialog box.
      if (categoryForm.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      // reload all the categories.
      _categories.Categories.ReloadCategories();

      // reload everything
      ReloadCategories();
    }
  }
}
