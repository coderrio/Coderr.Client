using System;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.WinForms
{
    /// <summary>
    ///     Customization for WinForms/WPF applications
    /// </summary>
    //Created for future use (to not break binary compatibility).
    public class WinformsErrorReportContext : ErrorReporterContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WinformsErrorReportContext" /> class.
        /// </summary>
        /// <param name="reporter">Class that caught the exception.</param>
        /// <param name="exception">Exception that was thrown.</param>
        public WinformsErrorReportContext(object reporter, Exception exception) : base(reporter, exception)
        {
        }
    }
}