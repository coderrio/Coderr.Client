using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Converters;

namespace OneTrueError.Client
{
    /// <summary>
    ///     Extension methods for objects.
    /// </summary>
    public static class ObjectExtensions
    {
        // Copied from System.Web.WebPages/Util/TypeHelpers.cs
        /// <summary>
        ///     Determines whether the given type is an anonymous object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     <c>true</c> if type is anonymous; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAnonymousType(this Type type)
        {
            // TODO: The only way to detect anonymous types right now.
            return type.IsGenericType
                   && Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.Name.Contains("AnonymousType")
                   &&
                   (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) ||
                    type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase))
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }


        /// <summary>
        ///     Converts simple objects into context collections
        /// </summary>
        /// <param name="instance">Object to convert</param>
        /// <returns>Context information</returns>
        /// <remarks>
        ///     Anonymous types get the collection name "CustomData" while any other class get the class name as collection name.
        /// </remarks>
        public static ContextCollectionDTO ToContextCollection(this object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            var converter = new ObjectToContextCollectionConverter();
            return converter.Convert(instance);
        }

        /// <summary>
        ///     Converts simple objects into context collections
        /// </summary>
        /// <param name="instance">Object to convert</param>
        /// <param name="name">Name to assign to the context collection</param>
        /// <returns>Context information</returns>
        /// <remarks>
        ///     Anonymous types get the collection name "CustomData" while any other class get the class name as collection name.
        /// </remarks>
        public static ContextCollectionDTO ToContextCollection(this object instance, string name)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            var converter = new ObjectToContextCollectionConverter();
            return converter.Convert(name, instance);
        }
    }
}