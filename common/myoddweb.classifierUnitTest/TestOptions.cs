using System;
using myoddweb.classifier.core;
using myoddweb.classifier.interfaces;
using Moq;
using NUnit.Framework;

namespace myoddweb.classifierUnitTest
{
  [TestFixture]
  public class TestOptions
  {
    [Test]
    public void ctor_configCannotBeNull()
    {
      Assert.Throws<ArgumentNullException>(() => new Options(null) );
    }

    protected Mock<IConfig> CreateDefaultMock()
    {
      var config = new Mock<IConfig>();
      config.Setup(c => c.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>())).Returns(
        (string configName, string defaultValue) => defaultValue
        );
      return config;
    }

    protected Mock<IConfig> CreateDefaultMockWithReturn( string value )
    {
      var config = new Mock<IConfig>();
      config.Setup(c => c.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>())).Returns(
        (string configName, string defaultValue) => value
        );
      return config;
    }

    protected Mock<IConfig> CreateSetMock(string value)
    {
      var config = new Mock<IConfig>();
      config.Setup(c => c.SetConfig(It.IsAny<string>(), It.IsAny<string>())).Returns(
        (string configName, string defaultValue) => true
        );
      return config;
    }

    [Test]
    public void MinPercentage_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options( config.Object );
      Assert.That( options.MinPercentage == (uint)DefaultOptions.MinPercentage );

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.MinPercentage", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("10")]
    [TestCase("100")]
    [TestCase("200")]
    public void MinPercentage_SetValue(string value)
    {
      var config = CreateSetMock(value);
      var options = new Options(config.Object)
      {
        MinPercentage = Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.MinPercentage", value), Times.Once);
    }

    [Test]
    public void ClassifyDelaySeconds_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.ClassifyDelaySeconds == (uint)DefaultOptions.ClassifyDelaySeconds);

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.ClassifyDelaySeconds", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("10")]
    [TestCase("100")]
    [TestCase("200")]
    public void ClassifyDelaySeconds_SetValue(string value)
    {
      var config = CreateSetMock( value );
      var options = new Options(config.Object)
      {
        ClassifyDelaySeconds = Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.ClassifyDelaySeconds", value ), Times.Once);
    }

    [Test]
    [TestCase("10")]
    [TestCase("100")]
    [TestCase("200")]
    public void ClassifyDelaySeconds_GetConfigValue( string value)
    {
      var config = CreateDefaultMockWithReturn( value );
      var options = new Options(config.Object);
      Assert.That(options.ClassifyDelaySeconds == Convert.ToUInt32(value));

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.ClassifyDelaySeconds", It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void ClassifyDelayMilliseconds_GetDefaultFromClassifyDelaySeconds()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.ClassifyDelayMilliseconds == (uint)DefaultOptions.ClassifyDelaySeconds*1000);

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.ClassifyDelaySeconds", It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void ReCheckCategories_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.ReCheckCategories == ((uint)DefaultOptions.ReCheckCategories == 1));

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.ReCheckCategories", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("1")]
    [TestCase("0")]
    public void ReCheckCategories_SetValue(string value)
    {
      var config = CreateSetMock(value);
      var options = new Options(config.Object)
      {
        ReCheckCategories = 1 == Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.ReCheckCategories", value), Times.Once);
    }

    [Test]
    public void ReAutomaticallyTrainMagnetMessages_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.ReAutomaticallyTrainMagnetMessages == ((uint)DefaultOptions.ReAutomaticallyTrainMagnetMessages == 1));

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.ReAutomaticallyTrainMagnetMessages", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("1")]
    [TestCase("0")]
    public void ReAutomaticallyTrainMagnetMessages_SetValue(string value)
    {
      var config = CreateSetMock(value);
      var options = new Options(config.Object)
      {
        ReAutomaticallyTrainMagnetMessages = 1 == Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.ReAutomaticallyTrainMagnetMessages", value), Times.Once);
    }

    [Test]
    public void ReAutomaticallyTrainMessages_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.ReAutomaticallyTrainMessages == ((uint)DefaultOptions.ReAutomaticallyTrainMessages == 1));

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.ReAutomaticallyTrainMessages", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("1")]
    [TestCase("0")]
    public void ReAutomaticallyTrainMessages_SetValue(string value)
    {
      var config = CreateSetMock(value);
      var options = new Options(config.Object)
      {
        ReAutomaticallyTrainMessages = 1 == Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.ReAutomaticallyTrainMessages", value), Times.Once);
    }

    [Test]
    public void ReCheckIfCtrlKeyIsDown_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.ReCheckIfCtrlKeyIsDown == ((uint)DefaultOptions.ReCheckIfCtrlKeyIsDown == 1));

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.ReCheckIfCtrlKeyIsDown", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("1")]
    [TestCase("0")]
    public void ReCheckIfCtrlKeyIsDown_SetValue(string value)
    {
      var config = CreateSetMock(value);
      var options = new Options(config.Object)
      {
        ReCheckIfCtrlKeyIsDown = 1 == Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.ReCheckIfCtrlKeyIsDown", value), Times.Once);
    }

    [Test]
    public void MagnetsWeight_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.MagnetsWeight == (uint)DefaultOptions.MagnetsWeight );

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.MagnetsWeight", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("10")]
    [TestCase("100")]
    [TestCase("50")]
    public void MagnetsWeight_SetConfigValue(string value)
    {
      var config = CreateSetMock(value);
      var options = new Options(config.Object)
      {
        MagnetsWeight = Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.MagnetsWeight", value), Times.Once);
    }

    [Test]
    public void UserWeight_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.UserWeight == (uint)DefaultOptions.UserWeight);

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.UserWeight", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("10")]
    [TestCase("100")]
    [TestCase("50")]
    public void UserWeight_SetConfigValue(string value)
    {
      var config = CreateSetMock(value);
      var options = new Options(config.Object)
      {
        UserWeight = Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.UserWeight", value), Times.Once);
    }
    
    [Test]
    public void LogRetention_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.LogRetention == (uint)DefaultOptions.LogRetention);

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.LogRetention", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("10")]
    [TestCase("100")]
    [TestCase("50")]
    public void LogRetention_SetConfigValue(string value)
    {
      var config = CreateSetMock(value);
      var options = new Options(config.Object)
      {
        LogRetention = Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.LogRetention", value), Times.Once);
    }

    [Test]
    public void LogDisplaySize_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.LogDisplaySize == (uint)DefaultOptions.LogDisplaySize);

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.LogDisplaySize", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("10")]
    [TestCase("100")]
    [TestCase("50")]
    public void LogDisplaySize_SetConfigValue(string value)
    {
      var config = CreateSetMock(value);
      var options = new Options(config.Object)
      {
        LogDisplaySize = Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.LogDisplaySize", value), Times.Once);
    }

    [Test]
    public void LogLevel_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.LogLevel == (LogLevels)DefaultOptions.LogLevel);

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.LogLevels", It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void CheckUnProcessedEmailsOnStartUp_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.CheckUnProcessedEmailsOnStartUp == ((uint)DefaultOptions.CheckUnProcessedEmailsOnStartUp == 1 ));

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.CheckUnProcessedEmailsOnStartUp", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("1")]
    [TestCase("0")]
    public void CheckUnProcessedEmailsOnStartUp_SetValue(string value)
    {
      var config = CreateSetMock(value);
      var options = new Options(config.Object)
      {
        CheckUnProcessedEmailsOnStartUp = 1 == Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.CheckUnProcessedEmailsOnStartUp", value), Times.Once);
    }

    [Test]
    public void CheckIfUnKnownCategory_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.CheckIfUnKnownCategory == ((uint)DefaultOptions.CheckIfUnKnownCategory == 1));

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.CheckIfUnKnownCategory", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("1")]
    [TestCase("0")]
    public void CheckIfUnKnownCategory_SetValue(string value)
    {
      var config = CreateSetMock(value);
      var options = new Options(config.Object)
      {
        CheckIfUnKnownCategory = 1 == Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.CheckIfUnKnownCategory", value), Times.Once);
    }

    [Test]
    public void CommonWordsMinPercent_GetDefault()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.That(options.CommonWordsMinPercent == (uint)DefaultOptions.CommonWordsMinPercent);

      // check that we only called the function once
      config.Verify(h => h.GetConfigWithDefault(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.GetConfigWithDefault("Option.CommonWordsMinPercent", It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCase("10")]
    [TestCase("100")]
    [TestCase("50")]
    public void CommonWordsMinPercent_SetConfigValue(string value)
    {
      var config = CreateSetMock(value);
      var options = new Options(config.Object)
      {
        CommonWordsMinPercent = Convert.ToUInt32(value)
      };

      // check that we only called the function once
      config.Verify(h => h.SetConfig(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      // and that the correct param was passed.
      config.Verify(h => h.SetConfig("Option.CommonWordsMinPercent", value), Times.Once);
    }
    
    [Test]
    public void CanLog_None()
    {
      var config = CreateDefaultMock();
      var options = new Options(config.Object);
      Assert.False(options.CanLog( LogLevels.None));
    }

    [Test]
    [TestCase(LogLevels.Error, LogLevels.Error, true )]           // [Error] only up to error
    [TestCase(LogLevels.Error, LogLevels.Warning, false)]
    [TestCase(LogLevels.Error, LogLevels.Information, false)]
    [TestCase(LogLevels.Error, LogLevels.Verbose, false)]
    [TestCase(LogLevels.Warning, LogLevels.Error, true)]          // [Warning] up to warning
    [TestCase(LogLevels.Warning, LogLevels.Warning, true)]
    [TestCase(LogLevels.Warning, LogLevels.Information, false)]
    [TestCase(LogLevels.Warning, LogLevels.Verbose, false)]
    [TestCase(LogLevels.Information, LogLevels.Error, true)]      // [Information] up to information
    [TestCase(LogLevels.Information, LogLevels.Warning, true)]
    [TestCase(LogLevels.Information, LogLevels.Information, true)]
    [TestCase(LogLevels.Information, LogLevels.Verbose, false)]
    [TestCase(LogLevels.Verbose, LogLevels.Error, true)]          // [Verbose] all cases
    [TestCase(LogLevels.Verbose, LogLevels.Warning, true)]
    [TestCase(LogLevels.Verbose, LogLevels.Information, true)]
    [TestCase(LogLevels.Verbose, LogLevels.Verbose, true)]
    public void CanLog( LogLevels current, LogLevels check, bool expected )
    {
      var config = CreateDefaultMockWithReturn( Convert.ToString((uint)current));
      var options = new Options(config.Object);
      Assert.That( options.CanLog(check) == expected );

      config.Verify(h => h.GetConfigWithDefault("Option.LogLevels", It.IsAny<string>()), Times.Once);
    }
  }
}
