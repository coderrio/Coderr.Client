using System;
using System.Web;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet
{
    /// <summary>
    ///     Context used when collecting error information (when an error have been caught).
    /// </summary>
    public class HttpErrorReporterContext : ErrorReporterContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpErrorReporterContext" /> class.
        /// </summary>
        /// <param name="reporter">The reporter.</param>
        /// <param name="exception">The exception.</param>
        public HttpErrorReporterContext(object reporter, Exception exception)
            : base(reporter, exception)
        {
        }

        /// <summary>
        ///     Message to display in the error page
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     Gets context
        /// </summary>
        public HttpContext HttpContext { get; set; }

        /// <summary>
        ///     Status code to report to OneTrueError.
        /// </summary>
        public int HttpStatusCode { get; set; }

        /// <summary>
        ///     Name like "InternalServerError".
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Used to select error page to display. Should not contain any spaces.
        ///     </para>
        /// </remarks>
        public string HttpStatusCodeName { get; set; }
    }
}