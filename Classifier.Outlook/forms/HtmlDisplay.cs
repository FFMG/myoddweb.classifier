using Classifier.Interfaces;
using myoddweb.viewer.utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace myoddweb.classifier.forms
{
  public class HtmlDisplay
  {
    /// <summary>
    /// The unprocessed, raw text.
    /// </summary>
    private readonly string _rawText;

    /// <summary>
    /// The classification engine.
    /// </summary>
    private readonly IClassify1 _classifyEngine;

    public HtmlDisplay(IClassify1 classifyEngine, string rawText )
    {
      // the raw text
      _rawText = rawText;

      // the classifier engine
      _classifyEngine = classifyEngine;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="body">The bodies.</param>
    /// <returns>The complete body of the html</returns>
    private static string GetHtml(string body)
    {
      var css = GetCss();
      return ($"<!DOCTYPE html><html><head><title>Message Details</title>{css}</head><body>{body}</body></html>");
    }

    public string GetHtml()
    {
      if (_classifyEngine == null)
      {
        // display the code.
        return GetHtml(GetParagraph("Missing Engine", "Unable to initialise the classifier engine"));
      }

      //  the final html
      var html = "";

      // try and categorise the text
      var wordsCategory = new List<WordCategory>();
      var categoryProbabilities = new Dictionary<int, double>();

      var sw = StopWatch.Start();
      var category = _classifyEngine == null ? -1 : _classifyEngine.Categorize(_rawText, 75, out wordsCategory, out categoryProbabilities);
      sw.Stop(@"Done : {0}.");

      // get the text body
      html += GetParsedBody(wordsCategory);

      // get the result
      html += GetSummary(category);

      // Display the categories table
      html += GetCategoriesTable(categoryProbabilities);

      // return what we found
      return GetHtml(html);
    }

    /// <summary>
    /// Get the sumary of the selected category/ 
    /// </summary>
    /// <param name="category"></param>
    /// <returns>String the sumary code for the selected category</returns>
    private string GetSummary(int category)
    {
      var categories = new Dictionary<int, string>();
      if (_classifyEngine?.GetCategories(out categories) <= 0)
      {
        return GetParagraph("Summary", "No categories listed!");
      }

      if (!categories.ContainsKey(category))
      {
        return GetParagraph("Summary", $"Could not find a valid category.");
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
      // this pattern is for all the characters we want to split the text into.
      // this is not exactly the same as the engine
      // but it should still split all the words.
      const string patternAll = @"[\s\p{P}\p{Z}\p{S}\p{C}\p{No}]";

      // the regex we will be using over and over...
      var regex = new Regex(patternAll);

      // this is to split everything, without actually removing the tags.
      // that way we can rebuild the entire text.
      var pattern = $@"(?<={patternAll}+)";

      var parts = Regex.Split(_rawText, pattern).ToList();
      var guids = new Dictionary<string, string>();
      var raws = new ConcurrentDictionary<string, string>();

      // the categories
      var categories = new Dictionary<int, string>();
      _classifyEngine?.GetCategories(out categories);

      // the list of tasks.
      var tasks = new List<Task>(wordsCategory.Count);

      foreach (var wordCategory in wordsCategory)
      {
        tasks.Add(
          Task.Run(() =>
          {
            // get akk the unique words we want to replace.
            var words = parts.FindAll(word => regex.Replace(word, " ").Trim() == wordCategory.Word)
              .Distinct();
            foreach (var word in words)
            {
              // the category id probability and 'clean' word.
              var categoryId = wordCategory.Category;
              var probability = wordCategory.Probability;
              var partClean = wordCategory.Word;
              
              // the class we will be using.
              var cr = $"cr{categoryId}";

              // wrapp the text with a class to give it color.
              var guid = Guid.NewGuid().ToString();
              var percent = (probability * 100).ToString(@"0.#0\%");
              guids.Add(guid, $"<span class='{cr}' title='{categories[categoryId]}, {percent}'>{partClean}</span>");
              var guidAndPart = word.Replace(partClean, guid);

              raws.TryAdd(word, guidAndPart);
            }
          })
        );
      }

      // wait for all.
      Task.WaitAll(tasks.ToArray());

      // now create the htm where we replace the raw text
      // with all our guid.
      var html = raws.Aggregate(_rawText, (guid, guidAndPart) => guid.Replace(guidAndPart.Key, guidAndPart.Value));

      // make sure that the text is html encoded so we don't display 'wrong' html
      // some emails risk to display things wrong, so it is best to escape everything.
      html = $@"<div class='text'>{WebUtility.HtmlEncode(html)}</div>";

      // now we can replace all the guids with the actual code.
      html = guids.Aggregate(html, (current, guid) => current.Replace(guid.Key, guid.Value));

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
      return $"<h1>{header}</h1><p>{paragraph}";
    }

    /// <summary>
    /// Create an html table with all the categories.
    /// </summary>
    /// <returns>string a categories table.</returns>
    private string GetCategoriesTable(Dictionary<int, double> categoryProbabilities)
    {
      var categories = new Dictionary<int, string>();
      if (_classifyEngine?.GetCategories(out categories) <= 0)
      {
        return "";
      }

      var table = "";
      foreach (var category in categories)
      {
        var cat = category.Key;
        var cr = $"cr{cat}";

        // get the percentage
        var probability = categoryProbabilities.ContainsKey(cat) ? (categoryProbabilities[cat] * 100).ToString(@"0.#0\%") : "0.00%";

        // add the row to the table.
        table += $"<tr><td style='text-align: right;'>{category.Key}</td><td class='{cr}'>{category.Value}</td><td style='text-align: right;'>{probability}</td></tr>";
      }
      return GetParagraph("Categories", $"<table>{table}</table>");
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
  }
}
