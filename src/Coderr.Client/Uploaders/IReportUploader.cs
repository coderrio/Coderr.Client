using System;
using codeRR.Client.Contracts;

namespace codeRR.Client.Uploaders
{
    /// <summary>
    ///     Defines the contract for all classes that will send the error report to a specific destination.
    /// </summary>
    /// <seealso cref="UploadToCoderr" />
    /// <remarks>
    ///     <para>
    ///         This contract gives a best effort promsie. i.e. it only promises to deliver the DTOs as soon as possible, but
    ///         reports
    ///         might be trashed if retried a certain amount of times or during a certain period of time. The
    ///         <c>UploadFailed</c>
    ///         event MUST be invoked by implementations when DTOs are thrown away.
    ///     </para>
    /// </remarks>
    public interface IReportUploader
    {
        /// <summary>
        ///     Have given up the attempt to deliver a report.
        /// </summary>
        /// <remarks>
        ///     The reason is implementation specific but is typically configured using a set of properties.
        /// </remarks>
        event EventHandler<UploadReportFailedEventArgs> UploadFailed;

        /// <summary>
        ///     Send feedback for a previously submitted error report
        /// </summary>
        /// <param name="feedback">Feedback to send</param>
        void UploadFeedback(FeedbackDTO feedback);

        /// <summary>
        ///     Upload report
        /// </summary>
        /// <param name="report">Error report that should be uploaded to the service</param>
        void UploadReport(ErrorReportDTO report);
    }
}