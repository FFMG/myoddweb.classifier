using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace myoddweb.classifier.forms
{
  public partial class ProgressForm : Form
  {
    public bool Cancelled { get; private set; }

    /// <summary>
    /// The window we are centering with.
    /// </summary>
    private readonly NativeWindow _mainWindow;

    /// <summary>
    /// The title we will be displaying
    /// </summary>
    private readonly string _title;

    public ProgressForm( string title )
    {
      _title = title;
      InitializeComponent();

      _mainWindow = new NativeWindow();
      _mainWindow.AssignHandle(Process.GetCurrentProcess().MainWindowHandle);
    }

    public new void Dispose()
    {
      _mainWindow.ReleaseHandle();
      base.Dispose();
    }

    private void ProgressForm_Load(object sender, EventArgs e)
    {
      Text = _title;
    }

    private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing)
      {
        Cancelled = true;
      }
    }

    public void AddRange(int range, int limit)
    {
      // are we even visible?
      if (!Visible)
      {
        StartPosition = FormStartPosition.CenterParent;
        Show(_mainWindow);
      }

      // set the min/max and make sure it is not out of limit
      progressBar.Minimum = 0;
      progressBar.Maximum = range > limit ? limit : range;
    }

    public void Step()
    {
      progressBar.PerformStep();

      Text = $"{_title} [{(int)((progressBar.Value * 100) / (double)progressBar.Maximum)}%]";
    }
  }
}