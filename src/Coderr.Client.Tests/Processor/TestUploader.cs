using System;
using codeRR.Client.Contracts;
using codeRR.Client.Uploaders;

namespace Coderr.Client.Tests.Processor
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