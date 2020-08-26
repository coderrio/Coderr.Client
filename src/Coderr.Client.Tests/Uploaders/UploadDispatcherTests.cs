using Coderr.Client.Config;
using Coderr.Client.Contracts;
using Coderr.Client.Uploaders;
using NSubstitute;
using Xunit;

namespace Coderr.Client.NetStd.Tests.Uploaders
{
    public class UploadDispatcherTests
    {
        [Fact]
        public void should_invoke_uploaders_when_a_new_report_is_being_uploaded()
        {
            var config = new CoderrConfiguration();
            var uploader = Substitute.For<IReportUploader>();
            var report = Substitute.For<ErrorReportDTO>();

            var sut = new UploadDispatcher(config);
            sut.Register(uploader);
            sut.Upload(report);

            uploader.Received().UploadReport(report);
        }

        [Fact]
        public void should_invoke_uploaders_when_a_feedback_is_submitted()
        {
            var config = new CoderrConfiguration();
            var uploader = Substitute.For<IReportUploader>();
            var feedback = Substitute.For<FeedbackDTO>();

            var sut = new UploadDispatcher(config);
            sut.Register(uploader);
            sut.Upload(feedback);

            uploader.Received().UploadFeedback(feedback);
        }
    }
}
