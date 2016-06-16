using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Security.Permissions;
using System.Web;
using Xunit;
using Xunit.Sdk;

namespace OneTrueError.Client.Tests
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PartialTrustRunnerAttribute : RunWithAttribute
    {
        public PartialTrustRunnerAttribute()
            : base(typeof (PartialTrustTestClassCommand))
        {
        }

        private class PartialTrustTestClassCommand : ITestClassCommand
        {
            // Delegate most of the work to the existing TestClassCommand class so that we
            // can preserve any existing behavior (like supporting IUseFixture<T>).
            private readonly TestClassCommand originalTestClassCommand = new TestClassCommand();
            private AppDomain sandbox;

            public int ChooseNextTest(ICollection<IMethodInfo> testsLeftToRun)
            {
                return originalTestClassCommand.ChooseNextTest(testsLeftToRun);
            }

            public Exception ClassFinish()
            {
                var result = originalTestClassCommand.ClassFinish();
                if (sandbox != null)
                {
                    AppDomain.Unload(sandbox);
                    sandbox = null;
                }

                return result;
            }

            public Exception ClassStart()
            {
                GuardTypeUnderTest();
                var xunitAssembly = typeof (FactAttribute).Assembly;
                sandbox = CreatePartialTrustAppDomain();

                return originalTestClassCommand.ClassStart();
            }

            public IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo testMethod)
            {
                return originalTestClassCommand.EnumerateTestCommands(testMethod);
            }

            public IEnumerable<IMethodInfo> EnumerateTestMethods()
            {
                return originalTestClassCommand.EnumerateTestMethods();
            }

            public bool IsTestMethod(IMethodInfo testMethod)
            {
                return originalTestClassCommand.IsTestMethod(testMethod);
            }

            public object ObjectUnderTest
            {
                get
                {
                    return sandbox.CreateInstanceAndUnwrap(TypeUnderTest.Type.Assembly.FullName,
                        TypeUnderTest.Type.FullName);
                }
            }

            public ITypeInfo TypeUnderTest
            {
                get { return originalTestClassCommand.TypeUnderTest; }
                set
                {
                    if (!typeof (MarshalByRefObject).IsAssignableFrom(value.Type))
                    {
                        throw new InvalidOperationException(
                            "Test types to be run in PT must derive from MarshalByRefObject");
                    }
                    if (value == null)
                        throw new ArgumentNullException("value");

                    originalTestClassCommand.TypeUnderTest = value;
                }
            }

            private static AppDomain CreatePartialTrustAppDomain()
            {
                var permissions = new PermissionSet(PermissionState.None);
                permissions.AddPermission(new AspNetHostingPermission(AspNetHostingPermissionLevel.Medium));
                permissions.AddPermission(new DnsPermission(PermissionState.Unrestricted));
                permissions.AddPermission(new EnvironmentPermission(EnvironmentPermissionAccess.Read,
                    "TEMP;TMP;USERNAME;OS;COMPUTERNAME"));
                permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess,
                    AppDomain.CurrentDomain.BaseDirectory));
                permissions.AddPermission(new IsolatedStorageFilePermission(PermissionState.None)
                {
                    UsageAllowed = IsolatedStorageContainment.AssemblyIsolationByUser,
                    UserQuota = Int64.MaxValue
                });
                permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
                permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlThread));
                permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlPrincipal));
                permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.RemotingConfiguration));
                permissions.AddPermission(new SmtpPermission(SmtpAccess.Connect));
                permissions.AddPermission(new SqlClientPermission(PermissionState.Unrestricted));
                permissions.AddPermission(new TypeDescriptorPermission(PermissionState.Unrestricted));
                permissions.AddPermission(new WebPermission(PermissionState.Unrestricted));
                permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess));

                var setup = new AppDomainSetup() {ApplicationBase = AppDomain.CurrentDomain.BaseDirectory};

                setup.PartialTrustVisibleAssemblies = new string[]
                {
                    "System.Web, PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293",
                    "System.Web.Extensions, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9",
                    "System.Web.Abstractions, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9",
                    "System.Web.Routing, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9",
                    "System.ComponentModel.DataAnnotations, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9",
                    "System.Web.DynamicData, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9",
                    "System.Web.DataVisualization, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9",
                    "System.Web.ApplicationServices, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9"
                };


                return AppDomain.CreateDomain("Partial Trust Sandbox", null, setup, permissions);
            }

            private void GuardTypeUnderTest()
            {
                if (TypeUnderTest == null)
                {
                    throw new InvalidOperationException("Forgot to set TypeUnderTest before calling ObjectUnderTest");
                }
            }
        }
    }
}