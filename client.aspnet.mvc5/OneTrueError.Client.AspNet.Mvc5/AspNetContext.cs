using System;
using System.Web;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.AspNet.Mvc5
{
    /// <summary>
    /// Reporter context with information about ASP.NET.
    /// </summary>
    public class AspNetContext : IErrorReporterContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="AspNetContext"/>.
        /// </summary>
        /// <param name="reporter">object triggering the collection</param>
        /// <param name="exception">caught exception</param>
        /// <param name="httpContext">context that the exception was thrown on</param>
        /// <exception cref="ArgumentNullException">reporter;exception</exception>
        public AspNetContext(object reporter, Exception exception, HttpContextBase httpContext)
        {
            if (reporter == null) throw new ArgumentNullException("reporter");
            if (exception == null) throw new ArgumentNullException("exception");
            Exception = exception;
            HttpContext = httpContext;
            Reporter = reporter;
        }

        /// <summary>
        /// Http context
        /// </summary>
        public HttpContextBase HttpContext { get; private set; }

        /// <inheritdoc/>
        public Exception Exception { get; private set; }

        /// <inheritdoc/>
        public object Reporter { get; private set; }
    }
}