using System;
using System.Collections.Generic;
using System.Linq;
using codeRR.Client.Config;
using codeRR.Client.Contracts;

namespace codeRR.Client.Uploaders
{
    /// <summary>
    ///     Invokes all uploaders for every report.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This class uses the <see cref="CoderrConfiguration.QueueReports" /> to determine if uploads should be done in
    ///         the background (i.e. don't fail on errors, attempt again late).
    ///     </para>
    /// </remarks>
    public class UploadDispatcher : IDisposable
    {
        private readonly CoderrConfiguration _configuration;
        private readonly UploadQueue<FeedbackDTO> _feedbackQueue;
        private readonly List<IReportUploader> _uploaders = new List<IReportUploader>();
        private UploadQueue<ErrorReportDTO> _reportQueue;


        /// <summary>
        ///     Creates a new instance of <see cref="UploadDispatcher" />.
        /// </summary>
        /// <param name="configuration">Used to check at runtime of queuing is enabled or not.</param>
        public UploadDispatcher(CoderrConfiguration configuration)
        {
            _configuration = configuration;
            _reportQueue = new UploadQueue<ErrorReportDTO>(UploadReportNow);
            _feedbackQueue = new UploadQueue<FeedbackDTO>(UploadFeedbackNow);
        }


        /// <summary>
        ///     Max number of items that may wait in queue to get uploaded.
        /// </summary>
        public int MaxQueueSize
        {
            get => _reportQueue.MaxQueueSize;
            set => _reportQueue.MaxQueueSize = value;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }


        /// <summary>
        ///     Register an uploader.
        /// </summary>
        /// <param name="uploader">uploader</param>
        public void Register(IReportUploader uploader)
        {
            if (uploader == null) throw new ArgumentNullException("uploader");
            uploader.UploadFailed += OnUploadFailed;
            _uploaders.Add(uploader);
        }

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
        public void Upload(ErrorReportDTO dto)
        {
            _configuration.ReportPreProcessor?.Invoke(dto);
            if (dto == null) throw new ArgumentNullException("dto");
            if (_configuration.QueueReports)
                _reportQueue.Add(dto);
            else
                UploadReportNow(dto);
        }

        /// <summary>
        ///     Upload feedback.
        /// </summary>
        /// <param name="dto">Feedback provided  by the user.</param>
        public void Upload(FeedbackDTO dto)
        {
            if (dto == null) throw new ArgumentNullException("dto");
            if (_configuration.QueueReports)
                _feedbackQueue.Add(dto);
            else
                UploadFeedbackNow(dto);
        }

        /// <summary>
        ///     Dispose pattern
        /// </summary>
        /// <param name="isDisposing">Invoked from the dispose method.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_reportQueue != null)
            {
                _reportQueue.Dispose();
                _reportQueue = null;
            }
        }

        /// <summary>
        ///     For tests
        /// </summary>
        /// <returns></returns>
        internal IReportUploader First()
        {
            return _uploaders.FirstOrDefault();
        }

        private void OnUploadFailed(object sender, UploadReportFailedEventArgs e)
        {
            if (UploadFailed != null)
                UploadFailed(sender, e);
        }

        /// <summary>
        ///     Have given up the attempt to deliver a report.
        /// </summary>
        /// <remarks>
        ///     The reason is implementation specific but is typically configured using a set of properties.
        /// </remarks>
        private event EventHandler<UploadReportFailedEventArgs> UploadFailed;

        private void UploadFeedbackNow(FeedbackDTO dto)
        {
            foreach (var uploader in _uploaders)
                uploader.UploadFeedback(dto);
        }

        private void UploadReportNow(ErrorReportDTO dto)
        {
            foreach (var uploader in _uploaders)
                uploader.UploadReport(dto);
        }
    }
}