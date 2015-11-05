using System;
using System.Windows.Forms;
using myoddweb.classifier.core;

namespace myoddweb.classifier.forms
{
  public partial class OptionsForm : Form
  {
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

      // check if we want to re-check all categories.
      reCheckCategories.Checked = options.ReCheckCategories;

      // and if we only want to check unknown categories.
      checkCategoryIfUnknown.Checked = _options.CheckIfUnownCategory;

      // we only check unknown categories if we check categories.
      checkCategoryIfUnknown.Enabled = reCheckCategories.Checked;

      // if we want to check only if the ctrl key is down.
      reCheckIfCtrl.Checked = _options.ReCheckIfCtrlKeyIsDown;
    }

    private void ok_Click(object sender, EventArgs e)
    {
      // we want to save our new settings.
      _options.ReCheckCategories = reCheckCategories.Checked;

      // and if we want to check unknown categories.
      _options.CheckIfUnownCategory = checkCategoryIfUnknown.Checked;

      // if we want to check only if the ctrl key is down.
      _options.ReCheckIfCtrlKeyIsDown = reCheckIfCtrl.Checked;
    }

    private void reCheckCategories_CheckedChanged(object sender, EventArgs e)
    {
      // we only check unknown categories if we check categories.
      checkCategoryIfUnknown.Enabled = reCheckCategories.Checked;
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
  }
}
