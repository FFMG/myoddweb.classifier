using System;
using System.Windows.Forms;
using myoddweb.classifier.core;
using System.Collections.Generic;
using System.Linq;
using myoddweb.classifier.interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier
{
  public partial class CategoryForm : Form
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
    /// The outlook folder.
    /// </summary>
    public IFolders _folders;

    private Category GivenCategory { get; set; }

    public CategoryForm( ICategories categories, IConfig config, IFolders folders, Category category  )
    {
      // 
      InitializeComponent();

      // the categories
      _categories = categories;

      // the config
      _config = config;

      // the outlook root project.
      _folders = folders;

      // the category we working with.
      GivenCategory = category;
    }

    private void CategoryForm_Load(object sender, EventArgs e)
    {
      // set the text
      textName.Text = GivenCategory == null ? "" : GivenCategory.Name;

      // rebuild the combo
      RebuildFoldersCombo();
    }

    private void RebuildFoldersCombo()
    { 
      // remove everything
      comboBoxFolders.Items.Clear();

      // the text and value.
      comboBoxFolders.DisplayMember = "Text";
      comboBoxFolders.ValueMember = "Value";

      // and set the folder values.
      var items = new List<object>();

      // select the first item
      var selectedIndex = 0;

      // we can have is 'n/a' if we want no folders
      items.Add(new { Text = "n/a", Value = "" });

      // go around all the folders.
      foreach (var folder in _folders.GetFolders() )
      {
        // is that our current one?
        if (GivenCategory?.FolderId == folder.Id())
        {
          selectedIndex = items.Count;
        }
        items.Add( new {Text = folder.Path( true ), Value = folder.Id() });
      }

      // the data source.
      comboBoxFolders.DataSource = items;

      // do we have any folders?
      if (_folders.GetFolders().Count() == 0)
      {
        // there is nothing to select here, nothing much we can do really.
        // so we select the first item, (the 'n/a' one)
        comboBoxFolders.SelectedIndex = 0;
        comboBoxFolders.Enabled = false;
      }
      else
      {
        // select our current one.
        comboBoxFolders.SelectedIndex = selectedIndex;
      }
    }

    private void Cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void Apply_Click(object sender, EventArgs e)
    {
      var allCategories = _categories.GetCategories();

      // get the text and clean it up
      var text = textName.Text;
      text = text.Trim();

      // is it empty?
      if (text.Length == 0)
      {
        MessageBox.Show("The category name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
        return;
      }

      // does it already exist?
      var item = allCategories.Where(c => string.Equals(c.Value, text, StringComparison.CurrentCultureIgnoreCase))
                              .Select( f => new { Key = f.Key, Value = f.Value })
                              .FirstOrDefault();
      if ( item != null )
      {
        if(GivenCategory == null          // Given category is null, no we are trying to create a duplicate.
           ||
          (item.Key != GivenCategory.Id ) // the new string matches an existing string.
          )
        {
          MessageBox.Show("The category name given already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
      }

      // Get the selected folder id.
      // We want to allow the user to select nothing, maybe they just want to 
      // classify the post and do nothing else with it.
      var folderId = GetSelectedFolderId();

      // did we change anything at all?
      if (GivenCategory != null && GivenCategory == new Category(text, GivenCategory.Id, folderId))
      {
        //  just ignore this.
        DialogResult = DialogResult.Ignore;
        Close();
        return;
      }

      //  we are updating a value
      if (GivenCategory != null)
      {
        //  change the name of the category.
        _categories.RenameCategory(GivenCategory.Name, text);
      }
      else
      {
        // add the category
        _categories.GetCategory(text);
      }

      // save the id of the folder.
      _config.SetConfig(CategoriesCollection.GetConfigName(text), folderId);

      // and we are dome
      DialogResult = DialogResult.OK;
      Close();
    }

    private string GetSelectedFolderId()
    {
      // did we select anything at all?
      if (comboBoxFolders.SelectedIndex == -1)
      {
        return "";
      }

      // otherwise get the id
      return ((string) comboBoxFolders.SelectedValue).Trim();
    }
  }
}
