using System;
using System.Net.Http;
using System.Threading.Tasks;
using Coderr.Client.Contracts;
using Coderr.Client.Uploaders;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Coderr.Client.Tests.Uploaders
{
    public class UploadToCoderrTests
    {
        private readonly TestConfig _config;
        
        public UploadToCoderrTests()
        {
            _config = new TestConfig
            {
                QueueReportsAccessor = () => false,
                UploadFunc = message => Task.FromResult(new HttpResponseMessage()),
                FeedbackQueue = Substitute.For<IUploadQueue<FeedbackDTO>>(),
                ReportQueue = Substitute.For<IUploadQueue<ErrorReportDTO>>(),
                ThrowExceptionsAccessor = () => false
            };
        }
        [Fact]
        public void should_make_sure_that_only_the_root_is_specified_in_the_uri_so_that_we_may_change_the_specific_uri_in_future_packages()
        {
            var uri = new Uri("http://localhost/receiver/");

            Action actual = () => new UploadToCoderr(uri, "ada", "cesar", _config);

            actual.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void should_make_sure_that_the_uri_ends_with_a_slash()
        {
            var report = Substitute.For<ErrorReportDTO>();
            HttpRequestMessage msg = null;
            var uri = new Uri("http://localhost");
            _config.UploadFunc = x =>
            {
                msg = x;
                return Task.FromResult(new HttpResponseMessage());
            };
            

            var sut = new UploadToCoderr(uri, "ada", "cesar", _config);
            sut.UploadReport(report);

            msg.RequestUri.ToString().EndsWith("/");
        }

        [Fact]
        public void should_queue_reports_when_specified()
        {
            var report = Substitute.For<ErrorReportDTO>();
            var uri = new Uri("http://localhost");
            _config.QueueReportsAccessor = () => true;

            var sut = new UploadToCoderr(uri, "ada", "cesar", _config);
            sut.UploadReport(report);

            _config.ReportQueue.Received().Enqueue(Arg.Any<ErrorReportDTO>());
        }

        [Fact]
        public void should_queue_feedback_when_specified()
        {
            var dto = Substitute.For<FeedbackDTO>();
            var uri = new Uri("http://localhost");
            _config.QueueReportsAccessor = () => true;

            var sut = new UploadToCoderr(uri, "ada", "cesar", _config);
            sut.UploadFeedback(dto);

            _config.FeedbackQueue.Received().Enqueue(Arg.Any<FeedbackDTO>());
        }

        [Fact]
        public void should_throw_exceptions_when_upload_fails_when_configured()
        {
            _config.ThrowExceptionsAccessor = () => true;
            var dto = Substitute.For<FeedbackDTO>();
            var uri = new Uri("http://localhost");
            _config.UploadFunc = message => throw new InvalidOperationException("err");

            var sut = new UploadToCoderr(uri, "ada", "cesar", _config);
            Action actual = ()=> sut.UploadFeedback(dto);


            actual.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void should_always_throw_when_queing_is_configured_to_allow_retries()
        {
            _config.QueueReportsAccessor = () => true;
            var dto = Substitute.For<FeedbackDTO>();
            var uri = new Uri("http://localhost");
            _config.UploadFunc = message =>
            {
                throw new InvalidOperationException("err");
            };

            var sut = new UploadToCoderr(uri, "ada", "cesar", _config);
            Action actual = () => sut.UploadFeedbackNow(dto);


            actual.Should().Throw<InvalidOperationException>();
        }
    }
}

