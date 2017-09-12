using System;
using System.Windows.Forms;
using myoddweb.classifier.core;
using myoddweb.classifier.utils;

namespace myoddweb.classifier.forms
{
  public partial class LogForm : Form
  {
    // the classifier engine
    private readonly Engine _engine;

    public LogForm( Engine engine)
    {
      _engine = engine;
      InitializeComponent();
    }

    private void LogForm_Load(object sender, EventArgs e)
    {
      // create the columns
      CreateColumns();

      // load the contents.
      ReloadLog();
    }

    private void CreateColumns()
    {
      // remove all the columns
      listLog.Columns.Clear();

      // we cannot click the header
      listLog.HeaderStyle = ColumnHeaderStyle.Nonclickable;

      // set up the widhts.
      var width = listLog.Width;
      var widthSections = width / 6;
      var widthOfDateTime = 1* widthSections -1;
      var widthOfSource = 2* widthSections - 1;
      var widthOfEntry = 3 * widthSections;

      // 
      listLog.View = View.Details;
      listLog.Columns.Add("Time", widthOfDateTime, HorizontalAlignment.Left);
      listLog.Columns.Add("Entry", widthOfEntry, HorizontalAlignment.Left);
      listLog.Columns.Add("Source", widthOfSource, HorizontalAlignment.Left);
    }

    private void ReloadLog()
    {
      // only one item at a time.
      listLog.MultiSelect = false;

      // remove everything
      listLog.Items.Clear();

      // get the Log.
      listLog.BeginUpdate();
      var logEntries = _engine.GetLogEntries(NumberOfEntriesToGet() );
      foreach (var entry in logEntries )
      {
        var item = new ListViewItem()
        {
          Text = Helpers.UnixToDateTime(entry.Unixtime ).ToString("MM/dd/yyyy HH:mm:ss"),
          ToolTipText = entry.Source,
          Tag = entry,
          SubItems = { entry.Entry, entry.Source }
        };
        listLog.Items.Add(item);
      }
      listLog.EndUpdate();

      // do we have anything at all?
      if (logEntries.Count > 0 )
      {
        // just select the first item in the list.
        listLog.Items[0].Selected = true;
        listLog.Select();
      }
    }

    private static int NumberOfEntriesToGet()
    {
      return 100;
    }

    private void Ok_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }
  }
}
