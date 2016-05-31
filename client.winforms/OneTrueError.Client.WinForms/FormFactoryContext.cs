namespace OneTrueError.Reporting.WinForms
{
    /// <summary>
    /// Allows us to pass information to the delegate that is used to create a new error reporting dialog
    /// </summary>
    public class FormFactoryContext
    {
        /// <summary>
        /// The generated report ID which is used to uniquely identify this error report.
        /// </summary>
        public string ReportId { get; set; }

        /// <summary>
        /// Context that will be used to collect information about this exception.
        /// </summary>
        public WinformsErrorReportContext Context { get; set; }
    }
}