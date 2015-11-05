using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using myoddweb.classifier.core;

namespace myoddweb.classifierUnitTest
{
  [TestClass]
  public class TestSqlUniqueIds : TestCommon
  {
    public TestSqlUniqueIds()
    {
      ReleaseEngine(false);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      ReleaseEngine(true);
    }

    [TestMethod]
    public void TestGetCategoryFromUniqueIdNoUpdates()
    {
      //  create a couple of categories
      var catNames = new List<string>()
      {
        RandomString(10),
        RandomString(10),
        RandomString(10),
        RandomString(10)
      };

      // create all the categories.
      catNames.Select(catName => TheEngine.GetCategory(catName)).ToList();

      // use a dictionary for all the unqique items.
      var uniqueIdentifiers = new Dictionary< string, string >();

      // get a random category
      var random = new Random(Guid.NewGuid().GetHashCode());
      for (var i = 0; i < (10 + random.Next(20)); ++i)
      {
        random = new Random(Guid.NewGuid().GetHashCode());
        var catName = catNames[ random.Next(catNames.Count) ];

        var uniqueIdentifier = RandomString(100);
        var textToCategorise = RandomStringWithSpaces(10);

        // add to the list.
        uniqueIdentifiers.Add( uniqueIdentifier, catName );

        TheEngine.Train(catName, uniqueIdentifier, textToCategorise);
      }

      // check that all of them are valid.
      foreach (var uniqueIdentifier in uniqueIdentifiers)
      {
        var thisCategoryId = TheEngine.GetCategoryFromUniqueId( uniqueIdentifier.Key );
        Assert.AreEqual(thisCategoryId, TheEngine.GetCategory(uniqueIdentifier.Value));
      }
    }

    [TestMethod]
    public void TestGetCategoryFromUniqueIdWithUpdates()
    {
      //  create a couple of categories
      var catNames = new List<string>()
      {
        RandomString(10),
        RandomString(10),
        RandomString(10),
        RandomString(10)
      };

      // create all the categories.
      catNames.Select(catName => TheEngine.GetCategory(catName)).ToList();

      // use a dictionary for all the unqique items.
      var uniqueIdentifiers = new Dictionary<string, string>();

      // get a random category
      var random = new Random(Guid.NewGuid().GetHashCode());
      for (var i = 0; i < (10 + random.Next(20)); ++i)
      {
        random = new Random(Guid.NewGuid().GetHashCode());
        var catName = catNames[random.Next(catNames.Count)];

        var uniqueIdentifier = RandomString(100);
        var textToCategorise = RandomStringWithSpaces(10);

        // add to the list.
        uniqueIdentifiers.Add(uniqueIdentifier, catName);

        // train it to one id
        TheEngine.Train(catName, uniqueIdentifier, textToCategorise);

        // then train it to another.
        var anotherCatName = catNames[random.Next(catNames.Count)];

        // update the list.
        uniqueIdentifiers[uniqueIdentifier] = anotherCatName;

        TheEngine.Train(anotherCatName, uniqueIdentifier, textToCategorise);
      }

      // make sure that everything has been updated.
      foreach (var uniqueIdentifier in uniqueIdentifiers)
      {
        var thisCategoryId = TheEngine.GetCategoryFromUniqueId(uniqueIdentifier.Key);
        Assert.AreEqual(thisCategoryId, TheEngine.GetCategory(uniqueIdentifier.Value));
      }
    }
  }
}
