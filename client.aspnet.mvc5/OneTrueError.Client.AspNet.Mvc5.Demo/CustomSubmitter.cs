using System;
using OneTrueError.Client.Contracts;
using OneTrueError.Client.Uploaders;

namespace OneTrueError.Client.AspNet.Mvc5.Demo
{
    public class CustomSubmitter : IReportUploader
    {
        public void UploadFeedback(FeedbackDTO feedback)
        {
        }

        public void UploadReport(ErrorReportDTO report)
        {
        }

        public event EventHandler<UploadReportFailedEventArgs> UploadFailed;
    }
}