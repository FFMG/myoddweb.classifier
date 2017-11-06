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
    private static volatile TasksController _controller;

    /// <summary>
    /// Our lock
    /// </summary>
    private static readonly object Lock = new object();

    /// <summary>
    /// Create the controller when needed.
    /// </summary>
    private static TasksController Controller
    {
      get
      {
        // check the value
        if (_controller != null)
        {
          return _controller;
        }
        lock (Lock)
        {
          // check the value again.
          if (_controller == null)
          {
            _controller = new TasksController();
          }
        }
        return _controller;
      }
    }

    /// <summary>
    /// All the tasks we are currently managing.
    /// Some might be finished.
    /// </summary>
    private readonly List<Task> _tasks;

    /// <summary>
    /// The one constructor.
    /// </summary>
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
      lock (Lock)
      {
        _tasks.Add(run);
      }
    }

    /// <summary>
    /// Wait for all the tasks to complete.
    /// </summary>
    private void _WaitAll()
    {
      lock (Lock)
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
      lock(Lock)
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
