using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using myoddweb.classifier.core;
using Classifier.Interfaces;
using System.Reflection;
using myoddweb.classifier.utils;

namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  public class TestCommon
  {
    /// <summary>
    /// Name for logging in the event viewer,
    /// </summary>
    private const string EventViewSource = "myoddweb.classifier";

    // the engine
    private Engine _engine;

    protected static string DbName = "ctest_database.classifier";
    protected static string CleandbName = "clean_ctest_database.classifier";

    protected Engine TheEngine => _engine ?? (_engine = CreateEngine());

    public static string DirectoryPath { get; private set; }
    public static string DatabaseFullPath { get; private set; }
    public static string CleanDatabaseFullPath { get; private set; }

    protected static string GetDirectory()
    {
      // the paths we will be using.
      var directoryName = AppDomain.CurrentDomain.BaseDirectory;
      if (File.Exists($"{directoryName}\\x64\\classifier.engine.dll"))
      {
        return directoryName;
      }

      var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (var loadedAssembly in loadedAssemblies)
      {
        directoryName = Path.GetDirectoryName(loadedAssembly.Location);
        for (; directoryName!= null;)
        {
          if (File.Exists($"{directoryName}\\x64\\classifier.engine.dll"))
          {
            return directoryName;
          }

          try
          {
            directoryName = Directory.GetParent(directoryName).FullName;
          }
          catch
          {
            break;
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Initialise the engine and load all the resources neeed.
    /// Will load the database and so on to get the plugin ready for use.
    /// </summary>
    /// <param name="directoryName">string the directory we are loading from.</param>
    /// <param name="databasePath">string the name/path of the database we will be loading.</param>
    /// <returns></returns>
    private static IClassify1 InitialiseEngine(string directoryName, string databasePath)
    {
      var dllInteropPath = Path.Combine(directoryName, "x86\\Classifier.Interop.dll");
      var dllEnginePath = Path.Combine(directoryName, "x86\\Classifier.Engine.dll");
      if (Environment.Is64BitProcess)
      {
        dllInteropPath = Path.Combine(directoryName, "x64\\Classifier.Interop.dll");
        dllEnginePath = Path.Combine(directoryName, "x64\\Classifier.Engine.dll");
      }

      // look for the 
      Assembly asm = null;
      try
      {
        asm = Assembly.LoadFrom(dllInteropPath);
        if (null == asm)
        {
          throw new Exception($"Unable to load the interop file. '{dllInteropPath}'.");
        }
      }
      catch (ArgumentException ex)
      {
        throw new Exception($"The interop file name/path does not appear to be valid. '{dllInteropPath}'.{Environment.NewLine}{Environment.NewLine}{ex.Message}");
      }
      catch (FileNotFoundException ex)
      {
        throw new Exception($"Unable to load the interop file. '{dllInteropPath}'.{Environment.NewLine}{Environment.NewLine}{ex.Message}");
      }

      // look for the interop interface
      var classifyEngine = TypeLoader.LoadTypeFromAssembly<Classifier.Interfaces.IClassify1>(asm);
      if (null == classifyEngine)
      {
        // could not locate the interface.
        throw new Exception($"Unable to load the IClasify1 interface in the interop file. '{dllInteropPath}'.");
      }

      // initialise the engine itself.
      if (!classifyEngine.Initialise(EventViewSource, dllEnginePath, databasePath))
      {
        return null;
      }
      return classifyEngine;
    }

    protected static Engine CreateEngine()
    {
      // the paths we will be using.
      DirectoryPath = GetDirectory();
      if (null == DirectoryPath)
      {
        throw new Exception("I could not find the current working path.");
      }

      //  the database path
      DatabaseFullPath = $"{DirectoryPath}\\{DbName}";

      // if the file exists, then we need to remove it.
      if (File.Exists(DatabaseFullPath))
      {
        // just in case it does exist.
        File.Delete(DatabaseFullPath);

        if (File.Exists(DatabaseFullPath))
        {
          Assert.Fail("Could not remove the old db from previous tests.");
        }
      }

      CleanDatabaseFullPath = $"{DirectoryPath}\\{CleandbName}";
      if (!File.Exists(CleanDatabaseFullPath))
      {
        var e = new Engine( InitialiseEngine( DirectoryPath, CleanDatabaseFullPath ));
        e.Config.SetConfig("Option.CommonWordsMinPercent", "100");
        e.Release();
      }

      // make sure that we have a clean database.
      File.Copy(CleanDatabaseFullPath, DatabaseFullPath);
      if (!File.Exists(DatabaseFullPath))
      {
        Assert.Fail( "Could not create the new db from the clean db." );
      }

      //create the engine
      return new Engine(InitialiseEngine(DirectoryPath, DatabaseFullPath));
    }

    protected void ReleaseEngine( bool removeCleanDb )
    {
      // don't do it more than once...
      if (null != _engine)
      {
        // release the engine
        _engine.Release();

        // free the memory, (but the gc might not do it right away).
        _engine = null;
      }

      // try and remove the database.
      try
      {
        File.Delete(DatabaseFullPath);
      }
      catch (Exception)
      {
        // ignore the Exception ...
        // the file will be deleted later.
      }

      try
      {
        // if the file exists, then we need to remove it.
        if (removeCleanDb == true && File.Exists(CleanDatabaseFullPath))
        {
          // just in case it does exist.
          File.Delete(CleanDatabaseFullPath);
        }
      }
      catch (Exception)
      {
        // ignore the Exception ...
        // the file will be deleted later.
      }
    }

    protected static string RandomNonAsciiString(int lenght)
    {
      const string chars = "ქართულიენისშესწავლადასწავლებაเรียนและสอนภาษา말배우기와가르치기";
      var random = new Random(Guid.NewGuid().GetHashCode());
      var result = new string(
          Enumerable.Repeat(chars, lenght)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
      return result;
    }

    public static string RandomString(int length)
    {
      const string chars = "abcdefghejklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      var random = new Random(Guid.NewGuid().GetHashCode());
      var result = new string(
          Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
      return result;
    }

    public static uint RandomId()
    {
      var random = new Random(Guid.NewGuid().GetHashCode());
      var result = (uint)random.Next();
      return result;
    }

    public static int RandomInt()
    {
      var random = new Random(Guid.NewGuid().GetHashCode());
      return random.Next();
    }

    public static string RandomStringWithSpaces(int wordcount)
    {
      var randomWordCount = new Random(Guid.NewGuid().GetHashCode());
      var randomWordChars = new Random(Guid.NewGuid().GetHashCode());
      string result = RandomString( randomWordCount.Next( 4, 10) );
      for (var i = 0; i < wordcount; ++i)
      {
        result += new string(
          Enumerable.Repeat("\t\n ", 1)
            .Select(s => s[randomWordChars.Next(s.Length)])
            .ToArray());
        result += RandomString(randomWordCount.Next(4, 10));
      }
      return result;
    }
  }
}
