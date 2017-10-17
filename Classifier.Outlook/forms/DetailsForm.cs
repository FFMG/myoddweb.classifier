using Classifier.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifier.forms
{
  public partial class DetailsForm : Form
  {
    //  the html string parser to get the html code.
    private readonly HtmlDisplay _htmlDisplay;

    public DetailsForm(IClassify classifyEngine, ICategories categories, string rawText )
    {
      _htmlDisplay = new HtmlDisplay(classifyEngine, categories, rawText);

      // we can now init everything.
      InitializeComponent();

      // maximized the window.
      WindowState = FormWindowState.Maximized;
    }

    private void OnLoad(object sender, EventArgs e)
    {
      // make sure that we have the right size.
      RedrawFormSize();

      // get the html code.
      Cursor.Current = Cursors.WaitCursor;
      {
        // get the html
        var html = _htmlDisplay.GetHtml();

        // display it.
        DisplayHtml(html);
      }

      // restore the cursor.
      Cursor.Current = Cursors.Default;
    }

    // display the code in our form.
    private void DisplayHtml(string html)
    {
      webBrowserDetails.Navigate("about:blank");
      try
      {
        if (webBrowserDetails.Document != null)
        {
          webBrowserDetails.Document.Write(string.Empty);
        }
        else
        {
          if (webBrowserDetails.Document != null)
          {
            webBrowserDetails.Document.OpenNew(true);
            webBrowserDetails.Document.Write(html);
          }
          webBrowserDetails.Refresh();
        }
      }
      catch (Exception)
      {
        // do nothing with this
      }
      webBrowserDetails.DocumentText = html;
    }

    /// <summary>
    /// Called when the form is resized.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DetailsForm_Resize(object sender, EventArgs e)
    {
      RedrawFormSize();
    }

    /// <summary>
    /// Redraw the form with the (new) size.
    /// </summary>
    private void RedrawFormSize()
    {
      const int padding = 5;

      //      buttonPaste.Location = new Point(this.ClientRectangle.Right - padding - buttonPaste.Width, buttonPaste.Location.Y);
      //      var top = buttonPaste.Bottom + padding;
      var top = padding;
      webBrowserDetails.Location = new Point(ClientRectangle.X + padding, top);

      webBrowserDetails.Width = ClientRectangle.Right - (2 * padding);
      webBrowserDetails.Height = ClientRectangle.Bottom + -(2 * padding);
    }
    
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Escape)
      {
        Close();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }
  }
}