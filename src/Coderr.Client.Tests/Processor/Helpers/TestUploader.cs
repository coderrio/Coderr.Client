using System;
using Coderr.Client.Contracts;
using Coderr.Client.Uploaders;

namespace Coderr.Client.Tests.Processor.Helpers
{
    public class TestUploader : IReportUploader
    {
        public FeedbackDTO Feedback { get; set; }

        public ErrorReportDTO Report { get; set; }
#pragma warning disable CS0067
        public event EventHandler<UploadReportFailedEventArgs> UploadFailed;
#pragma warning restore CS0067

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