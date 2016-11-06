using System;
using System.Windows.Forms;
using myoddweb.classifier.core;
using System.Collections.Generic;
using System.Drawing;

namespace myoddweb.classifier.forms
{
  public partial class OptionsForm : Form
  {
    public class ComboboxWeightValue
    {
      public uint Weight { get; set; }
      public override string ToString() { return Convert.ToString(  Weight ); }
    }

    // all the options.
    private readonly Options _options;

    // all the categories.
    private readonly Categories _categories;

    // all the engine.
    private readonly Engine _engine;

    public OptionsForm( Engine engine, Options options, Categories categories )
    {
      _engine = engine;
      _categories = categories;
      _options = options;
      InitializeComponent();
    }

    private void RebuildCombos()
    {
      // the categories
      RebuildMagnetsCombo();

      // the combos
      RebuildUsersCombos();
    }

    /// <summary>
    /// List all the users combo.
    /// </summary>
    private void RebuildUsersCombos()
    {
      // remove everything
      comboUser.Items.Clear();

      // and set the folder values.
      var items = new List<ComboboxWeightValue>();

      // select the first item
      var selectedIndex = 0;

      // guess what the category could be
      var currentWeight = _options.UserWeight;

      // go around all the folders.
      for (var i = 1; i <= 40; ++i)
      {
        // is that our current one?
        if (i == currentWeight)
        {
          selectedIndex = items.Count;
        }
        items.Add(new ComboboxWeightValue { Weight = (uint)i });
      }

      // the data source.
      comboUser.DataSource = items;

      // select our current one.
      comboUser.SelectedIndex = selectedIndex;
    }

    /// <summary>
    /// List all the magnets weight.
    /// </summary>
    private void RebuildMagnetsCombo()
    {
      // remove everything
      comboMagnets.Items.Clear();

      // and set the folder values.
      var items = new List<ComboboxWeightValue>();

      // select the first item
      var selectedIndex = 0;

      // guess what the category could be
      var currentWeight = _options.MagnetsWeight;

      // go around all the folders.
      for (var i = 1; i <= 20; ++i )
      {
        // is that our current one?
        if (i == currentWeight)
        {
          selectedIndex = items.Count;
        }
        items.Add(new ComboboxWeightValue { Weight= (uint)i });
      }

      // the data source.
      comboMagnets.DataSource = items;

      // select our current one.
      comboMagnets.SelectedIndex = selectedIndex;
    }

    private void Ok_Click(object sender, EventArgs e)
    {
      // we want to save our new settings.
      _options.ReCheckCategories = reCheckCategories.Checked;

      // and if we want to check unknown categories.
      _options.CheckIfUnownCategory = checkCategoryIfUnknown.Checked;

      // if we want to check only if the ctrl key is down.
      _options.ReCheckIfCtrlKeyIsDown = reCheckIfCtrl.Checked;

      // save the weights of the magnets.
      _options.MagnetsWeight = GetMagnetsWeight();

      // save the weights of the magnets.
      _options.UserWeight = GetUserWeight();

      // save the percent option
      _options.CommonWordsMinPercent = GetCommonWordsMinPercent();

      // automatically train messages with magnet
      _options.ReAutomaticallyTrainMagnetMessages = checkAutomaticallyMagnetTrain.Checked;

      // automatically train messages?
      _options.ReAutomaticallyTrainMessages = checkAutomaticallyTrain.Checked;
    }

    private uint GetCommonWordsMinPercent()
    {
      return (uint)numericCommonPercent.Value;
    }

    private uint GetMagnetsWeight()
    {
      // did we select anything at all?
      if (comboMagnets.SelectedIndex == -1)
      {
        return (uint)Options.DefaultOptions.MagnetsWeight;
      }

      // otherwise get the id
      return ((ComboboxWeightValue)comboMagnets.SelectedItem).Weight;
    }

    private uint GetUserWeight()
    {
      // did we select anything at all?
      if (comboUser.SelectedIndex == -1)
      {
        return (uint)Options.DefaultOptions.UserWeight;
      }

      // otherwise get the id
      return ((ComboboxWeightValue)comboUser.SelectedItem).Weight;
    }

    private void reCheckCategories_CheckedChanged(object sender, EventArgs e)
    {
      // we only check unknown categories if we check categories.
      checkCategoryIfUnknown.Enabled = reCheckCategories.Checked;
    }

    private void checkAutomaticallyTrain_CheckedChanged(object sender, EventArgs e)
    {
      // we only check unknown categories if we check categories.
      checkAutomaticallyMagnetTrain.Enabled = !checkAutomaticallyTrain.Checked;
    }

    private void Categories_Click(object sender, EventArgs e)
    {
      using (var categoriesForm = new CategoriesForm(categories: _categories, engine: _engine))
      {
        categoriesForm.ShowDialog();
      }
    }

    private void Magnets_Click(object sender, EventArgs e)
    {
      using (var rulesForm = new MagnetsForm( engine: _engine, categories: _categories ))
      {
        rulesForm.ShowDialog();
      }
    }

    private void OnLoad(object sender, EventArgs e)
    {
      // check if we want to re-check all categories.
      reCheckCategories.Checked = _options.ReCheckCategories;

      // and if we only want to check unknown categories.
      checkCategoryIfUnknown.Checked = _options.CheckIfUnownCategory;

      // we only check unknown categories if we check categories.
      checkCategoryIfUnknown.Enabled = reCheckCategories.Checked;

      // if we want to check only if the ctrl key is down.
      reCheckIfCtrl.Checked = _options.ReCheckIfCtrlKeyIsDown;

      // check if we want to train new messages or not.
      checkAutomaticallyTrain.Checked = _options.ReAutomaticallyTrainMessages;

      // check if we want to train new messages that used a magnet or not.
      checkAutomaticallyMagnetTrain.Checked = _options.ReAutomaticallyTrainMagnetMessages;

      // rebuild the combo
      RebuildCombos();

      // the spinner
      RebuildSpinner();

      // the defaults.
      RebuildDefaults();
    }

    private void RebuildDefaults()
    {
      labelMagnets.Text = $"[ {(int)Options.DefaultOptions.MagnetsWeight} ]";
      labelMagnets.ForeColor = Color.DarkGray;

      labelUserTrained.Text = $"[ {(int)Options.DefaultOptions.UserWeight} ]";
      labelUserTrained.ForeColor = Color.DarkGray;

      labelCommonWord.Text = $"[ {(int)Options.DefaultOptions.CommonWordsMinPercent}% ]";
      labelCommonWord.ForeColor = Color.DarkGray;
    }

    private void RebuildSpinner()
    {
      numericCommonPercent.Maximum = 100;
      numericCommonPercent.Minimum = 1;
      numericCommonPercent.ReadOnly = false;
      numericCommonPercent.Value = _options.CommonWordsMinPercent;
    }
  }
}
