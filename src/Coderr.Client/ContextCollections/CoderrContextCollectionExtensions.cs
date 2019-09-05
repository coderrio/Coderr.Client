using System;
using System.Collections.Generic;
using System.Linq;
using Coderr.Client.Contracts;
using Coderr.Client.Reporters;

namespace Coderr.Client.ContextCollections
{
    /// <summary>
    ///     Extensions to get the Coderr collection that we store meta data in
    /// </summary>
    public static class CoderrContextCollectionExtensions
    {
        /// <summary>
        ///     Get or create our collection
        /// </summary>
        /// <param name="context">context to find the collection in</param>
        /// <returns>collection</returns>
        public static ContextCollectionDTO GetCoderrCollection(this IErrorReporterContext context)
        {
            var ctx2 = (IErrorReporterContext2)context;
            var collection = ctx2.ContextCollections.FirstOrDefault(x => x.Name == "CoderrData");
            if (collection != null)
                return collection;

            collection = new ContextCollectionDTO("CoderrData");
            ctx2.ContextCollections.Add(collection);
            return collection;
        }

        /// <summary>
        ///     Get or create our collection
        /// </summary>
        /// <param name="collections">Collections array</param>
        /// <returns>collection</returns>
        public static ContextCollectionDTO GetCoderrCollection(this IList<ContextCollectionDTO> collections)
        {
            var collection = collections.FirstOrDefault(x => x.Name == "CoderrData");
            if (collection != null)
                return collection;

            collection = new ContextCollectionDTO("CoderrData");
            collections.Add(collection);
            return collection;
        }

        /// <summary>
        /// Used to navigate through related errors (all related errors must have the same correlation id).
        /// </summary>
        public static void AddCorrelationId(this IList<ContextCollectionDTO> collections, string value)
        {
            var coderrCollection = GetCoderrCollection(collections);
            coderrCollection.Properties["CorrelationId"] = value;
        }

        /// <summary>
        /// Used to navigate through related errors (all related errors must have the same correlation id).
        /// </summary>
        public static void AddCorrelationId(this IErrorReporterContext context, string value)
        {
            if (!(context is IErrorReporterContext2 ctx))
                throw new NotSupportedException("Only works with IErrorReporterContext2.");

            ctx.ContextCollections.AddCorrelationId(value);
        }

        /// <summary>
        ///     Add a tag which can be used in the UI to limit search result.
        /// </summary>
        /// <param name="collections">instance</param>
        /// <param name="tagName">tag name</param>
        public static void AddTag(this IList<ContextCollectionDTO> collections, string tagName)
        {
            var coderrCollection = GetCoderrCollection(collections);
            if (!coderrCollection.Properties.TryGetValue(CoderrCollectionProperties.Tags, out var tags))
            {
                coderrCollection.Properties[CoderrCollectionProperties.Tags] = tagName;
                return;
            }

            if (!tags.Contains(tagName))
                coderrCollection.Properties[CoderrCollectionProperties.Tags] = $"{tags},{tagName}";
        }

        /// <summary>
        /// Quick facts are shown in the UI in the right panel
        /// </summary>
        /// <param name="collections">instance</param>
        /// <param name="propertyName">property name</param>
        /// <param name="propertyValue">value</param>
        public static void AddQuickFact(this IList<ContextCollectionDTO> collections, string propertyName,
            string propertyValue)
        {
            var coderrCollection = GetCoderrCollection(collections);

            var name = CoderrCollectionProperties.QuickFact.Replace("{Name}", propertyName);
            var value = propertyValue;
            coderrCollection.Properties[name] = value;
        }

        /// <summary>
        /// Highlighted properties are shown directly before the stack trace.
        /// </summary>
        /// <param name="collections">instance</param>
        /// <param name="contextCollectionName">Name of the context collection that the property is in</param>
        /// <param name="propertyName">Property to display (along with its value)</param>
        public static void AddHighlightedProperty(this IList<ContextCollectionDTO> collections,
            string contextCollectionName, string propertyName)
        {
            var coderrCollection = GetCoderrCollection(collections);

            var value = $"{contextCollectionName}.{propertyName}";
            if (!coderrCollection.Properties.TryGetValue(CoderrCollectionProperties.HighlightProperties, out var values)
            )
            {
                coderrCollection.Properties[CoderrCollectionProperties.HighlightProperties] = value;
                return;
            }

            if (!values.Contains(value))
                coderrCollection.Properties[CoderrCollectionProperties.HighlightProperties] = $"{values},{value}";
        }


        /// <summary>
        /// The first highlighted collection are selected per default in the context collection navigator.
        /// </summary>
        /// <param name="collections">instance</param>
        /// <param name="contextCollectionName">Name of the context collection that we should show all properties and their values from.</param>
        public static void AddHighlightedCollection(this IList<ContextCollectionDTO> collections,
            string contextCollectionName)
        {
            var coderrCollection = GetCoderrCollection(collections);

            var value = contextCollectionName;
            if (!coderrCollection.Properties.TryGetValue(CoderrCollectionProperties.HighlightCollection, out var values)
            )
            {
                coderrCollection.Properties[CoderrCollectionProperties.HighlightCollection] = value;
                return;
            }

            if (!values.Contains(value))
                coderrCollection.Properties[CoderrCollectionProperties.HighlightCollection] = $"{values},{value}";
        }

        /// <summary>
        ///     Add a tag which can be used in the UI to limit search result.
        /// </summary>
        /// <param name="context">instance</param>
        /// <param name="tagName">tag name</param>
        public static void AddTag(this IErrorReporterContext context, string tagName)
        {
            var ctx2 = (IErrorReporterContext2)context;
            AddTag(ctx2.ContextCollections, tagName);
        }



        /// <summary>
        /// Highlighted properties are shown directly before the stack trace.
        /// </summary>
        /// <param name="context">instance</param>
        /// <param name="contextCollectionName">Name of the context collection that the property is in</param>
        /// <param name="propertyName">Property to display (along with its value)</param>
        public static void AddHighlightedProperty(this IErrorReporterContext context, string contextCollectionName,
                string propertyName)
        {
            var ctx2 = (IErrorReporterContext2)context;
            AddHighlightedProperty(ctx2.ContextCollections, contextCollectionName, propertyName);
        }

        /// <summary>
        /// The first highlighted collection are selected per default in the context collection navigator.
        /// </summary>
        /// <param name="context">instance</param>
        /// <param name="contextCollectionName">Name of the context collection that we should show all properties and their values from.</param>
        public static void AddHighlightedCollection(this IErrorReporterContext context, string contextCollectionName)
        {
            var ctx2 = (IErrorReporterContext2)context;
            AddHighlightedCollection(ctx2.ContextCollections, contextCollectionName);
        }

        /// <summary>
        /// Quick facts are shown in the UI in the right panel
        /// </summary>
        /// <param name="context">instance</param>
        /// <param name="propertyName">property name</param>
        /// <param name="propertyValue">value</param>
        public static void AddQuickFact(this IErrorReporterContext context, string propertyName, string propertyValue)
        {
            var ctx2 = (IErrorReporterContext2)context;
            AddQuickFact(ctx2.ContextCollections, propertyName, propertyValue);
        }
    }
}