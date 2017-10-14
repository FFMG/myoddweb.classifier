namespace myoddweb.classifier.interfaces
{
  public enum DefaultOptions
  {
    UserWeight = 10,
    MagnetsWeight = 2,
    CommonWordsMinPercent = 50,
    MinPercentage = 75,
    LogLevel = LogLevels.Error,
    LogRetention = 30,
    LogDisplaySize = 100,
    ClassifyDelaySeconds = 1
  }

  public interface IOptions
  {
    /// <summary>
    /// Check if we can log given a certain log level.
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    bool CanLog(LogLevels level);

    uint CommonWordsMinPercent { get; set; }

    /// <summary>
    /// Check the category only if we don't currently have a valid value.
    /// The default is to get the value if the value is not known.
    /// </summary>
    bool CheckIfUnKnownCategory { get; set; }

    /// <summary>
    /// Mainly used for exchange, when outlook starts we want to check unprocessed emails.
    /// In some cases emails are 'received' even if outlook is not running.
    /// </summary>
    bool CheckUnProcessedEmailsOnStartUp { get; set; }

    /// <summary>
    /// Get or set the log level
    /// </summary>
    LogLevels LogLevel { get; set; }

    /// <summary>
    /// Get the number of log entries we want to display
    /// </summary>
    uint LogDisplaySize { get; set; }

    /// <summary>
    /// Get or set the log retention policy
    /// </summary>
    uint LogRetention { get; set; }

    /// <summary>
    /// Set the user weight multiplier.
    /// </summary>
    uint UserWeight { get; set; }

    /// <summary>
    /// Set the magnet weight multiplier.
    /// </summary>
    uint MagnetsWeight { get; set; }

    /// <summary>
    /// (re) Check all the categories if the control key is pressed down.
    /// This is on by default as the control key has to be pressed.
    /// </summary>
    bool ReCheckIfCtrlKeyIsDown { get; set; }

    /// <summary>
    /// Check if we want to automaticlly train messages when they arrive.
    /// We categorise messages, but do we also want to train them?
    /// By default we don't do that...
    /// </summary>
    bool ReAutomaticallyTrainMessages { get; set; }

    /// <summary>
    /// Check if we want to train the message because we used a magnet.
    /// </summary>
    bool ReAutomaticallyTrainMagnetMessages { get; set; }

    /// <summary>
    /// (re) Check all the categories all the time.
    /// This is on by default as we have the other default option "CheckIfUnKnownCategory" also set to on.
    /// The net effect of that would be to only check if we don't already know the value.
    /// </summary>
    bool ReCheckCategories { get; set; }

    /// <summary>
    /// The number of seconds we want to wait before we classify a message.
    /// This delay is needed to let outlook apply its own rules.
    /// </summary>
    uint ClassifyDelaySeconds { get; set; }

    /// <summary>
    /// Get the classification delay in milliseconds.
    /// </summary>
    uint ClassifyDelayMilliseconds { get; }

    /// <summary>
    /// (re) Check all the categories all the time.
    /// This is on by default as we have the other default option "CheckIfUnKnownCategory" also set to on.
    /// The net effect of that would be to only check if we don't already know the value.
    /// </summary>
    uint MinPercentage { get; set; }
  }
}
