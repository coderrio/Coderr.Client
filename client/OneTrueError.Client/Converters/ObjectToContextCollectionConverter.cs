using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OneTrueError.Client.Contracts;

namespace OneTrueError.Client.Converters
{
    /// <summary>
    ///     Converts an object into a context collection
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Anonymous objects are added as a "CustomData" collection while all other objects are added in a collection
    ///         which is named as their type name.
    ///     </para>
    /// </remarks>
    public class ObjectToContextCollectionConverter
    {
        private string[] _propertiesToIgnore = new string[0];

        /// <summary>
        ///     Turn an object into a string which can be used for debugging.
        /// </summary>
        /// <param name="collectionName">
        ///     Name of the collection that is being created. This name is displayed under "Similarities"
        ///     and "Context info" in our UI.
        /// </param>
        /// <param name="instance">Object to get a string representation for</param>
        /// <returns>"null" if the object is null, otherwise an string as given per object sample</returns>
        /// <remarks>
        ///     Look at the class doc for an example.
        /// </remarks>
        public ContextCollectionDTO Convert(string collectionName, object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (IsFilteredOut(instance))
                return new ContextCollectionDTO(collectionName,
                    new Dictionary<string, string> {{"Error", "The object type can not be traversed by OneTrueError"}});

            try
            {
                if (instance is ContextCollectionDTO)
                    return (ContextCollectionDTO) instance;
                if (instance is IDictionary<string, string>)
                    return new ContextCollectionDTO(collectionName, (IDictionary<string, string>) instance);
                if (instance is NameValueCollection)
                    return new ContextCollectionDTO(collectionName, (NameValueCollection) instance);
                var collection = new ContextCollectionDTO(collectionName);
                if (IsSimpleType(instance.GetType()))
                {
                    collection.Items.Add("Value", instance.ToString());
                }
                else
                {
                    var path = new List<object>();
                    ReflectObject(instance, "", collection, path);
                }

                return collection;
            }
            catch (Exception exception)
            {
                return new ContextCollectionDTO("OneTrueClientError",
                    new Dictionary<string, string>
                    {
                        {"Exception", exception.ToString()},
                        {"Type", instance.GetType().FullName},
                        {"Source", "ObjectToContextCollectionConverter"}
                    });
            }
        }

        /// <summary>
        ///     Turn an object into a string which can be used for debugging.
        /// </summary>
        /// <param name="instance">Object to get a string representation for</param>
        /// <returns>"null" if the object is null, otherwise an string as given per object sample</returns>
        /// <remarks>
        ///     <para>
        ///         Collection name will be <c>ContextData</c> unless the object is a real type which is not a collection (in that
        ///         case the type name is used).
        ///     </para>
        ///     <para>Look at the class doc for an example.</para>
        /// </remarks>
        public ContextCollectionDTO Convert(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");


            if (instance is ContextCollectionDTO)
                return (ContextCollectionDTO) instance;
            if (instance is IDictionary<string, string>)
                return new ContextCollectionDTO("ContextData", (IDictionary<string, string>) instance);
            if (instance is NameValueCollection)
                return new ContextCollectionDTO("ContextData", (NameValueCollection) instance);

            var name = instance.GetType().IsAnonymousType()
                ? "ContextData"
                : instance.GetType().Name;


            return Convert(name, instance);
        }

        /// <summary>
        ///     Properties that should be ignored when the context collection is being built.
        /// </summary>
        /// <param name="properties">Case sensitive names</param>
        public void Ignore(params string[] properties)
        {
            if (properties == null) throw new ArgumentNullException("properties");
            _propertiesToIgnore = properties;
        }

        /// <summary>
        ///     Checks if the specified type could be traversed or just added as a value.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns><c>true</c> if we should add this type as a value; <c>false</c> if we should do reflection on it.</returns>
        public bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
                   || type == typeof(decimal)
                   || type == typeof(string)
                   || type == typeof(DateTime)
                   || type == typeof(Guid)
                   || type == typeof(DateTimeOffset)
                   || type == typeof(NumberFormatInfo)
                   || type == typeof(DateTimeFormatInfo)
                   || type == typeof(TimeSpan)
                   || type.IsEnum
                   || type.FullName == "System.Drawing.Color";
        }

        /// <summary>
        ///     Use reflection on a complex object to add it's values to our context collection
        /// </summary>
        /// <param name="instance">Current object to reflect</param>
        /// <param name="prefix">Prefix, like "User.Address.Postal.ZipCode"</param>
        /// <param name="contextCollection">Collection that values should be added to.</param>
        /// <param name="path">To prevent circular references.</param>
        protected void ReflectObject(object instance, string prefix, ContextCollectionDTO contextCollection,
            List<object> path)
        {
            if (path.Contains(instance) || path.Count > 3)
                return;
            if (IsFilteredOut(instance))
                return;

            path.Add(instance);

            foreach (var propInfo in instance.GetType().GetProperties())
            {
                //TODO: Add support.
                if (propInfo.GetIndexParameters().Length != 0)
                    continue;

                if (_propertiesToIgnore.Contains(propInfo.Name))
                    continue;

                object value;
                try
                {
                    value = propInfo.GetValue(instance, null);
                    if (value == null)
                    {
                        contextCollection.Items.Add(prefix + propInfo.Name, "null");
                        continue;
                    }
                    var enc = value as Encoding;
                    if (enc != null)
                    {
                        contextCollection.Items.Add(prefix + propInfo.Name, enc.EncodingName);
                        continue;
                    }
                    var v1 = value as DateTimeFormatInfo;
                    if (v1 != null)
                    {
                        contextCollection.Items.Add(prefix + propInfo.Name, v1.NativeCalendarName);
                        continue;
                    }
                    var v2 = value as CultureInfo;
                    if (v2 != null)
                    {
                        contextCollection.Items.Add(prefix + propInfo.Name, "Culture[" + v2.LCID + "]");
                        continue;
                    }
                }
                catch (Exception exception)
                {
                    contextCollection.Items.Add(prefix + propInfo.Name + "._error", exception.ToString());
                    continue;
                }

                if (IsSimpleType(value.GetType()) || propInfo.Name == "Encoding")
                {
                    contextCollection.Items.Add(prefix + propInfo.Name, value.ToString());
                }
                else
                {
                    var items = value as IEnumerable;
                    if (items != null)
                    {
                        var index = 0;
                        foreach (var item in items)
                        {
                            var newPrefix = prefix == ""
                                ? string.Format("{0}[{1}].", propInfo.Name, index)
                                : string.Format("{0}{1}[{2}].", prefix, propInfo.Name, index);
                            ReflectObject(item, newPrefix, contextCollection, path);
                            index++;
                        }
                    }
                    else
                    {
                        var newPrefix = prefix == ""
                            ? propInfo.Name + "."
                            : prefix + propInfo.Name + ".";

                        if (propInfo.PropertyType == typeof(Type))
                            contextCollection.Items.Add(newPrefix, value.ToString());
                        else if (propInfo.PropertyType == typeof(Assembly))
                            contextCollection.Items.Add(newPrefix, value.ToString());
                        else if (propInfo.PropertyType.Namespace != null &&
                                 propInfo.PropertyType.Namespace.StartsWith("System.Reflection"))
                            contextCollection.Items.Add(newPrefix, value.ToString());
                        else
                            ReflectObject(value, newPrefix, contextCollection, path);
                    }
                }
            }

            path.Remove(instance);
        }

        private static bool IsFilteredOut(object instance)
        {
            if (instance is Exception)
                return false;

            var type = instance.GetType();
            var ns = type.Namespace;
            if (ns != null && ns.StartsWith("System.Runtime"))
                return true;
            if (instance.GetType().FullName == "System.RuntimeType")
                return true;
            if (ns != null && ns.StartsWith("System.Reflection"))
                return true;
            if (ns != null && ns.StartsWith("System.Threading.Tasks"))
                return true;
            if (instance.GetType().Name == "System.ContextStaticAttribute")
                return true;
            return false;
        }
    }
}