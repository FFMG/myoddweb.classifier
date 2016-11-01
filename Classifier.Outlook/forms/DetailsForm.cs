using Classifier.Interfaces;
using myoddweb.classifier.core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace myoddweb.classifier.forms
{
  public partial class DetailsForm : Form
  {
    /// <summary>
    /// The unprocessed, raw text.
    /// </summary>
    private readonly string _rawText;

    /// <summary>
    /// The classification engine.
    /// </summary>
    private readonly Engine _classifyEngine;

    public DetailsForm(Engine classifyEngine, string rawText )
    {
      // the classifier engine
      _classifyEngine = classifyEngine;

      // the raw text
      _rawText = rawText;

      // we can now init everything.
      InitializeComponent();

      // maximized the window.
      WindowState = FormWindowState.Maximized;
    }

    private void OnLoad(object sender, EventArgs e)
    {
      // make sure that we have the right size.
      RedrawFormSize();

      if (_classifyEngine == null)
      {
        // display the code.
        DisplayHtml(GetHtml(GetParagraph( "Missing Engine", "Unable to initialise the classifier engine" )));
        return;
      }

      //  the final html
      var html = "";

      Cursor.Current = Cursors.WaitCursor;
      {
        // try and categorise the text
        List<WordCategory> wordsCategory;
        var category = _classifyEngine.Categorize(_rawText, _classifyEngine.MinPercentage, out wordsCategory);

        // get the text body
        html += GetParsedBody(wordsCategory);

        // get the result
        html += GetSummary(category);

        // Display the categories table
        html += GetCategoriesTable();
      }
      Cursor.Current = Cursors.Default;

      // display the code.
      DisplayHtml(GetHtml(html));
    }

    private string GetSummary(int category)
    {
      // did we find a valid category?
      if (category == -1 )
      {
        return GetParagraph("Summary", "Could not select a possible category");
      }

      // get the categories.
      var categories = _classifyEngine.GetCategories();

      // do we have any??
      if (categories.Count == 0)
      {
        return GetParagraph("Summary", "No categories listed!");
      }
      
      var cr = $"cr{category}";
      return GetParagraph("Summary", $"Selected category <span class='{cr}'>{categories[category]}({category})</span>");
    }

    /// <summary>
    /// Get the text body with html code.
    /// </summary>
    /// <param name="wordsCategory">Each used words and category.</param>
    /// <returns></returns>
    private string GetParsedBody(List<WordCategory> wordsCategory)
    {
      var html = "";

      // this pattern is for all the characters we want to split the text into.
      // this is not exactly the same as the engine
      // but it should still split all the words.
      const string patternAll = @"[\s\p{P}\p{Z}\p{S}\p{C}\p{No}]";

      // this is to split everything, without actually removing the tags.
      // that way we can rebuild the entire text.
      var pattern = $@"(?<={patternAll}+)";
      var parts = Regex.Split(_rawText, pattern);
      var guids = new Dictionary<Guid, string>();

      // the categories
      var categories = _classifyEngine.GetCategories();

      foreach (var part in parts)
      {
        // clean uo the code to be the same as what the engine would use.
        // but we will use the original 'part' to rebuild the string.
        var partClean = Regex.Replace(part, patternAll, " ").Trim();

        // do we have that word/category?
        var wordCategory = wordsCategory.Find(s => s.Word == partClean);
        if (null != wordCategory)
        {
          // the category id
          var categoryId = wordCategory.Category;

          // the class we will be using.
          var cr = $"cr{categoryId}";

          // wrapp the text with a class to give it color.
          var guid = Guid.NewGuid();
          var percent =(wordCategory.Probability * 100).ToString( @"0.#0\%");
          guids.Add(guid, $"<span class='{cr}' title='{categories[categoryId]}, {percent}'>{partClean}</span>");
          var subPart = part.Replace(partClean, guid.ToString() );

          // and add it all together.
          html += subPart;
        }
        else
        {
          // this is not a string we know.
          html += part;
        }
      }

      // make sure that the text is html encoded so we don't display 'wrong' html
      // some emails risk to display things wrong, so it is best to escape everything.
      html = $@"<div class='text'>{WebUtility.HtmlEncode(html)}</div>";

      // now we can replace all the guids with the actual code.
      html = guids.Aggregate(html, (current, guid) => current.Replace(guid.Key.ToString(), guid.Value));

      return GetParagraph("Classified", html);
    }

    /// <summary>
    /// Get a paragraph, with a header and a body
    /// </summary>
    /// <param name="header">The header we would like to use.</param>
    /// <param name="paragraph">The paragraph.</param>
    /// <returns></returns>
    private static string GetParagraph(string header, string paragraph)
    {
      return $"<h1>{header}</h1><p>{paragraph}</p>";
    }

    /// <summary>
    /// Create an html table with all the categories.
    /// </summary>
    /// <returns>string a categories table.</returns>
    private string GetCategoriesTable()
    {
      var categories = _classifyEngine.GetCategories();
      if (categories.Count == 0)
      {
        return "";
      }

      string table = "";
      foreach (var category in categories)
      {
        int cat = category.Key;
        var cr = $"cr{cat}";

        // add the row to the table.
        table += $"<tr><td style='text-align: right;'>{category.Key}</td><td class='{cr}'>{category.Value}</td><tr>";
      }
      return GetParagraph("Categories", $"<table>{table}</table>");
    }

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="body">The bodies.</param>
    /// <returns>The complete body of the html</returns>
    private static string GetHtml(string body)
    {
      var css = GetCss();
      return ($"<!DOCTYPE html>{css}<html><body>{body}</body></html>");
    }

    /// <summary>
    /// Get the common css code.
    /// </summary>
    /// <returns>The css code.</returns>
    private static string GetCss()
    {
      return @"<style>
        body { background-color: #eeeeee; font-family: 'Open Sans', sans-serif;}
        h1 { color: #6b6467; font-size: 1.1em; font-weight: 300; line-height: 40px; margin: 0 0 6px; }
        p { color: #2b2729; font-size: 0.9em; font-weight: 400; line-height: 24px; margin: 0 0 4px; }
        .text{ color: #808080;}

        th, td { padding: 1px; padding-left: 5px; padding-right: 5px; text-align: left;}
        table, td, th { border: 1px solid black; }

        .cr1{ color: #80BFFF; }
        .cr2{ color: #148F77; }
        .cr3{ color: #922B21; }
        .cr4{ color: #76448A; }
        .cr5{ color: #E09E72; }
        .cr6{ color: #148F77; }
        .cr7{ color: #F90808; }
        .cr8{ color: #EB7088; }
        .cr9{ color: #784212; }
        .cr10{ color: #20B2AA; }
        .cr11{ color: #4286f4; }
        .cr12{ color: #5ff442; }
        .cr13{ color: #ff4500; }
        .cr14{ color: #e8f442; }
        .cr15{ color: #FFC300; }
        </style>";
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