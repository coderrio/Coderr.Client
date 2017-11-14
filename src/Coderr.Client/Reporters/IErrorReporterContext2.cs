using System.Collections.Generic;
using codeRR.Client.Contracts;

namespace codeRR.Client.Reporters
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