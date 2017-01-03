using System;
using OneTrueError.Client.Reporters;

namespace OneTrueError.Client.Wpf.Contexts
{
    /// <summary>
    ///     Customization for WPF applications
    /// </summary>
    //Created for future use (to not break binary compatibility).
    public class WpfErrorReportContext : ErrorReporterContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WpfErrorReportContext" /> class.
        /// </summary>
        /// <param name="reporter">Class that caught the exception.</param>
        /// <param name="exception">Exception that was thrown.</param>
        public WpfErrorReportContext(object reporter, Exception exception) : base(reporter, exception)
        {
        }
    }
}
