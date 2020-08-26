using System;

namespace Coderr.Client
{
    /// <summary>
    ///     Exception thrown in the Coderr library
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         We should not try to upload these.
    ///     </para>
    /// </remarks>
    public class CoderrClientException : Exception
    {
        /// <summary>
        /// Base exception for internal exceptions in the Coderr client.
        /// </summary>
        /// <param name="msg">Error message</param>
        public CoderrClientException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Base exception for internal exceptions in the Coderr client.
        /// </summary>
        /// <param name="msg">Error message</param>
        /// <param name="inner">inner exception.</param>
        public CoderrClientException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}