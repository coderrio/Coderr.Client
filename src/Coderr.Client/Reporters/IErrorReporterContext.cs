using System;

namespace Coderr.Client.Reporters
{
    /// <summary>
    ///     contains context information which can be used during collection such as <c>HttpContext</c> (if the exception was
    ///     thrown during a HTTP request)
    /// </summary>
    public interface IErrorReporterContext
    {
        /// <summary>
        ///     Exception which was caused the error.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        ///     Gets class which is sending the report ( so that we know which part of the system that caught the exception)
        /// </summary>
        object Reporter { get; }
    }
}