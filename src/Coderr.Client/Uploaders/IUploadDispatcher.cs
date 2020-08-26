using System;
using Coderr.Client.Contracts;

namespace Coderr.Client.Uploaders
{
    /// <summary>
    ///     Invokes all uploaders for every report.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Every time a complete error report have been collected it need to be uploaded. But since the internet
    ///         connection might be down, there might delivery failures etc we use
    ///         specific classes for uploaded error reports. Those classes is reponsible of handling all that. This class takes
    ///         care of invoking all uploaders.
    ///     </para>
    /// </remarks>
    /// <seealso cref="UploadQueue{T}" />
    public interface IUploadDispatcher
    {
        /// <summary>
        ///     Register an uploader.
        /// </summary>
        /// <param name="uploader">uploader</param>
        /// <exception cref="ArgumentNullException">uploader</exception>
        void Register(IReportUploader uploader);

        /// <summary>
        ///     Invoke callbacks
        /// </summary>
        /// <param name="dto">Report to be uploaded.</param>
        /// <returns><c>false</c> if any of the callbacks return <c>false</c>; otherwise <c>true</c></returns>
        /// <remarks>
        ///     <para>
        ///         All callbacks will be invoked, even if one of them returns <c>false</c>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">dto</exception>
        void Upload(ErrorReportDTO dto);

        /// <summary>
        ///     Upload feedback.
        /// </summary>
        /// <param name="dto">Feedback provided  by the user.</param>
        /// <exception cref="ArgumentNullException">dto</exception>
        void Upload(FeedbackDTO dto);
    }
}