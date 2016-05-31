using System;
using System.Linq;
using OneTrueError.Client.Converters;
using Xunit;
using Xunit.Sdk;

namespace OneTrueError.Client.Tests
{
    public class ObjectToContextCollectionTests
    {
        [Fact]
        public void should_be_able_to_serialize_all_types_of_exceptions()
        {
            string[] ignoredExceptions = new string[]
            {
                /*"UpaException", "EventLogException", "EventLogNotFoundException", "PrivilegeNotHeldException",
                "ContractExceptionUnknown", "ContractExceptionUnknown"*/
            };


            var sut = new ObjectToContextCollectionConverter();

            var inner = new Exception("hello");
            var exceptionTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes().Where(y => typeof(Exception).IsAssignableFrom(y)));

            foreach (var exceptionType in exceptionTypes)
            {
                if (exceptionType.Namespace.StartsWith("Xunit") || ignoredExceptions.Contains(exceptionType.Name))
                    continue;

                var constructor = exceptionType.GetConstructor(new[] { typeof(string), typeof(Exception) });
                if (constructor != null)
                {
                    Exception thrownException = null;
                    try
                    {
                        TestOne(() =>
                        {
                            thrownException = (Exception)Activator.CreateInstance(exceptionType, new object[] { "Hello world", inner });
                            throw thrownException;
                        });
                    }
                    catch (Exception ex)
                    {
                        var item = sut.Convert(ex);
                        if (item.Items.ContainsKey("Error"))
                            Console.WriteLine(ex.GetType().FullName + ": " + item.Items["Error"]);
                        else if (item.Items["Message"] != thrownException.Message)
                        {
                            Console.WriteLine(exceptionType.FullName + ": Failed to check " + item.Items["Message"]);
                            throw new AssertException("Failed to serialize message for " + thrownException);
                        }
                    }

                }
                else
                    Console.WriteLine(exceptionType + ": Unknown constructor ");
            }
        }

        [Fact]
        public void TestEveryTypeWithDefaultConstructor()
        {

            var sut = new ObjectToContextCollectionConverter();

            var inner = new Exception("hello");
            var types =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes());

            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsGenericType)
                    continue;
                if (type.Namespace == "System.Threading.Tasks.Task")
                    continue;

                var constructor = type.GetConstructor(new Type[0]);
                if (constructor != null)
                {
                    try
                    {
                        var e = Activator.CreateInstance(type, null);
                        var t = sut.Convert(e);
                        Console.WriteLine(t);
                    }
                    catch{}

                }
            }
        }

        [Fact]
        public void MEdiaPortalArgumentException()
        {
            try
            {
                TestOne(() => { throw new ArgumentNullException("pluginRuntime"); });
            }
            catch (Exception ex)
            {
                var sut = new ObjectToContextCollectionConverter();
                var item = sut.Convert(ex);
            }
        }

        public void TestOne(Action x)
        {
            TestOne2(x);
        }

        public void TestOne2(Action x)
        {
            TestOne3(x);
        }



        public void TestOne3(Action x)
        {
            TestOne4(x);
        }


        public void TestOne4(Action x)
        {
            x();
        }

    }
}
