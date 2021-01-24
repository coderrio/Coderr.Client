using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Coderr.Client.Contracts;
using Coderr.Client.Uploaders;
using FluentAssertions;
using Xunit;

#pragma warning disable 4014

namespace Coderr.Client.Tests.Uploaders
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class UploadToCoderrIntegrationTests
    {
        private static ExceptionDTO CreateExceptionDTO()
        {
            try
            {
                var a = 100;
                var b = 200;
                var c = a / (b - 200);
            }
            catch (Exception e)
            {
                return new ExceptionDTO(e);
            }

            throw new InvalidOperationException();
        }

        [Fact]
        public void should_be_able_to_upload_correctly()
        {
            var listener = new ListenerStub();
            var dto = CreateExceptionDTO();
            var e1 = new ErrorReportDTO("dsjklsdfl", dto,
                new[] { new ContextCollectionDTO("name1"), new ContextCollectionDTO("name2") });

            var url = new Uri($"http://localhost:{listener.ListenerPort}/");
            var sut = new UploadToCoderr(url, "cramply", "majs");
            sut.UploadReport(e1);

            listener.Wait(5000).Should().BeTrue();
        }

        [Fact]
        public void should_pack_and_sign_an_entity_correctly()
        {
            var apiKey = Guid.NewGuid();
            const string sharedSecret = "SomeSharedSecret";
            var url = new Uri("http://localhost");
            var reporter = new UploadToCoderr(url, apiKey.ToString(), sharedSecret);
            var dto = CreateExceptionDTO();

            var e1 = new ErrorReportDTO("dsadasdas", dto,
                new[] { new ContextCollectionDTO("name1"), new ContextCollectionDTO("name2") });

            reporter.CreateRequest("http://somewherre.com/report", e1);
        }

        //[Fact] //TODO: Readd
        public void should_report_invalid_app_key()
        {
            var listener = new ListenerStub("400 APP_KEY");
            var dto = CreateExceptionDTO();
            var e1 = new ErrorReportDTO("dsjklsdfl", dto,
                new[] { new ContextCollectionDTO("name1"), new ContextCollectionDTO("name2") });

            var url = new Uri($"http://localhost:{listener.ListenerPort}/");
            var sut = new UploadToCoderr(url, "cramply", "majs");
            try
            {
                sut.UploadReport(e1);
                listener.Wait(5000);
                throw new InvalidOperationException("Test failed");

            }
            catch (InvalidApplicationKeyException)
            {

            }
        }
    }

#pragma warning restore 4014
}