using System;
using Coderr.Client.Contracts;

namespace Coderr.Client.Processor
{
    /// <summary>
    ///     Used when invoking all <see cref="IReportFilter" /> to determine if the current report may be sent to the
    ///     submitters.
    /// </summary>
    /// <seealso cref="Uploaders" />
    public class ReportFilterContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportFilterContext" /> class.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <exception cref="System.ArgumentNullException">report</exception>
        public ReportFilterContext(ErrorReportDTO report)
        {
            Report = report ?? throw new ArgumentNullException("report");
            CanSubmitReport = true;
        }

        /// <summary>
        ///     True if we may submit the report
        /// </summary>
        /// <value>Default is true</value>
        public bool CanSubmitReport { get; set; }

        /// <summary>
        ///     Created report
        /// </summary>
        public ErrorReportDTO Report { get; }
    }
}