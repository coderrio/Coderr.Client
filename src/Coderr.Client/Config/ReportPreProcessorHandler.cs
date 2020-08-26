using Coderr.Client.Contracts;

namespace Coderr.Client.Config
{
    /// <summary>
    /// Used to be able to process error reports before they are delivered.
    /// </summary>
    /// <param name="report">Generated error report</param>
    /// <seealso cref="CoderrConfiguration.ReportPreProcessor"/>
    public delegate void ReportPreProcessorHandler(ErrorReportDTO report);
}
