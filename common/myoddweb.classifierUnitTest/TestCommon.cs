using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using myoddweb.classifier.core;

namespace myoddweb.classifierUnitTest
{
  [TestClass]
  public class TestCommon
  {
    // the engine
    private static Engine _engine = null;

    protected static string _dbName = "ctest_database.classifier";
    protected static string _cleandbName = "clean_ctest_database.classifier";

    protected Engine TheEngine => _engine ?? (_engine = CreateEngine());

    public static string DirectoryPath { get; private set; }
    public static string DatabaseFullPath { get; private set; }
    public static string CleanDatabaseFullPath { get; private set; }

    public static string GetDirectory()
    {
      // the paths we will be using.
      var directoryName = AppDomain.CurrentDomain.BaseDirectory;
      if (File.Exists($"{directoryName}\\classifier.engine.dll"))
      {
        return directoryName;
      }

      var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (var loadedAssembly in loadedAssemblies)
      {
        directoryName = Path.GetDirectoryName(loadedAssembly.Location);
        for (;;)
        {
          if (File.Exists($"{directoryName}\\classifier.engine.dll"))
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

    protected static Engine CreateEngine()
    {
      // the paths we will be using.
      DirectoryPath = GetDirectory();
      if (null == DirectoryPath)
      {
        throw new Exception("I could not find the current working path.");
      }

      //  the database path
      DatabaseFullPath = $"{DirectoryPath}\\{_dbName}";

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

      CleanDatabaseFullPath = $"{DirectoryPath}\\{_cleandbName}";
      if (!File.Exists(CleanDatabaseFullPath))
      {
        var e = new Engine(DirectoryPath, CleanDatabaseFullPath);
        e.Release();
      }

      // make sure that we have a clean database.
      File.Copy(CleanDatabaseFullPath, DatabaseFullPath);
      if (!File.Exists(DatabaseFullPath))
      {
        Assert.Fail( "Could not create the new db from the clean db." );
      }

      //create the engine
      return new Engine(DirectoryPath, DatabaseFullPath);
    }

    protected static void ReleaseEngine( bool removeCleanDb )
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

    protected static string RandomString(int lenght)
    {
      const string chars = "abcdefghejklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      var random = new Random(Guid.NewGuid().GetHashCode());
      var result = new string(
          Enumerable.Repeat(chars, lenght)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
      return result;
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
