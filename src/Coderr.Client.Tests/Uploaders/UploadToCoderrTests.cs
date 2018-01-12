using System;
using System.Collections.Generic;
using System.Net;
using codeRR.Client.Contracts;
using codeRR.Client.Uploaders;
using FluentAssertions;
using Griffin.Net.Channels;
using Griffin.Net.Protocols.Http;
using Xunit;
using HttpListener = Griffin.Net.Protocols.Http.HttpListener;

namespace Coderr.Client.Tests.Uploaders
{
    public class UploadToCoderrTests : IDisposable
    {
        public UploadToCoderrTests()
        {
            _listener = new HttpListener();
            _listener.Start(IPAddress.Any, 0);
            _listener.MessageReceived += OnMessage;
        }


        public void Dispose()
        {
            _listener.Stop();
        }

        private readonly HttpListener _listener;

        private void OnMessage(ITcpChannel channel, object message)
        {
            var msg = (HttpRequest) message;
            channel.Send(msg.CreateResponse());
        }

        [Fact]
        public void Should_throw_exception_when_ThrowExceptions_and_report_upload_fails()
        {
            _listener.Stop();
            var collectionDto = new ContextCollectionDTO("MyName", new Dictionary<string, string> {{"Key", "Val"}});
            var report = new ErrorReportDTO("aaa", new ExceptionDTO(new Exception()), new[] {collectionDto});

            var uri = new Uri($"http://localhost:{_listener.LocalPort}/coderr/");
            var sut = new UploadToCoderr(uri, "api", "secret", () => false, () => true);
            Action actual = () => sut.UploadReport(report);

            actual.ShouldThrow<Exception>();
        }

        [Fact]
        public void Should_not_throw_exception_when_ThrowExceptions_is_false_and_report_upload_fails()
        {
            _listener.Stop();
            var collectionDto = new ContextCollectionDTO("MyName", new Dictionary<string, string> {{"Key", "Val"}});
            var report = new ErrorReportDTO("aaa", new ExceptionDTO(new Exception()), new[] {collectionDto});

            var uri = new Uri($"http://localhost:{_listener.LocalPort}/coderr/");
            var sut = new UploadToCoderr(uri, "api", "secret", () => false, () => false);
            sut.UploadReport(report);
        }

        [Fact]
        public void Should_not_throw_exception_when_ThrowExceptions_is_true_and_report_upload_succeeds()
        {
            var uri = new Uri($"http://localhost:{_listener.LocalPort}/coderr/");
            var collectionDto = new ContextCollectionDTO("MyName", new Dictionary<string, string> {{"Key", "Val"}});
            var report = new ErrorReportDTO("aaa", new ExceptionDTO(new Exception()), new[] {collectionDto});

            var sut = new UploadToCoderr(uri, "api", "secret", () => false, () => true);
            sut.UploadReport(report);
        }

        [Fact]
        public void Should_throw_exception_when_ThrowExceptions_and_feedback_upload_fails()
        {
            _listener.Stop();
            var feedbackDto = new FeedbackDTO {Description = "Hello world"};

            var uri = new Uri($"http://localhost:{_listener.LocalPort}/coderr/");
            var sut = new UploadToCoderr(uri, "api", "secret", () => false, () => true);
            Action actual = () => sut.UploadFeedback(feedbackDto);

            actual.ShouldThrow<Exception>();
        }

        [Fact]
        public void Should_not_throw_exception_when_ThrowExceptions_is_false_and_feedback_upload_fails()
        {
            _listener.Stop();
            var feedbackDto = new FeedbackDTO {Description = "Hello world"};

            var uri = new Uri($"http://localhost:{_listener.LocalPort}/coderr/");
            var sut = new UploadToCoderr(uri, "api", "secret", () => false, () => false);
            sut.UploadFeedback(feedbackDto);
        }

        [Fact]
        public void Should_not_throw_exception_when_ThrowExceptions_is_true_and_feedback_upload_succeeds()
        {
            var uri = new Uri($"http://localhost:{_listener.LocalPort}/coderr/");
            var feedbackDto = new FeedbackDTO {Description = "Hello world"};

            var sut = new UploadToCoderr(uri, "api", "secret", () => false, () => true);
            sut.UploadFeedback(feedbackDto);
        }
    }
}