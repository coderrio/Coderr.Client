using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using FluentAssertions;
using OneTrueError.Client.Converters;
using Xunit;
using Xunit.Sdk;

namespace OneTrueError.Client.Tests
{
    public class ObjectToContextCollectionTests
    {
        [Fact]
        public void should_be_able_To_convert_dictionaries_using_the_key_as_index()
        {
            var obj = new Dictionary<object, string>() { { "ada", "hello" }, { 1, "world" } };

            var sut = new ObjectToContextCollectionConverter();
            var actual = sut.Convert(obj);

            actual.Properties["ada"].Should().Be("hello");
            actual.Properties["1"].Should().Be("world");
        }

        [Fact]
        public void should_be_able_to_process_type_with_parent_reference()
        {
            var obj = new CreateUserWizard();

            var sut = new ObjectToContextCollectionConverter();
            var actual = sut.Convert(obj);

            actual.Properties.Count.Should().BeLessThan(sut.MaxPropertyCount);
        }

        [Fact]
        public void should_be_able_to_convert_an_ienumerableKeyValuePair_as_a_dictionary()
        {
            var obj = new CustomDictionary();
            obj.Dictionary["Ada"] = "Lovelace";

            var sut = new ObjectToContextCollectionConverter();
            var actual = sut.Convert(obj);

            actual.Properties["[0].Key"].Should().Be("Ada");
            actual.Properties["[0].Value"].Should().Be("Lovelace");
        }

        [Fact]
        public void should_be_able_to_convert_an_ienumerableKeyValuePair_as_a_dictionary_when_being_inner_object()
        {
            var obj = new CustomDictionary();
            obj.Dictionary["Ada"] = "Lovelace";

            var sut = new ObjectToContextCollectionConverter();
            var actual = sut.Convert(new {brainiac=obj});

            actual.Properties["brainiac.Ada"].Should().Be("Lovelace");
        }

        [Fact]
        public void should_be_able_To_convert_sub_object_dictionaries_using_the_key_as_index()
        {
            var obj = new
            {
                mofo = new Dictionary<object, string>()
                {
                    {"ada", "hello"},
                    {1, "world"}
                }
            };

            var sut = new ObjectToContextCollectionConverter();
            var actual = sut.Convert(obj);

            actual.Properties["mofo[ada]"].Should().Be("hello");
            actual.Properties["mofo[1]"].Should().Be("world");
        }

        [Fact]
        public void should_be_able_to_convert_a_dynamic_object()
        {
            var item = new
            {
                Amount = 20000,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };


            var sut = new ObjectToContextCollectionConverter();
            var actual = sut.Convert(item);

            actual.Properties["Amount"].Should().Be("20000");
            actual.Properties["Expires"].Should().StartWith(DateTime.UtcNow.Year.ToString());
        }

        [Fact]
        public void should_be_able_to_convert_a_dictionary_with_a_dynamic_object_as_an_item()
        {
            var item = new
            {
                Amount = 20000,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };
            var dict = new Dictionary<string,object>();
            dict["DemoKey"] = item;


            var sut = new ObjectToContextCollectionConverter();
            var actual = sut.Convert(dict);

            actual.Properties["DemoKey.Amount"].Should().Be("20000");
            actual.Properties["DemoKey.Expires"].Should().StartWith(DateTime.UtcNow.Year.ToString());
        }

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
                if (exceptionType.Namespace == null)
                    continue;
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
                        if (item.Properties.ContainsKey("Error"))
                            Console.WriteLine(ex.GetType().FullName + ": " + item.Properties["Error"]);
                        else if (item.Properties["Message"] != thrownException.Message)
                        {
                            Console.WriteLine(exceptionType.FullName + ": Failed to check " + item.Properties["Message"]);
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

                        //contains temporal coupling
                        if (type == typeof (PartialTrustRunnerAttribute))
                            continue;

                        var t = sut.Convert(e);
                        Console.WriteLine(t);
                    }
                    catch { }

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
