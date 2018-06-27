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
        public static ContextCollectionDTO GetCoderrCollection(this IErrorReporterContext2 context)
        {
            var collection = context.ContextCollections.FirstOrDefault(x => x.Name == "CoderrData");
            if (collection != null)
                return collection;

            collection = new ContextCollectionDTO("CoderrData");
            context.ContextCollections.Add(collection);
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

        public static void AddTag(this IList<ContextCollectionDTO> collections, string tagName)
        {
            var coderrCollection = GetCoderrCollection(collections);
            if (!coderrCollection.Properties.TryGetValue("ErrTags", out var tags))
            {
                coderrCollection.Properties["ErrTags"] = tagName;
                return;
            }

            if (!tags.Contains(tagName))
                coderrCollection.Properties["ErrTags"] = tags + "," + tagName;
        }

        public static void AddTag(this IErrorReporterContext2 context, string tagName)
        {
            AddTag(context.ContextCollections, tagName);
        }
    }
}