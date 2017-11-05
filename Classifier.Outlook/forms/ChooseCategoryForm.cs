using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using myoddweb.classifier.core;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.forms
{
  public partial class ChooseCategoryForm : Form
  {
    private class ComboboxSeparatorItem
    {
      public override string ToString()
      {
        return "";
      }
    }

    private class ComboboxNoneItem
    {
      public override string ToString()
      {
        return "[None]";
      }
    }

    private class ComboboxCategoryValue
    {
      public ComboboxCategoryValue(Category category)
      {
        if (category == null)
        {
          throw new ArgumentNullException(nameof(category));
        }
        Category = category;
      }

      public Category Category { get; }

      public override string ToString() { return Category.Name; }
    }

    private const int SeparatorHeight = 3, VerticalItemPadding = 4;

    /// <summary>
    /// The categories that we want to show first.
    /// </summary>
    private readonly IEnumerable<Category> _preferedCategories;

    /// <summary>
    /// The categories.
    /// </summary>
    private readonly ICategories _categories;

    /// <summary>
    /// The options.
    /// </summary>
    private readonly IOptions _options;

    /// <summary>
    /// Get the currently selected item
    /// </summary>
    public Category SelectedCategory => (comboBoxCategories.SelectedItem as ComboboxCategoryValue)?.Category;

    /// <summary>
    /// Keep track of the currently selected item
    /// </summary>
    private int _lastSelectedIndex;

    public ChooseCategoryForm( IEnumerable<Category> preferedCategories, ICategories categories, IOptions options )
    {
      // the chosen categories.
      _preferedCategories = preferedCategories;

      // the categories
      _categories = categories;

      // the options
      _options = options;

      InitializeComponent();
    }
    
    private void ChooseCategoryForm_Load(object sender, EventArgs e)
    {
      // the combo box should not be checked by default, (how else would we get here?)
      // but we never know how the user might have goten here.
      checkBoxDontAskAgain.Checked = !_options.ReConfirmMultipleTainingCategory;

      // set the combo box so we can draw our own stuff
      comboBoxCategories.DrawMode = DrawMode.OwnerDrawVariable;
      comboBoxCategories.DrawItem += DrawCustomCategoryItem;
      comboBoxCategories.MeasureItem += MeasureCategoryItem;

      // load the contents.
      ReloadCategories();
    }

    private void MeasureCategoryItem(object sender, MeasureItemEventArgs e)
    {
      if (e.Index == -1)
      {
        return;
      }
      var comboBoxItem = comboBoxCategories.Items[e.Index];

      // in Whidbey consider using TextRenderer.MeasureText instead
      var textSize = e.Graphics.MeasureString(comboBoxItem.ToString(), comboBoxCategories.Font).ToSize();
      e.ItemHeight = textSize.Height + VerticalItemPadding;
      e.ItemWidth = textSize.Width;


      if (comboBoxItem is ComboboxSeparatorItem)
      {
        // one white line, one dark, one white.
        e.ItemHeight += SeparatorHeight;
      }
    }

    private void DrawCustomCategoryItem(object sender, DrawItemEventArgs e)
    {
      if (e.Index == -1)
      {
        return;
      }
      var comboBoxItem = comboBoxCategories.Items[e.Index];
      
      e.DrawBackground();
      e.DrawFocusRectangle();

      var isSeparatorItem = comboBoxItem is ComboboxSeparatorItem;

      using (Brush textBrush = new SolidBrush( (e.Index > _preferedCategories.Count() || e.Index == 0) ? Color.BlueViolet : e.ForeColor ))
      {
        var bounds = e.Bounds;

        // Draw the string vertically centered but on the left
        using (var format = new StringFormat())
        {
          format.LineAlignment = StringAlignment.Center;
          format.Alignment = StringAlignment.Near;
          // in Whidbey consider using TextRenderer.DrawText instead
          if (isSeparatorItem)
          {
            var separatorRect = new Rectangle(e.Bounds.Left, e.Bounds.Bottom - SeparatorHeight, e.Bounds.Width, SeparatorHeight);
            using (Brush b = new SolidBrush(comboBoxCategories.BackColor))
            {
              e.Graphics.FillRectangle(b, e.Bounds);
            }
            e.Graphics.DrawLine(SystemPens.ControlText, separatorRect.Left + 2, separatorRect.Top + 1,
              separatorRect.Right - 2, separatorRect.Top + 1);
          }
          else
          {
            e.Graphics.DrawString(comboBoxItem.ToString(), comboBoxCategories.Font, textBrush, bounds, format);
          }
        }
      }
    }

    private void SelectedIndexChanged(object sender, EventArgs e)
    {
      var comboBox = (ComboBox)sender;

      var isSeparatorItem = comboBox.SelectedItem is ComboboxSeparatorItem;
      if (!isSeparatorItem)
      {
        // update the selection
        _lastSelectedIndex = comboBoxCategories.SelectedIndex;
        return;
      }

      var idx = comboBoxCategories.SelectedIndex;
      if (idx > 0 && _lastSelectedIndex != idx-1)
      {
        comboBoxCategories.SelectedIndex = idx -1;
      }
      else
      {
        comboBoxCategories.SelectedIndex = idx +1;
      }

      // update the selection
      _lastSelectedIndex = comboBoxCategories.SelectedIndex;
    }

    private void btnSelect_Click(object sender, EventArgs e)
    {
      _options.ReConfirmMultipleTainingCategory = !checkBoxDontAskAgain.Checked;
    }

    private void ReloadCategories()
    {
      // don't sort it.
      comboBoxCategories.Sorted = false;

      // make the list read only
      comboBoxCategories.DropDownStyle = ComboBoxStyle.DropDownList;

      // remove everything
      comboBoxCategories.Items.Clear();

      // add a 'none' item first.
      var items = new List<object>
      {
        new ComboboxNoneItem()
      };

      // first add the prefered items
      foreach (var category in _preferedCategories)
      {
        // is that our current one?
        items.Add(new ComboboxCategoryValue(category));
      }

      // add the separator if we have something alreadt
      if (items.Count > 0)
      {
        items.Add(new ComboboxSeparatorItem());
      }

      // then add all the other items afterward.
      foreach (var category in _categories.List )
      {
        // add the other items.
        if (!items.Any(c => (c as ComboboxCategoryValue) != null && ((ComboboxCategoryValue)c).Category == category))
        {
          items.Add(new ComboboxCategoryValue( category ));
        }
      }

      // set the data source.
      comboBoxCategories.BeginUpdate();
      comboBoxCategories.DataSource = items;

      // select our first one.
      comboBoxCategories.SelectedIndex = 0;
      comboBoxCategories.EndUpdate();
    }
  }
}

