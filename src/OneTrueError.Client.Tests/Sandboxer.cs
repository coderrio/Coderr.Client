using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace OneTrueError.Client.Tests
{
    /// <summary>
    ///     From http://msdn.microsoft.com/en-us/library/bb763046.aspx
    /// </summary>
    internal class Sandboxer : MarshalByRefObject
    {
        private static Object[] parameters = {45};
        private readonly string _untrustedAssembly;
        private readonly string _untrustedClass;
        private string _entryMethodName;
        private string _targetPath;

        public Sandboxer(Type classToRun, string methodToRun)
        {
            _targetPath = Path.GetTempPath();
            Directory.CreateDirectory(_targetPath);
            foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory))
            {
                File.Copy(file, Path.Combine(_targetPath, Path.GetFileName(file)), true);
            }
            _untrustedAssembly = Assembly.GetExecutingAssembly().FullName;
            _untrustedClass = classToRun.FullName;
            _entryMethodName = methodToRun;
        }

        private void Run()
        {
            var adSetup = new AppDomainSetup {ApplicationBase = _targetPath};
            var permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            var newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet);
            var handle = Activator.CreateInstanceFrom(
                newDomain, typeof (Sandboxer).Assembly.ManifestModule.FullyQualifiedName,
                typeof (Sandboxer).FullName
                );
            var newDomainInstance = (Sandboxer) handle.Unwrap();
            newDomainInstance.ExecuteUntrustedCode(_untrustedAssembly, _untrustedClass, _entryMethodName, parameters);
        }

        public void ExecuteUntrustedCode(string assemblyName, string typeName, string entryPoint, Object[] parameters)
        {
            //Load the MethodInfo for a method in the new Assembly. This might be a method you know, or 
            //you can use Assembly.EntryPoint to get to the main function in an executable.
            var target = Assembly.Load(assemblyName).GetType(typeName).GetMethod(entryPoint);
            try
            {
                //Now invoke the method.
                var retVal = (bool) target.Invoke(null, parameters);
            }
            catch (Exception ex)
            {
                // When we print informations from a SecurityException extra information can be printed if we are 
                //calling it with a full-trust stack.
                (new PermissionSet(PermissionState.Unrestricted)).Assert();
                Console.WriteLine("SecurityException caught:\n{0}", ex.ToString());
                CodeAccessPermission.RevertAssert();
                Console.ReadLine();
            }
        }
    }
}