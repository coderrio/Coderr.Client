using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace OneTrueError.Client.Tests
{
    /// <summary>
    /// http://stackoverflow.com/questions/987332/how-to-automate-testing-of-medium-trust-code
    /// </summary>
    internal class PartialTrustTest : MarshalByRefObject
    {
        public void PartialTrustSuccess()
        {
            Console.WriteLine("partial trust success #1");
        }

        public void PartialTrustFailure()
        {
            FieldInfo fi = typeof(Int32).GetField("m_value", BindingFlags.Instance | BindingFlags.NonPublic);
            object value = fi.GetValue(1);
            Console.WriteLine("value: {0}", value);
        }

        private static AppDomain CreatePartialTrustDomain()
        {
            AppDomainSetup setup = new AppDomainSetup() { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory };
            PermissionSet permissions = new PermissionSet(null);
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess));
            return AppDomain.CreateDomain("Partial Trust AppDomain", null, setup, permissions);
        }

        static void Main(string[] args)
        {
            AppDomain appDomain = CreatePartialTrustDomain();

            PartialTrustTest partialTrustTest = (PartialTrustTest)appDomain.CreateInstanceAndUnwrap(
                typeof(PartialTrustTest).Assembly.FullName,
                typeof(PartialTrustTest).FullName);

            partialTrustTest.PartialTrustSuccess();

            try
            {
                partialTrustTest.PartialTrustFailure();
                Console.Error.WriteLine("!!! partial trust test failed");
            }
            catch (FieldAccessException)
            {
                Console.WriteLine("partial trust success #2");
            }
        }
    }
}