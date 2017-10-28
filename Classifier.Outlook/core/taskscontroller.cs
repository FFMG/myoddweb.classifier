using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace myoddweb.classifier.core
{
  internal class TasksController : IDisposable
  {
    /// <summary>
    /// The one and only controller.
    /// </summary>
    private static TasksController _controller;

    /// <summary>
    /// Create the controller when needed.
    /// </summary>
    private static TasksController Controller => _controller ?? (_controller = new TasksController());

    /// <summary>
    /// Our lock
    /// </summary>
    private readonly object _lock = new object();

    // all the ongoing tasks.
    private readonly List<Task> _tasks;

    private TasksController()
    {
      _tasks = new List<Task>();
    }

    public void Dispose()
    {
      WaitAll();
    }

    /// <summary>
    /// Add a task to our list
    /// </summary>
    /// <param name="run"></param>
    private void _Add( Task run )
    {
      //  remove all the completed tasks
      RemoveAllCompleted();

      // and add the new task
      lock (_lock)
      {
        _tasks.Add(run);
      }
    }

    /// <summary>
    /// Wait for all the tasks to complete.
    /// </summary>
    private void _WaitAll()
    {
      lock (_lock)
      {
        Task.WaitAll(_tasks.ToArray());
      }
      RemoveAllCompleted();
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
    public static void WaitAll()
    {
      Controller._WaitAll();
    }

    /// <summary>
    /// Add a task to our list
    /// </summary>
    /// <param name="run"></param>
    public static void Add(Task run)
    {
      Controller._Add( run );
    }
  }
}
