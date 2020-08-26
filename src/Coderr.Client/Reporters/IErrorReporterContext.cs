using System;
using System.Collections.Generic;
using Coderr.Client.Contracts;

namespace Coderr.Client.Reporters
{
    /// <summary>
    ///     contains context information which can be used during collection such as <c>HttpContext</c> (if the exception was
    ///     thrown during a HTTP request)
    /// </summary>
    public interface IErrorReporterContext
    {
        /// <summary>
        ///     All collections which have been added so far.
        /// </summary>
        IList<ContextCollectionDTO> ContextCollections { get; }

        /// <summary>
        ///     Exception which was caused the error.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        ///     Gets class which is sending the report (so that we know which part of the system that caught the exception)
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Typically the class in one of the integration libraries that detected the exception.
        ///     </para>
        /// </remarks>
        object Reporter { get; }
    }
}