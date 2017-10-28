using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace myoddweb.classifier.core
{
  class TasksController : IDisposable
  {
    /// <summary>
    /// Our lock
    /// </summary>
    private object _lock = new object();

    // all the ongoing tasks.
    private List<Task> _tasks;

    public TasksController()
    {
      _tasks = new List<Task>();
    }

    public void Dispose()
    {
      WaitAll();
    }

    /// <summary>
    /// Remove all the completed tasks.
    /// </summary>
    private void RemoveAllCompleted()
    {
      lock(_lock)
      {
        _tasks.RemoveAll(t => t.IsCompleted);
      }
    }

    /// <summary>
    /// Wait for all the tasks to complete.
    /// </summary>
    public void WaitAll()
    {
      lock (_lock)
      {
        Task.WaitAll(_tasks.ToArray());
      }
      RemoveAllCompleted();
    }

    /// <summary>
    /// Add a task to our list
    /// </summary>
    /// <param name="run"></param>
    public void Add(Task run)
    {
      //  remove all the completed tasks
      RemoveAllCompleted();

      // and add the new task
      lock (_lock)
      {
        _tasks.Add(run);
      }
    }
  }
}
