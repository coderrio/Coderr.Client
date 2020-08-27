using System;
using Coderr.Client.Contracts;
using Coderr.Client.Uploaders;

namespace Coderr.Client.Tests.Processor.Helpers
{
    public class TestUploader : IReportUploader
    {
        public FeedbackDTO Feedback { get; set; }

        public ErrorReportDTO Report { get; set; }

        public event EventHandler<UploadReportFailedEventArgs> UploadFailed;

        public void UploadFeedback(FeedbackDTO feedback)
        {
            Feedback = feedback;
        }

        public void UploadReport(ErrorReportDTO report)
        {
            Report = report;
        }
    }
}