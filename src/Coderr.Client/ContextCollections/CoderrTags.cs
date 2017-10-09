using System;
using System.Collections.Generic;

namespace codeRR.Client.ContextCollections
{
    /// <summary>
    ///     Represents a set of tags that should be visible in the UI when the report is received.
    /// </summary>
    public sealed class CoderrTags : IContextCollection
    {
        private readonly string[] _tags;

        /// <summary>
        ///     Creates a new instance of <see cref="CoderrTags" />.
        /// </summary>
        /// <param name="tags"></param>
        public CoderrTags(string[] tags)
        {
            if (tags == null) throw new ArgumentNullException("tags");
            if (tags.Length == 0)
                throw new ArgumentOutOfRangeException("tags", "Tags must not be an empty collection.");
            _tags = tags;
        }

        string IContextCollection.CollectionName => "CoderrTags";

        IDictionary<string, string> IContextCollection.Properties => new Dictionary<string, string>
        {
            {"ErrTags", string.Join(";", _tags)}
        };
    }
}