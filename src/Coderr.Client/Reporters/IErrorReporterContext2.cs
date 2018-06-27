using System.Collections.Generic;
using Coderr.Client.Contracts;

namespace Coderr.Client.Reporters
{
    /// <summary>
    /// To be able to add context collections directly
    /// </summary>
    public interface IErrorReporterContext2 : IErrorReporterContext
    {
        /// <summary>
        ///     All collections which have been added so far.
        /// </summary>
        IList<ContextCollectionDTO> ContextCollections { get; }
    }
}