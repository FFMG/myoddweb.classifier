using System.Collections.Generic;
using NUnit.Framework;

namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  public class TestSqlConfig : TestCommon
  {
    [OneTimeTearDown]
    public void ClassCleanup()
    {
      ReleaseEngine(true);
    }

    [TearDown]
    public void TestCleanup()
    {
      ReleaseEngine( false );
    }

    [Test]
    public void ConfigvalueDoesNotExist()
    {
      // brand new value.
      Assert.Throws<KeyNotFoundException>(() => TheEngine.GetConfig( RandomString(8) ));
    }

    [Test]
    public void ConfigvalueWithDefaultDoesNotExist()
    {
      // brand new value.
      var defaultValue = RandomString(8);
      var returnValue = TheEngine.GetConfigWithDefault(RandomString(8), defaultValue );
      Assert.AreEqual(defaultValue, returnValue);
    }

    [Test]
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

    [Test]
    public void MethodSimpleSetAndGetConfig()
    {
      // simple save and load config.
      var u16Value = RandomString(16);
      var u16Name = RandomString(8);

      Assert.IsTrue(TheEngine.SetConfig(u16Name, u16Value ) );
      Assert.AreEqual(u16Value, TheEngine.GetConfig(u16Name));
    }

    [Test]
    public void MethodSimpleGetConfigWithSpacesInName()
    {
      // simple save and load config.
      var u16Value = RandomString(16);
      var u16Name = RandomString(8);

      Assert.IsTrue(TheEngine.SetConfig( $"   {u16Name}    ", u16Value));
      Assert.AreEqual(u16Value, TheEngine.GetConfig(u16Name));
    }

    [Test]
    public void MethodSimpleSetConfigWithSpacesInName()
    {
      // simple save and load config.
      var u16Value = RandomString(16);
      var u16Name = RandomString(8);

      Assert.IsTrue(TheEngine.SetConfig( u16Name, u16Value));
      Assert.AreEqual(u16Value, TheEngine.GetConfig($"   {u16Name}    "));
    }

    [Test]
    public void EmptyConfigValue()
    {
      // simple save and load config.
      Assert.IsFalse(TheEngine.SetConfig("", RandomString(8)));
    }

    [Test]
    public void EmptyConfigValueWithSpaces()
    {
      // simple save and load config.
      Assert.IsFalse(TheEngine.SetConfig("        ", RandomString(8)));
    }
	
    [Test]
    public void SetConfigurationWithNonAsciiCharacters()
    {
      // simple save and load config.
      var u16Value = RandomNonAsciiString(16);
      var u16Name = RandomString(8);
      Assert.IsTrue(TheEngine.SetConfig(u16Name, u16Value));
      Assert.AreEqual( u16Value, TheEngine.GetConfig(u16Name));
    }

    [Test]
    public void SetConfigurationNameWithNonAsciiCharacters()
    {
      // simple save and load config.
      var u16Value = RandomString(16);
      var u16Name = RandomNonAsciiString(8);
      Assert.IsTrue(TheEngine.SetConfig(u16Name, u16Value));
      Assert.AreEqual(u16Value, TheEngine.GetConfig(u16Name));
    }

    [Test]
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
