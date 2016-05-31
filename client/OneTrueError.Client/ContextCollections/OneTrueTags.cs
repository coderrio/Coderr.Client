using System;
using System.Collections.Generic;

namespace OneTrueError.Client.ContextCollections
{
    /// <summary>
    ///     Represents a set of tags that should be visible in the UI when the report is received.
    /// </summary>
    public sealed class OneTrueTags : IContextCollection
    {
        private readonly string[] _tags;

        /// <summary>
        ///     Creates a new instance of <see cref="OneTrueTags" />.
        /// </summary>
        /// <param name="tags"></param>
        public OneTrueTags(string[] tags)
        {
            if (tags == null) throw new ArgumentNullException("tags");
            if (tags.Length == 0)
                throw new ArgumentOutOfRangeException("tags", "Tags must not be an empty collection.");
            _tags = tags;
        }

        string IContextCollection.CollectionName
        {
            get { return "OneTrueTags"; }
        }

        IDictionary<string, string> IContextCollection.Properties
        {
            get { return new Dictionary<string, string> {{"Tags", string.Join(";", _tags)}}; }
        }
    }
}