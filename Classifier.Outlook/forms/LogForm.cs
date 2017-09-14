using System;
using System.Windows.Forms;
using myoddweb.classifier.core;
using myoddweb.classifier.utils;
using System.Drawing;

namespace myoddweb.classifier.forms
{
  public partial class LogForm : Form
  {
    // the classifier engine
    private readonly Engine _engine;

    public LogForm( Engine engine)
    {
      Padding = new Padding(5);
      _engine = engine;
      InitializeComponent();
    }

    private void LogForm_Load(object sender, EventArgs e)
    {
      // make sure that we have the right size.
      RedrawFormSize();

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
      var widthSections = width / 12;
      var widthOfDateTime = 2* widthSections -1;
      var widthOfSource = 3* widthSections - 1;
      var widthOfEntry = 7 * widthSections;

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

    /// <summary>
    /// Redraw the form with the (new) size.
    /// </summary>
    private void RedrawFormSize()
    {
      var top = Padding.Top;
      ok.Location = new Point(
        ClientRectangle.Right -(Padding.Right) - ok.Width,
        ClientRectangle.Bottom - (Padding.Bottom) - ok.Height);

      listLog.Location = new Point(ClientRectangle.X + Padding.Left, top);
      listLog.Width = ok.Right - listLog.Left;
      listLog.Height = ok.Top - Padding.Top - listLog.Top;

      // only update the columns if we have created them already
      if (listLog.Columns.Count > 2)
      {
        listLog.BeginUpdate();
        // save the new width
        var width = listLog.Width;
        var widthOfDateTime = listLog.Columns[0].Width;
        var widthOfSource = listLog.Columns[2].Width;
        listLog.Columns[1].Width =  width - widthOfDateTime - widthOfSource - Padding.Right;
        listLog.EndUpdate();
      }
    }


    /// <summary>
    /// Called when the form is resized.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LogForm_Resize(object sender, EventArgs e)
    {
      RedrawFormSize();
    }
  }
}
