using System.Collections.Generic;

namespace Coderr.Client.ContextCollections
{
    /// <summary>
    ///     Context collections provide information about the current application state when the exception as thrown.
    /// </summary>
    public interface IContextCollection
    {
        /// <summary>
        ///     Name of this collection
        /// </summary>
        string CollectionName { get; }

        /// <summary>
        ///     Properties that this collection supply.
        /// </summary>
        IDictionary<string, string> Properties { get; }
    }
}