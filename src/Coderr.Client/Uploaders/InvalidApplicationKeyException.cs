using System;

namespace codeRR.Client.Uploaders
{
    /// <summary>
    ///     Thrown when the server did not find our application key
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         You can catch this exception using the <c>Err.Configuration.Advanced.ReportingFailed</c> event.
    ///     </para>
    /// </remarks>
    [Serializable]
    public class InvalidApplicationKeyException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidApplicationKeyException" /> class.
        /// </summary>
        /// <param name="errMsg">The error MSG.</param>
        /// <param name="inner">The inner.</param>
        public InvalidApplicationKeyException(string errMsg, Exception inner)
            : base(errMsg, inner)
        {
        }
    }
}