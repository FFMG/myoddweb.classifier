using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Office.Interop.Outlook;

namespace myoddweb.classifier.forms
{
  public partial class ProgressForm : Form
  {
    public bool Cancelled { get; private set; }

    private NativeWindow _mainWindow;

    public ProgressForm()
    {
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
        Show(_mainWindow);
      }

      // set the min/max and make sure it is not out of limit
      progressBar.Minimum = 0;
      progressBar.Maximum = range > limit ? limit : range;
    }

    public void Step()
    {
      progressBar.PerformStep();
    }
  }
}
