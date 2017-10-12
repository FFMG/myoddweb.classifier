using System;
using System.Windows.Forms;
using myoddweb.classifier.core;
using myoddweb.classifier.utils;
using System.Drawing;
using System.Linq;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.forms
{
  public partial class LogForm : Form
  {
    // the classifier engine
    private readonly ILogger _logger;

    private readonly uint _numberOfItemsToDisplay;

    public LogForm( ILogger logger, uint numberOfItemsToDisplay )
    {
      Padding = new Padding(5);
      _logger = logger;
      _numberOfItemsToDisplay = numberOfItemsToDisplay;
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
      var widthSections = width / 13;
      var widthOfDateTime = 2* widthSections -1;
      var widthOfSource = 3* widthSections - 1;
      var widthOfEntry = 7 * widthSections;
      var widthOfEntryId = 1 * widthSections;

      // 
      listLog.View = View.Details;
      listLog.Columns.Add("Time", widthOfDateTime, HorizontalAlignment.Left);
      listLog.Columns.Add("Entry", widthOfEntry, HorizontalAlignment.Left);
      listLog.Columns.Add("Source", widthOfSource, HorizontalAlignment.Left);
      listLog.Columns.Add("Id", widthOfEntryId, HorizontalAlignment.Left); 
    }

    private void ReloadLog()
    {
      // only one item at a time.
      listLog.MultiSelect = false;

      // remove everything
      listLog.Items.Clear();

      // get the Log and make sure that it is ordered by date.
      listLog.BeginUpdate();
      var logEntries = _logger.GetLogEntries( (int)_numberOfItemsToDisplay );
      logEntries = logEntries.OrderByDescending(o => o.Unixtime).ToList();
      foreach (var entry in logEntries )
      {
        var item = new ListViewItem()
        {
          Text = Helpers.UnixToDateTime(entry.Unixtime ).ToString("MM/dd/yyyy HH:mm:ss"),
          ToolTipText = entry.Source,
          Tag = entry,
          SubItems = { entry.Entry, entry.Source, entry.Id.ToString() }
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
        var widthOfEntryId = listLog.Columns[3].Width;
        listLog.Columns[1].Width =  width - (widthOfDateTime + widthOfSource + widthOfEntryId) - Padding.Right;
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
