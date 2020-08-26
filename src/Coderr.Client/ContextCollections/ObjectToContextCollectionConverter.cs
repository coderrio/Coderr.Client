using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Coderr.Client.Contracts;

namespace Coderr.Client.ContextCollections
{
    /// <summary>
    ///     Converts an object into a context collection.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The following conversions are supported:
    ///     </para>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>type</term>
    ///             <description>description</description>
    ///         </listheader>
    ///         <item>
    ///             <term>anonymous object</term>
    ///             <description>Collection will be named <c>CustomData</c>. All properties will be included</description>
    ///         </item>
    ///         <item>
    ///             <term>class</term>
    ///             <description>Collection will be named as the class. All properties will be included</description>
    ///         </item>
    ///         <item>
    ///             <term>
    ///                 <see cref="ContextCollectionDTO" />
    ///             </term>
    ///             <description>Collection is included directly</description>
    ///         </item>
    ///         <item>
    ///             <term>
    ///                 <c>ContextCollectionDTO[]</c>
    ///             </term>
    ///             <description>All collections will be added as different ones (and not nested)</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public class ObjectToContextCollectionConverter
    {
        private readonly MethodInfo _dictionaryConverterMethod;
        private readonly MethodInfo _keyValuePairEnumeratorConverterMethod;
        private string[] _propertiesToIgnore = new string[0];

        /// <summary>
        ///     Creates a new instance of <see cref="ObjectToContextCollectionConverter" />.
        /// </summary>
        public ObjectToContextCollectionConverter()
        {
            MaxPropertyCount = 10000;
            _dictionaryConverterMethod = GetType()
                .GetTypeInfo()
                .DeclaredMethods.First(x => x.Name == "FlattenDictionary");
            _keyValuePairEnumeratorConverterMethod = GetType()
                .GetTypeInfo()
                .DeclaredMethods.First(x => x.Name == "ConvertKvpEnumerator");
        }

        /// <summary>
        ///     Maximum number of properties that can be added during the collection process.
        /// </summary>
        /// <value>
        ///     Default is 10,000
        /// </value>
        public int MaxPropertyCount { get; set; }

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
                    new Dictionary<string, string> { { "Error", "The object type can not be traversed by codeRR" } });

            var props = ConvertToDictionary("", instance);
            return new ContextCollectionDTO(collectionName, props);
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
                return (ContextCollectionDTO)instance;
            if (instance is IDictionary<string, string>)
                return new ContextCollectionDTO("ContextData", (IDictionary<string, string>)instance);

            var name = instance.GetType().IsAnonymousType() || IsSimpleType(instance.GetType())
                ? "ContextData"
                : instance.GetType().Name;


            return Convert(name, instance);
        }

        /// <summary>
        ///     Turn an object into a string which can be used for debugging.
        /// </summary>
        /// <param name="prefix">
        ///     Prefix all keys in the dictionary with this. For instance <c>"User"</c> will generate keys like
        ///     <c>"User.FirstName"</c>. Leave empty for no prefix.
        /// </param>
        /// <param name="instance">Object to get a string representation for</param>
        /// <param name="destination">Dictionary that items will be added to.</param>
        /// <returns>"null" if the object is null, otherwise dictionary</returns>
        /// <remarks>
        ///     Look at the class doc for an example.
        /// </remarks>
        public void ConvertToDictionary(string prefix, object instance, IDictionary<string, string> destination)
        {
            try
            {
                var dictIf = GetGenericDictionaryInterface(instance);
                if (dictIf != null)
                {
                    var path = new List<object>();
                    _dictionaryConverterMethod.MakeGenericMethod(dictIf.GenericTypeArguments)
                        .Invoke(this, new[] { prefix, instance, destination, path });
                    return;
                }
                var kvpTypes = GetKeyValuePairFromEnumeratorInterface(instance);
                if (kvpTypes != null)
                {
                    var path = new List<object>();
                    _keyValuePairEnumeratorConverterMethod.MakeGenericMethod(kvpTypes)
                        .Invoke(this, new[] { prefix, instance, destination, path });
                    return;
                }
                if (instance is IDictionary<string, string> dictionary)
                {
                    if (string.IsNullOrEmpty(prefix))
                        return;

                    foreach (var kvp in dictionary)
                        destination.Add($"{prefix}.{kvp.Key}", kvp.Value);
                }

                if (instance is IDictionary)
                {
                    FlattenDictionary((IDictionary)instance, destination);
                    return;
                }
                if (IsSimpleType(instance.GetType()))
                {
                    if (string.IsNullOrEmpty(prefix))
                        destination.Add($"Value", instance.ToString());
                    else
                        destination.Add($"{prefix}.Value", instance.ToString());
                }
                else
                {
                    var path = new List<object>();
                    ReflectObject(instance, prefix, destination, path);
                }
            }
            catch (Exception exception)
            {
                destination.Add("Exception", exception.ToString());
                destination.Add("Type", instance.GetType().FullName);
                destination.Add("Source", "ObjectToContextCollectionConverter");
            }
            ;
        }


        /// <summary>
        ///     Turn an object into a string which can be used for debugging.
        /// </summary>
        /// <param name="prefix">
        ///     Prefix all keys in the dictionary with this. For instance <c>"User"</c> will generate keys like
        ///     <c>"User.FirstName"</c>. Leave empty for no prefix.
        /// </param>
        /// <param name="instance">Object to get a string representation for</param>
        /// <returns>"null" if the object is null, otherwise dictionary</returns>
        /// <remarks>
        ///     Look at the class doc for an example.
        /// </remarks>
        public IDictionary<string, string> ConvertToDictionary(string prefix, object instance)
        {
            var result = new Dictionary<string, string>();
            ConvertToDictionary(prefix, instance, result);
            return result;
        }

        /// <summary>
        ///     Properties that should be ignored when the context collection is being built.
        /// </summary>
        /// <param name="properties">Case sensitive names</param>
        /// <exception cref="ArgumentNullException">properties</exception>
        public void Ignore(params string[] properties)
        {
            _propertiesToIgnore = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        /// <summary>
        ///     Checks if the specified type could be traversed or just added as a value.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns><c>true</c> if we should add this type as a value; <c>false</c> if we should do reflection on it.</returns>
        public bool IsSimpleType(Type type)
        {
            return type.GetTypeInfo().IsPrimitive
                   || type == typeof(decimal)
                   || type == typeof(string)
                   || type == typeof(DateTime)
                   || type == typeof(Guid)
                   || type == typeof(DateTimeOffset)
                   || type == typeof(NumberFormatInfo)
                   || type == typeof(DateTimeFormatInfo)
                   || type == typeof(TimeSpan)
                   || type.GetTypeInfo().IsEnum
                   || type.FullName == "System.Drawing.Color";
        }

        /// <summary>
        ///     Use reflection on a complex object to add it's values to our context collection
        /// </summary>
        /// <param name="source">Current object to reflect</param>
        /// <param name="prefix">Prefix, like "User.Address.Postal.ZipCode"</param>
        /// <param name="destination">Collection that values should be added to.</param>
        /// <param name="path">To prevent circular references.</param>
        protected void ReflectObject(object source, string prefix, IDictionary<string, string> destination,
            List<object> path)
        {
            if (path.Contains(source) || path.Count > 10 || MaxPropertyCount <= destination.Count)
            {
                destination.Add(prefix + "_error", "Circular reference or to deep hierarchy.");
                return;
            }
            if (IsFilteredOut(source))
                return;

            path.Add(source);

            var type = source.GetType().GetTypeInfo();
            while (!type.Name.Equals("object", StringComparison.OrdinalIgnoreCase))
            {

                foreach (var propInfo in type.DeclaredProperties)
                {
                    //TODO: Add support.
                    if (propInfo.GetIndexParameters().Length != 0)
                        continue;

                    if (_propertiesToIgnore.Contains(propInfo.Name))
                        continue;

                    var propertyName = propInfo.Name;
                    object value;
                    try
                    {
                        value = propInfo.GetValue(source, null);
                    }
                    catch (Exception exception)
                    {
                        destination[prefix + propertyName + "._error"] = exception.ToString();
                        continue;
                    }

                    if (value == null)
                    {
                        destination[prefix + propertyName] = "null";
                        continue;
                    }
                    if (value is Encoding enc)
                    {
                        destination[prefix + propertyName] = enc.EncodingName;
                        continue;
                    }
                    if (value is DateTimeFormatInfo v1)
                    {
                        destination[prefix + propertyName] = v1.FullDateTimePattern;
                        continue;
                    }
                    if (value is CultureInfo v2)
                    {
                        destination[prefix + propertyName] = $"Culture[\"{v2.Name}\"]";
                        continue;
                    }
                    if (IsSimpleType(value.GetType()) || propertyName == "Encoding")
                    {
                        destination[prefix + propertyName] = value.ToString();
                    }
                    else
                    {
                        var dictIf = GetGenericDictionaryInterface(value);
                        var kvpTypes = GetKeyValuePairFromEnumeratorInterface(value);
                        if (dictIf != null)
                        {
                            _dictionaryConverterMethod.MakeGenericMethod(dictIf.GenericTypeArguments)
                                .Invoke(this, new[] { propertyName, value, destination, path });
                        }
                        else if (kvpTypes != null)
                        {
                            _keyValuePairEnumeratorConverterMethod.MakeGenericMethod(kvpTypes)
                                .Invoke(this, new[] { propertyName, value, destination, path });
                        }
                        else if (value is IDictionary)
                        {
                            var items = value as IDictionary;
                            foreach (DictionaryEntry kvp in items)
                            {
                                var dictPropName = kvp.Key == null ? "null" : kvp.Key.ToString();
                                var newPrefix = string.Format("{0}{1}[{2}]", prefix, propertyName, dictPropName);
                                ReflectValue(newPrefix, kvp.Value, destination,
                                    path);
                            }
                        }
                        else if (value is IEnumerable)
                        {
                            var items = value as IEnumerable;
                            var index = 0;
                            foreach (var item in items)
                            {
                                var newPrefix = prefix == ""
                                    ? string.Format("{0}[{1}]", propertyName, index)
                                    : string.Format("{0}{1}[{2}]", prefix, propertyName, index);
                                ReflectValue(newPrefix, item, destination, path);
                                index++;
                            }
                        }
                        else
                        {
                            var newPrefix = prefix == ""
                                ? propertyName + "."
                                : prefix + propertyName + ".";

                            if (propInfo.PropertyType == typeof(Type))
                                destination.Add(newPrefix, value.ToString());
                            else if (propInfo.PropertyType == typeof(Assembly))
                                destination.Add(newPrefix, value.ToString());
                            else if (propInfo.PropertyType.Namespace != null &&
                                     propInfo.PropertyType.Namespace.StartsWith("System.Reflection"))
                                destination.Add(newPrefix, value.ToString());
                            else
                                ReflectObject(value, newPrefix, destination, path);
                        }
                    }
                }

                type = type.BaseType.GetTypeInfo();
            }


            path.Remove(source);
        }

        /// <summary>
        ///     Use reflection on a complex object to add it's values to our context collection.
        /// </summary>
        /// <param name="propertyName">Property that this collection belongs to</param>
        /// <param name="source">object to reflect</param>
        /// <param name="destination">Collection that values should be added to.</param>
        /// <param name="path">To prevent circular references.</param>
        protected void ReflectValue(string propertyName, object source, IDictionary<string, string> destination,
            List<object> path)
        {
            if (IsFilteredOut(source))
                return;

            if (source == null)
            {
                destination.Add(propertyName, "null");
                return;
            }

            if (source is string)
            {
                destination.Add(propertyName, source.ToString());
                return;
            }

            if (source is Encoding enc)
            {
                destination.Add(propertyName, enc.EncodingName);
                return;
            }
            if (source is DateTimeFormatInfo v1)
            {
                destination.Add(propertyName, v1.FullDateTimePattern);
                return;
            }
            if (source is CultureInfo v2)
            {
                destination.Add(propertyName, $"Culture[\"{v2.Name}\"]");
                return;
            }
            if (IsSimpleType(source.GetType()) || propertyName == "Encoding")
            {
                destination.Add(propertyName, source.ToString());
            }
            else
            {
                var dictIf = GetGenericDictionaryInterface(source);
                var kvpTypes = GetKeyValuePairFromEnumeratorInterface(source);
                if (dictIf != null)
                {
                    _dictionaryConverterMethod.MakeGenericMethod(dictIf.GenericTypeArguments)
                        .Invoke(this, new[] { propertyName, source, destination, path });
                }
                else if (kvpTypes != null)
                {
                    _keyValuePairEnumeratorConverterMethod.MakeGenericMethod(kvpTypes)
                        .Invoke(this, new[] { propertyName, source, destination, path });
                }

                else if (source is IDictionary)
                {
                    var items = source as IDictionary;
                    foreach (DictionaryEntry kvp in items)
                    {
                        var newPrefix = $"{propertyName}[{kvp.Key}].";
                        ReflectObject(kvp.Value, newPrefix, destination, path);
                    }
                }
                else if (source is IEnumerable)
                {
                    var items = source as IEnumerable;
                    var index = 0;
                    foreach (var item in items)
                    {
                        var newPrefix = $"{propertyName}[{index}].";
                        ReflectObject(item, newPrefix, destination, path);
                        index++;
                    }
                }
                else
                {
                    var newPrefix = $"{propertyName}.";

                    if (source.GetType() == typeof(Type))
                        destination.Add(newPrefix, source.ToString());
                    else if (source.GetType() == typeof(Assembly))
                        destination.Add(newPrefix, source.ToString());
                    else if (source.GetType().Namespace != null &&
                             source.GetType().Namespace.StartsWith("System.Reflection"))
                        destination.Add(newPrefix, source.ToString());
                    else
                        ReflectObject(source, newPrefix, destination, path);
                }
            }


            path.Remove(source);
        }

        // ReSharper disable once UnusedMember.Local   //used through reflection
        private void ConvertKvpEnumerator<TKey, TValue>(string propertyName,
            IEnumerable<KeyValuePair<TKey, TValue>> enumerable, IDictionary<string, string> destination,
            List<object> path)
        {
            var index = 0;
            foreach (var kvp in enumerable)
            {
                var key = kvp.Key == null ? "null" : kvp.Key.ToString();
                var prefix = string.IsNullOrEmpty(propertyName)
                    ? string.Format("[{0}]", index++)
                    : string.Format("{0}[{1}]", propertyName, index++);

                destination.Add(prefix + ".Key", key);
                ReflectValue(prefix + ".Value", kvp.Value, destination, path);
            }
        }

        // ReSharper disable once UnusedMember.Local   //used through reflection
        private void FlattenDictionary<TKey, TValue>(string propertyName, IDictionary<TKey, TValue> source,
            IDictionary<string, string> destination,
            List<object> path)
        {
            foreach (var kvp in source)
            {
                var key = kvp.Key == null ? "null" : kvp.Key.ToString();
                var prefix = string.IsNullOrEmpty(propertyName)
                    ? key
                    : $"{propertyName}[{key}]";
                ReflectValue(prefix, kvp.Value, destination, path);
            }
        }


        private void FlattenDictionary(IDictionary source, IDictionary<string, string> destination)
        {
            var path = new List<object>();
            foreach (DictionaryEntry kvp in source)
            {
                var propertyName = kvp.Key == null ? "null" : kvp.Key.ToString();
                ReflectValue(propertyName, kvp.Value, destination, path);
            }
        }

        private static Type GetGenericDictionaryInterface(object instance)
        {
            var dictIf = instance.GetType()
                .GetTypeInfo()
                .ImplementedInterfaces
                .FirstOrDefault(x => x.GetTypeInfo().IsGenericType &&
                                     x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            return dictIf;
        }

        private static Type[] GetKeyValuePairFromEnumeratorInterface(object instance)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery - Easier to debug
            foreach (var @interface in instance.GetType().GetTypeInfo().ImplementedInterfaces)
            {
                var ifType = @interface.GetTypeInfo();
                if (!ifType.IsGenericType || !ifType.GenericTypeArguments[0].IsConstructedGenericType)
                    continue;

                var kvpType = ifType.GenericTypeArguments[0].GetGenericTypeDefinition();
                if (kvpType != typeof(KeyValuePair<,>))
                    continue;

                return ifType.GenericTypeArguments[0].GenericTypeArguments;
            }
            return null;
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