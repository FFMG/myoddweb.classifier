using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace myoddweb.classifier.interfaces
{
  public interface IMailProcessor
  {
    /// <summary>
    /// Given a mail item class name, we check if this is one we could classify.
    /// </summary>
    /// <param name="className">The classname we are checking</param>
    /// <returns>boolean if we can/could classify this mail item or not.</returns>
    bool IsUsableClassNameForClassification(string className);

    /// <summary>
    /// Get the last time we processed an email
    /// Or the current time we we don't have a valid value.
    /// </summary>
    DateTime LastProcessed { get; }

    /// <summary>
    /// Add a mail item to be moved to the folder.
    /// </summary>
    /// <param name="entryIdItem">The item we are moving.</param>
    void Add(string entryIdItem);

    /// <summary>
    /// Classify an item given an entry id.
    /// </summary>
    /// <param name="entryIdItem">The item we want to classify</param>
    /// <param name="categoryId">The category of the item</param>
    /// <param name="weight">The classification weight.</param>
    /// <returns></returns>
    Task<bool> ClassifyAsync(string entryIdItem, uint categoryId, uint weight);

    /// <summary>
    /// Add a range of mail entry ids to our list.
    /// </summary>
    /// <param name="ids"></param>
    void Add(List<string> ids);

    /// <summary>
    /// Are we currently busy with this mail item?
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    bool IsProccessing(string itemId);
  }
}
