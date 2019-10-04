using System;
using System.Diagnostics;
using System.Reflection;

namespace myoddweb.classifier.utils
{
  public static class TypeLoader
  {
    public static T LoadTypeFromAssembly<T>(string assemblyFilePath) where T : class
    {
      var assembly = Assembly.LoadFrom(assemblyFilePath);
      return LoadTypeFromAssembly<T>(assembly);
    }

    public static T LoadTypeFromAssembly<T>(Assembly assembly) where T : class
    {
      var exportedTypes = assembly.GetExportedTypes();
      foreach (var t in exportedTypes)
      {
        // When coming from different assemblies the types don't match, this is very fragile.
        //if( typeof( T ).IsAssignableFrom( t ) )
        if (t.GetInterface(typeof(T).FullName ?? throw new InvalidOperationException(), true) == null)
        {
          continue;
        }
        try
        {
          Debug.WriteLine($"Trying to create instance of {t.FullName}...");
          return assembly.CreateInstance(t.FullName ?? throw new InvalidOperationException()) as T;
        }
        catch (TargetInvocationException e)
        {
          if (!(e.InnerException is DllNotFoundException dllNotFoundException))
          {
            throw;
          }

          Debug.WriteLine(
            "A DllNotFoundException was thrown during the attempt to create a type instance. Are you missing some DLL dependencies?");
          throw dllNotFoundException;

        }
      }
      return null;
    }
  }
}
