using codeRR.Client.Contracts;
using codeRR.Client.Reporters;

namespace codeRR.Client.Config
{
    /// <summary>
    /// Used to be able to process exceptions before they are converted into DTOs
    /// </summary>
    /// <param name="context">context info</param>
    /// <seealso cref="CoderrConfiguration.ExceptionPreProcessor"/>
    public delegate void ExceptionPreProcessorHandler(IErrorReporterContext2 context);

    /// <summary>
    /// Used to be able to process error reports before they are delivered.
    /// </summary>
    /// <param name="report">Generated error report</param>
    /// <seealso cref="CoderrConfiguration.ReportPreProcessor"/>
    public delegate void ReportPreProcessorHandler(ErrorReportDTO report);
}