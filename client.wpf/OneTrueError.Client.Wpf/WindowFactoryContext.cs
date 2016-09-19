using OneTrueError.Client.Contracts;

namespace OneTrueError.Client.Wpf
{
    /// <summary>
    ///     Allows us to pass information to the delegate that is used to create a new error reporting dialog
    /// </summary>
    public class WindowFactoryContext
    {
        /// <summary>
        ///     Context that will be used to collect information about this exception.
        /// </summary>
        public WpfErrorReportContext Context { get; set; }

        /// <summary>
        ///     Generated report
        /// </summary>
        public ErrorReportDTO Report { get; set; }
    }
}
