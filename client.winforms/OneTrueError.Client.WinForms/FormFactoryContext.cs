using OneTrueError.Client.Contracts;

namespace OneTrueError.Client.WinForms
{
    /// <summary>
    ///     Allows us to pass information to the delegate that is used to create a new error reporting dialog
    /// </summary>
    public class FormFactoryContext
    {
        /// <summary>
        ///     Context that will be used to collect information about this exception.
        /// </summary>
        public WinformsErrorReportContext Context { get; set; }

        /// <summary>
        ///     Generated report
        /// </summary>
        public ErrorReportDTO Report { get; set; }
    }
}