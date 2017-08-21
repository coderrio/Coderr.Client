using System;

namespace OneTrueError.Client.Reporters
{
    /// <summary>
    ///     Context supplied by error reports
    /// </summary>
    /// <remarks>
    ///     Used to be able to provide app specific context information (for instance HTTP apps can provide the HTTP
    ///     context)
    /// </remarks>
    public class ErrorReporterContext : IErrorReporterContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ErrorReporterContext" /> class.
        /// </summary>
        /// <param name="reporter">The reporter.</param>
        /// <param name="exception">The exception.</param>
        public ErrorReporterContext(object reporter, Exception exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            Exception = exception;
            Reporter = reporter;
        }

        /// <summary>
        ///     Gets class which is sending the report
        /// </summary>
        public object Reporter { get; }

        /// <summary>
        ///     Gets caught exception
        /// </summary>
        public Exception Exception { get; }
    }
}