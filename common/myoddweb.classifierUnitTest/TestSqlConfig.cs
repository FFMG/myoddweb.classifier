using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace myoddweb.classifierUnitTest
{
  [TestClass]
  public class TestSqlConfig : TestCommon
  {

    [ClassCleanup]
    public static void ClassCleanup()
    {
      ReleaseEngine(true);
    }

    [TestCleanup]
    public void TestCleanup()
    {
      ReleaseEngine( false );
    }

    [TestMethod()]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void ConfigvalueDoesNotExist()
    {
      // brand new value.
      TheEngine.GetConfig( RandomString(8) );
    }

    [TestMethod]
    public void ConfigvalueWithDefaultDoesNotExist()
    {
      // brand new value.
      var defaultValue = RandomString(8);
      var returnValue = TheEngine.GetConfigWithDefault(RandomString(8), defaultValue );
      Assert.AreEqual(defaultValue, returnValue);
    }

    [TestMethod]
    public void ConfigvalueWithDefaultDoesExist()
    {
      // brand new value.
      var configName = RandomString(8);
      var defaultValue = RandomString(8);
      var savedValue = RandomString(8);
      Assert.IsTrue(TheEngine.SetConfig(configName, savedValue));

      var returnValue = TheEngine.GetConfigWithDefault( configName , defaultValue);
      Assert.AreEqual(savedValue, returnValue);
    }

    [TestMethod]
    public void MethodSimpleSetAndGetConfig()
    {
      // simple save and load config.
      var u16Value = RandomString(16);
      var u16Name = RandomString(8);

      Assert.IsTrue(TheEngine.SetConfig(u16Name, u16Value ) );
      Assert.AreEqual(u16Value, TheEngine.GetConfig(u16Name));
    }

    [TestMethod]
    public void MethodSimpleGetConfigWithSpacesInName()
    {
      // simple save and load config.
      var u16Value = RandomString(16);
      var u16Name = RandomString(8);

      Assert.IsTrue(TheEngine.SetConfig( $"   {u16Name}    ", u16Value));
      Assert.AreEqual(u16Value, TheEngine.GetConfig(u16Name));
    }

    [TestMethod]
    public void MethodSimpleSetConfigWithSpacesInName()
    {
      // simple save and load config.
      var u16Value = RandomString(16);
      var u16Name = RandomString(8);

      Assert.IsTrue(TheEngine.SetConfig( u16Name, u16Value));
      Assert.AreEqual(u16Value, TheEngine.GetConfig($"   {u16Name}    "));
    }

    [TestMethod]
    public void EmptyConfigValue()
    {
      // simple save and load config.
      Assert.IsFalse(TheEngine.SetConfig("", RandomString(8)));
    }

    [TestMethod]
    public void EmptyConfigValueWithSpaces()
    {
      // simple save and load config.
      Assert.IsFalse(TheEngine.SetConfig("        ", RandomString(8)));
    }
	
    [TestMethod]
    public void SetConfigurationWithNonAsciiCharacters()
    {
      // simple save and load config.
      var u16Value = RandomNonAsciiString(16);
      var u16Name = RandomString(8);
      Assert.IsTrue(TheEngine.SetConfig(u16Name, u16Value));
      Assert.AreEqual( u16Value, TheEngine.GetConfig(u16Name));
    }

    [TestMethod]
    public void SetConfigurationNameWithNonAsciiCharacters()
    {
      // simple save and load config.
      var u16Value = RandomString(16);
      var u16Name = RandomNonAsciiString(8);
      Assert.IsTrue(TheEngine.SetConfig(u16Name, u16Value));
      Assert.AreEqual(u16Value, TheEngine.GetConfig(u16Name));
    }

    [TestMethod]
    public void SetConfigurationNameAndValueWithNonAsciiCharacters()
    {
      // simple save and load config.
      var u16Value = RandomNonAsciiString(16);
      var u16Name = RandomNonAsciiString(8);
      Assert.IsTrue(TheEngine.SetConfig(u16Name, u16Value));
      Assert.AreEqual(u16Value, TheEngine.GetConfig(u16Name));
    }
  }
}
