using System;
using System.Windows.Forms;
using myoddweb.viewer.forms;
using Classifier.Interfaces;
using System.IO;
using System.Linq;
using System.Reflection;
using myoddweb.viewer.utils;

namespace myoddweb.viewer
{
  internal class CommandLineInfo
  {
    private readonly string[] _args;

    public CommandLineInfo(string[] args)
    {
      // set the args.
      _args = args;
    }

    public bool DisplayHelp()
    {
      // either we have '-h' or w are missing '-d' and/or '-db' arguments.
      if (!HasCommand("h") && HasCommand("db") && HasCommand("d"))
      {
        return false;
      }

      MessageBox.Show("--h : Display this message box" + Environment.NewLine +
                      Environment.NewLine +
                      "--d : Directory that contains the classifier engines, Classifier.Engine.dll)." + Environment.NewLine +
                      "     The directory must contain 2 sub directories x64 and x86, depending on the version been debuged"+ Environment.NewLine +
                      Environment.NewLine +
                      "--db : The full path of the classifier database.",
                      "Help",
                      MessageBoxButtons.OK,
                      // display the error icon if we are not asking for help
                      // and we are missing some values.
                      (!HasCommand("h") && (!HasCommand("db") || !HasCommand("d")) ? MessageBoxIcon.Error : MessageBoxIcon.Information)
                      );
      return true;
    }

    /// <summary>
    /// Check if a command exists, (but it does not have to have a value.
    /// So you can look for -h and -db even if one has a value and the other does not.
    /// </summary>
    /// <param name="command">The value we are looking for.</param>
    /// <returns></returns>
    protected bool HasCommand(string command)
    {
      return _args.Any(s => s == $"--{command}");
    }

    /// <summary>
    /// Get the path of the database including the name.
    /// This is where the file is located.
    /// </summary>
    /// <returns></returns>
    public string GetDatabasePath()
    {
      var db = GetValue("db");
      if (null == db)
      {
        return null;
      }

      // if the file does not exist, return null.
      return !File.Exists(db) ? null : db;
    }

    /// <summary>
    /// Get the engines path, the engines are located in x86 and x64 sub paths
    /// We are looking to the root path that contains both those directories.
    /// </summary>
    /// <returns></returns>
    public string GetEnginePath()
    {
      var d = GetValue("d");
      if (null == d)
      {
        return null;
      }

      // if the file does not exist, return null.
      return !Directory.Exists( d) ? null : d;
    }

    protected string GetValue( string command )
    {
      for (var i = 0; i  < _args.Length; ++i )
      {
        var s = _args[i];
        if (s != $"--{command}")
        {
          continue;
        }

        // do we have a value after that?
        if (i + 1 == _args.Length)
        {
          return null;
        }

        var posibleValue = _args[i + 1];

        // is it another argument?
        if (posibleValue.Length > 2 && posibleValue.Substring(0, 2) == "--" )
        {
          return null;
        }

        // remove the spaces and quotes.
        return posibleValue.Trim().Trim('"');
      }

      // if we are here, we could not get it.
      return null;
    }
  }

  internal static class Program
  {
    /// <summary>
    /// Name for logging in the event viewer,
    /// </summary>
    private const string EventViewSource = "myoddweb.classifier";

    /// <summary>
    /// Initialise the engine and load all the resources neeed.
    /// Will load the database and so on to get the plugin ready for use.
    /// </summary>
    /// <param name="directoryName">string the directory we are loading from.</param>
    /// <param name="databasePath">string the name/path of the database we will be loading.</param>
    /// <param name="lastError">If needed, set the last error.</param>
    /// <returns></returns>
    private static IClassify1 InitialiseEngine(string directoryName, string databasePath, out string lastError )
    {
      lastError = "";
      var dllInteropPath = Path.Combine(directoryName, "x86\\Classifier.Interop.dll");
      var dllEnginePath = Path.Combine(directoryName, "x86\\Classifier.Engine.dll");
      if (Environment.Is64BitProcess)
      {
        dllInteropPath = Path.Combine(directoryName, "x64\\Classifier.Interop.dll");
        dllEnginePath = Path.Combine(directoryName, "x64\\Classifier.Engine.dll");
      }

      // look for the 
      Assembly asm;
      try
      {
        asm = Assembly.LoadFrom(dllInteropPath);
        if (null == asm)
        {
          lastError = $"Unable to load the interop file. '{dllInteropPath}'.";
          return null;
        }
      }
      catch (ArgumentException ex)
      {
        lastError = $"The interop file name/path does not appear to be valid. '{dllInteropPath}'.{Environment.NewLine}{Environment.NewLine}{ex.Message}";
        return null;
      }
      catch (FileNotFoundException ex)
      {
        lastError = $"Unable to load the interop file. '{dllInteropPath}'.{Environment.NewLine}{Environment.NewLine}{ex.Message}";
        return null;
      }

      // look for the interop interface
      var classifyEngine = TypeLoader.LoadTypeFromAssembly<IClassify1>(asm);
      if (null == classifyEngine)
      {
        // could not locate the interface.
        lastError = $"Unable to load the IClasify1 interface in the interop file. '{dllInteropPath}'.";
        return null;
      }

      if( !classifyEngine.Initialise(EventViewSource, dllEnginePath, databasePath))
      {
        return null;
      }

      // initialise the engine itself.
      return classifyEngine;
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
      // get the command line info.
      var ci = new CommandLineInfo(args);

      // is it help?
      if (ci.DisplayHelp())
      {
        return;
      }

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var databasePath = ci.GetDatabasePath();
      if (null == databasePath)
      {
        MessageBox.Show("Unknown or missing database path.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      var directoryName = ci.GetEnginePath();
      if (null == directoryName)
      {
        MessageBox.Show("Unknown or missing engines path.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // the text
      const string text = @"Blah";
      string lastError;
      var engine = InitialiseEngine(directoryName, databasePath, out lastError);
      Application.Run(new DetailsForm( engine, text ));

      // release it then.
      engine?.Release();
    }
  }
}
