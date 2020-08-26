using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Coderr.Client.ContextCollections;
using Coderr.Client.NetStd.Tests.TestObjects;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Coderr.Client.NetStd.Tests.Config.ContextCollections
{
    public class ObjectToContextCollectionConverterTests
    {
        public ObjectToContextCollectionConverterTests()
        {
            CultureInfo.CurrentCulture = new CultureInfo("sv-se");
            CultureInfo.CurrentUICulture = new CultureInfo("sv-se");
        }

        #region Test helper methods
        private void TestOne(Action x)
        {
            TestOne2(x);
        }

        private void TestOne2(Action x)
        {
            TestOne3(x);
        }


        private void TestOne3(Action x)
        {
            TestOne4(x);
        }


        private void TestOne4(Action x)
        {
            x();
        }
        #endregion

        [Fact]
        public void should_be_able_to_handle_nested_inner_exceptions()
        {
            var ex1= new ArgumentNullException("moot");
            ex1.Data["aa"] = "World";
            var ex2 = new ArgumentException("Ex2","SomeProperty", ex1);
            var ex3= new Exception("Ex3", ex2);

            var sut = new ObjectToContextCollectionConverter();
            var actual = sut.Convert(ex3);

            actual.Properties["Message"].Should().Be("Ex3");
            actual.Properties["InnerException.Message"].Should().Contain("Ex2");
            actual.Properties["InnerException.ParamName"].Should().Be("SomeProperty");
            actual.GetProperty("InnerException.InnerException.Message").Should().Contain("moot");
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

        [Fact]
        public void should_be_able_to_convert_a_dictionary_with_a_dynamic_object_as_an_item()
        {
            var item = new
            {
                Amount = 20000,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };
            var dict = new Dictionary<string, object> {["DemoKey"] = item};


            var sut = new ObjectToContextCollectionConverter();
            var actual = sut.Convert(dict);

            actual.Properties["DemoKey.Amount"].Should().Be("20000");
            actual.Properties["DemoKey.Expires"].Should().StartWith(DateTime.UtcNow.Year.ToString());
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
            var actual = sut.Convert(new { brainiac = obj });

            actual.Properties["brainiac[0].Key"].Should().Be("Ada");
            actual.Properties["brainiac[0].Value"].Should().Be("Lovelace");
        }

        [Fact]
        public void should_be_able_To_convert_dictionaries_using_the_key_as_index()
        {
            var obj = new Dictionary<object, string> { { "ada", "hello" }, { 1, "world" } };

            var sut = new ObjectToContextCollectionConverter();
            var actual = sut.Convert(obj);

            actual.Properties["ada"].Should().Be("hello");
            actual.Properties["1"].Should().Be("world");
        }

        [Fact]
        public void should_be_able_To_convert_sub_object_dictionaries_using_the_key_as_index()
        {
            var obj = new
            {
                mofo = new Dictionary<object, string>
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
        public void should_be_able_to_process_type_with_circular_reference()
        {
            var obj = new GotParentReference { Child = new GotParentReferenceChild { Title = "chld" }, Name = "prnt" };
            obj.Child.Parent = obj;

            var sut = new ObjectToContextCollectionConverter();
            var actual = sut.Convert(obj);

            actual.Properties.ContainsKey("Child.Parent._error").Should().BeTrue();
        }

        [Fact]
        public void validation_exception_from_dataAnnotations_is_special_so_make_Sure_That_it_can_Be_serialized()
        {
            var ex = new ValidationException("Hello world");
            
            var sut = new ObjectToContextCollectionConverter();
            var actual = sut.Convert(ex);

            actual.Properties["Message"].Should().Be("Hello world");
        }

        [Fact]
        public void should_be_able_to_serialize_all_types_of_exceptions()
        {
            string[] ignoredExceptions =
            {
                /*"UpaException", "EventLogException", "EventLogNotFoundException", "PrivilegeNotHeldException",
                "ContractExceptionUnknown", "ContractExceptionUnknown"*/
            };


            var sut = new ObjectToContextCollectionConverter();

            var inner = new Exception("hello");
            var exceptionTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes().Where(y => typeof(Exception).IsAssignableFrom(y)))
                    .ToList();

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
                            thrownException = (Exception)Activator.CreateInstance(exceptionType, "Hello world", inner);
                            throw thrownException;
                        });
                    }
                    catch (Exception ex)
                    {
                        var item = sut.Convert(ex);
                        if (item.Properties.ContainsKey("Error"))
                        {
                            Console.WriteLine(ex.GetType().FullName + ": " + item.Properties["Error"]);
                        }
                        else if (item.Properties["Message"] != thrownException.Message)
                        {
                            Console.WriteLine(
                                exceptionType.FullName + ": Failed to check " + item.Properties["Message"]);
                            throw new AssertionFailedException("Failed to serialize message for " + thrownException);
                        }
                    }
                }
                else
                {
                    Console.WriteLine(exceptionType + ": Unknown constructor ");
                }
            }
        }
        
    }
}