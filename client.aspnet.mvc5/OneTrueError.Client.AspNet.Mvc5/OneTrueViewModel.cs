using System;

namespace OneTrueError.Client.AspNet.Mvc5
{
    /// <summary>
    ///     View model for all error pages used by this plugin.
    /// </summary>
    public class OneTrueViewModel
    {
        /// <summary>
        ///     OneTrueError error id
        /// </summary>
        public string ErrorId { get; set; }

        /// <summary>
        ///     Caught exception
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        ///     Http status code. Typically 404 or 500
        /// </summary>
        public int HttpStatusCode { get; set; }

        /// <summary>
        ///     Name of HTTP Status Code, like "InternalServerError"
        /// </summary>
        public string HttpStatusCodeName { get; set; }
    }
}