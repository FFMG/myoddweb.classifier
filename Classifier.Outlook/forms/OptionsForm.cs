using System;
using System.Windows.Forms;
using myoddweb.classifier.core;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;

namespace myoddweb.classifier.forms
{
  public partial class OptionsForm : Form
  {
    public class ComboboxLogSizeValue
    {
      public uint Size { get; set; }
      public override string ToString() { return Convert.ToString(Size); }
    }

    public class ComboboxWeightValue
    {
      public uint Weight { get; set; }
      public override string ToString() { return Convert.ToString(  Weight ); }
    }

    public class ComboboxLogLevelValue
    {
      public Options.LogLevels Level { get; set; }
      public override string ToString() { return Convert.ToString(Level); }
    }

    public class ComboboxRetentionValue
    {
      public uint Days { get; set; }
      public override string ToString() {
        return $"{Days} days";
      }
    }

    // all the categories.
    private readonly Categories _categories;

    // all the engine.
    private readonly IEngine _engine;

    public OptionsForm( IEngine engine, Categories categories )
    {
      _engine = engine;
      _categories = categories;
      InitializeComponent();
    }

    #region Combos
    /// <summary>
    /// Rebuild all the combos.
    /// </summary>
    private void RebuildCombos()
    {
      // the categories
      RebuildMagnetsCombo();

      // the users combos
      RebuildUsersCombos();

      // the logs combo
      RebuildLogCombos();
    }

    /// <summary>
    /// Rebuild the log combos.
    /// </summary>
    private void RebuildLogCombos()
    {
      // log levels
      RebuildLogLevelCombo();

      // retention policy
      RebuildRetentionPolicyCombo();

      // the number of items we want to display
      RebuildDisplayLogSize();
    }

    /// <summary>
    /// The log level combo.
    /// </summary>
    private void RebuildLogLevelCombo()
    {
      // remove everything
      comboLogLevel.Items.Clear();

      // make the list read only
      comboLogLevel.DropDownStyle = ComboBoxStyle.DropDownList;

      // and set the folder values.
      var items = new List<ComboboxLogLevelValue>();

      // select the first item
      var selectedIndex = 0;

      // guess what the category could be
      var currentLogLevel = _engine.Options.LogLevel;

      // go around all the folders.
      foreach( Options.LogLevels i in Enum.GetValues( typeof(Options.LogLevels)))
      {
        // is that our current one?
        if (i == currentLogLevel)
        {
          selectedIndex = items.Count;
        }
        items.Add(new ComboboxLogLevelValue { Level = i });
      }

      // the data source.
      comboLogLevel.DataSource = items;

      // select our current one.
      comboLogLevel.SelectedIndex = selectedIndex;
    }

    /// <summary>
    /// Add the retention policy combo.
    /// </summary>
    private void RebuildRetentionPolicyCombo()
    {
      // remove everything
      comboRetention.Items.Clear();

      // make the list read only
      comboRetention.DropDownStyle = ComboBoxStyle.DropDownList;

      // and set the folder values.
      var items = new List<ComboboxRetentionValue>();

      // select the first item
      var selectedIndex = 0;

      // guess what the category could be
      var currentRetention = _engine.Options.LogRetention;

      // go around all the folders.
      uint[] days = { 1, 5, 10, 30, 90 };
      foreach(var i in days )
      {
        // is that our current one?
        if (i == currentRetention)
        {
          selectedIndex = items.Count;
        }
        items.Add(new ComboboxRetentionValue { Days = i });
      }

      // the data source.
      comboRetention.DataSource = items;

      // select our current one.
      comboRetention.SelectedIndex = selectedIndex;
    }

    /// <summary>
    /// List all the users combo.
    /// </summary>
    private void RebuildUsersCombos()
    {
      // remove everything
      comboUser.Items.Clear();

      // make the list read only
      comboUser.DropDownStyle = ComboBoxStyle.DropDownList;

      // and set the folder values.
      var items = new List<ComboboxWeightValue>();

      // select the first item
      var selectedIndex = 0;

      // guess what the category could be
      var currentWeight = _engine.Options.UserWeight;

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

      // make the list read only
      comboMagnets.DropDownStyle = ComboBoxStyle.DropDownList;

      // and set the folder values.
      var items = new List<ComboboxWeightValue>();

      // select the first item
      var selectedIndex = 0;

      // guess what the category could be
      var currentWeight = _engine.Options.MagnetsWeight;

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

    private void RebuildDisplayLogSize()
    {
      comboDisplaySize.Items.Clear();

      // make the list read only
      comboDisplaySize.DropDownStyle = ComboBoxStyle.DropDownList;

      // and set the folder values.
      var items = new List<ComboboxLogSizeValue>();

      // select the first item
      var selectedIndex = 0;

      // guess what the category could be
      var currentDisplaySize = _engine.Options.LogDisplaySize;

      // go around all the folders.
      uint[] displaySizes = { 10, 50, 100, 200, 500 };
      foreach (var i in displaySizes)
      {
        // is that our current one?
        if (i == currentDisplaySize )
        {
          selectedIndex = items.Count;
        }
        items.Add(new ComboboxLogSizeValue() { Size = i });
      }

      // the data source.
      comboDisplaySize.DataSource = items;

      // select our current one.
      comboDisplaySize.SelectedIndex = selectedIndex;
    }
    #endregion

    private void Ok_Click(object sender, EventArgs e)
    {
      // we want to save our new settings.
      _engine.Options.ReCheckCategories = reCheckCategories.Checked;

      // and if we want to check unknown categories.
      _engine.Options.CheckIfUnKnownCategory = checkCategoryIfUnknown.Checked;

      // do we want to check unprocessed emails on startup
      _engine.Options.CheckUnProcessedEmailsOnStartUp = checkUnProcessedEmails.Checked;

      // if we want to check only if the ctrl key is down.
      _engine.Options.ReCheckIfCtrlKeyIsDown = reCheckIfCtrl.Checked;

      // save the weights of the magnets.
      _engine.Options.MagnetsWeight = GetMagnetsWeight();

      // save the weights of the magnets.
      _engine.Options.UserWeight = GetUserWeight();

      // save the percent option
      _engine.Options.CommonWordsMinPercent = GetCommonWordsMinPercent();

      // the min category percent
      _engine.Options.MinPercentage = GetMinPercentage();

      // automatically train messages with magnet
      _engine.Options.ReAutomaticallyTrainMagnetMessages = checkAutomaticallyMagnetTrain.Checked;

      // automatically train messages?
      _engine.Options.ReAutomaticallyTrainMessages = checkAutomaticallyTrain.Checked;

      // set the log level
      _engine.Options.LogLevel = GetLogLevel();

      // set the retention policy
      _engine.Options.LogRetention = GetLogRetentionPolicy();

      // the classification delay in seconds.
      _engine.Options.ClassifyDelaySeconds = GetClassifyDelaySeconds();

      // the number of items we want to display in the log.
      _engine.Options.LogDisplaySize = GetNumberOfEntriesToDisplay();
    }

    private uint GetLogRetentionPolicy()
    {
      // did we select anything at all?
      if (comboRetention.SelectedIndex == -1)
      {
        return (uint)Options.DefaultOptions.LogRetention;
      }

      // otherwise get the id
      return ((ComboboxRetentionValue)comboRetention.SelectedItem).Days;
    }

    private Options.LogLevels GetLogLevel()
    {
      // did we select anything at all?
      if (comboLogLevel.SelectedIndex == -1)
      {
        return (Options.LogLevels)Options.DefaultOptions.LogLevel;
      }

      // otherwise get the id
      return ((ComboboxLogLevelValue)comboLogLevel.SelectedItem).Level;
    }

    /// <summary>
    /// The minimum percent value a message must be in a category for the classifier to use it.
    /// </summary>
    /// <returns></returns>
    private uint GetMinPercentage()
    {
      return (uint)numericMinPercentage.Value;
    }

    /// <summary>
    /// Percentage word common before they are ignored.
    /// If a word is 50% common accross all the categories, then we ignore it.
    /// </summary>
    /// <returns></returns>
    private uint GetCommonWordsMinPercent()
    {
      return (uint)numericCommonPercent.Value;
    }

    private uint GetClassifyDelaySeconds()
    {
      return (uint)numericUpDownClassifyDelay.Value;
    }

    private uint GetNumberOfEntriesToDisplay()
    {
      if (comboDisplaySize.SelectedIndex == -1)
      {
        return (uint)Options.DefaultOptions.LogDisplaySize;
      }

      // otherwise get the log size
      return ((ComboboxLogSizeValue)comboDisplaySize.SelectedItem).Size;
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

    private Version GetFileVersion()
    {
      var assembly = System.Reflection.Assembly.GetExecutingAssembly();
      var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      return new Version(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
    }

    private void OnLoad(object sender, EventArgs e)
    {
      //  get the file version
      var version = GetFileVersion();

      // get the engine version.
      var engineVersion = _engine.GetEngineVersion();

      //Version version = Assembly.GetEntryAssembly().GetName().Version;
      Text = $"Options - [{version.Major}.{version.Minor}.{version.Build}.{version.Revision}] - (Engine [{engineVersion.Major}.{engineVersion.Minor}.{engineVersion.Build}])";

      // check if we want to re-check all categories.
      reCheckCategories.Checked = _engine.Options.ReCheckCategories;

      // and if we only want to check unknown categories.
      checkCategoryIfUnknown.Checked = _engine.Options.CheckIfUnKnownCategory;

      // do we want to check unprocessed emails on startup
      checkUnProcessedEmails.Checked = _engine.Options.CheckUnProcessedEmailsOnStartUp;

      // we only check unknown categories if we check categories.
      checkCategoryIfUnknown.Enabled = reCheckCategories.Checked;

      // if we want to check only if the ctrl key is down.
      reCheckIfCtrl.Checked = _engine.Options.ReCheckIfCtrlKeyIsDown;

      // check if we want to train new messages or not.
      checkAutomaticallyTrain.Checked = _engine.Options.ReAutomaticallyTrainMessages;

      // check if we want to train new messages that used a magnet or not.
      checkAutomaticallyMagnetTrain.Checked = _engine.Options.ReAutomaticallyTrainMagnetMessages;

      // rebuild the combo
      RebuildCombos();

      // the spinners
      RebuildCommonPercentSpinner();
      RebuildMinPercentageSpinner();

      // the classify delay in seconds.
      RebuildClassifyDelay();

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

      labelMinPercentage.Text = $"[ {(int)Options.DefaultOptions.MinPercentage}% ]";
      labelCommonWord.ForeColor = Color.DarkGray;

      labelDefaultLogLevel.Text = $"[ {(Options.LogLevels)Options.DefaultOptions.LogLevel} ]";
      labelDefaultLogLevel.ForeColor = Color.DarkGray;

      labelDefaultRetention.Text = $"[ {(int)Options.DefaultOptions.LogRetention} Days ]";
      labelCommonWord.ForeColor = Color.DarkGray;

      labelDefaultClassifyDelay.Text = $"[ {(int)Options.DefaultOptions.ClassifyDelaySeconds} Seconds ]";
      labelDefaultClassifyDelay.ForeColor = Color.DarkGray;

      labelDisplaySizeDefault.Text = $"[ {(int)Options.DefaultOptions.LogDisplaySize} ]";
      labelDisplaySizeDefault.ForeColor = Color.DarkGray;
    }

    private void RebuildCommonPercentSpinner()
    {
      numericCommonPercent.Maximum = 100;
      numericCommonPercent.Minimum = 1;
      numericCommonPercent.ReadOnly = false;
      numericCommonPercent.Value = _engine.Options.CommonWordsMinPercent;
    }

    private void RebuildMinPercentageSpinner()
    {
      numericMinPercentage.Maximum = 100;
      numericMinPercentage.Minimum = 1;
      numericMinPercentage.ReadOnly = false;
      numericMinPercentage.Value = _engine.Options.MinPercentage;
    }

    private void RebuildClassifyDelay()
    {
      numericUpDownClassifyDelay.Maximum = 100;
      numericUpDownClassifyDelay.Minimum = 0;
      numericUpDownClassifyDelay.ReadOnly = false;
      numericUpDownClassifyDelay.Value = _engine.Options.ClassifyDelaySeconds;
    }

    private void Log_Click(object sender, EventArgs e)
    {
      using (var logForm = new LogForm(engine: _engine, numberOfItemsToDisplay: GetNumberOfEntriesToDisplay() ))
      {
        logForm.ShowDialog();
      }
    }
  }
}
