using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Coderr.Client.Contracts;
using Coderr.Client.Uploaders;
using FluentAssertions;
using Xunit;

#pragma warning disable 4014

namespace Coderr.Client.NetStd.Tests.Uploaders
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class UploadToCoderrIntegrationTests
    {
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

            var message = reporter.CreateRequest("http://somewherre.com/report", e1);
        }

        private void AcceptAndRead(Task<Socket> task)
        {
            var client = task.Result;

            client.Receive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None);

            var resp =
                $"HTTP/1.1 {_statusCodeToReturn}\r\nDate: Sun, 10 Mar 2013 19:20:58 GMT\r\nServer: Jonas l33tServer\r\nContent-Length: 0\r\nContent-Type: text/plain\r\nConnection: Close\r\n\r\n";
            var buffer = Encoding.ASCII.GetBytes(resp);
            client.Send(buffer);
            client.Shutdown(SocketShutdown.Receive);
            Task.Delay(100).Wait();
            client.Dispose();
            _tcs.SetResult(true);
        }

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

        private readonly byte[] _readBuffer = new byte[65535];
        private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();
        private string _statusCodeToReturn = "204 OK";

        [Fact]
        public void should_be_able_to_upload_correctly()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.AcceptSocketAsync().ContinueWith(AcceptAndRead);
            var dto = CreateExceptionDTO();
            var e1 = new ErrorReportDTO("dsjklsdfl", dto,
                new[] { new ContextCollectionDTO("name1"), new ContextCollectionDTO("name2") });

            var url = new Uri("http://localhost:" + port + "/");
            var sut = new UploadToCoderr(url, "cramply", "majs");
            sut.UploadReport(e1);
            _tcs.Task.Wait(1000);

            listener.Stop();
            _tcs.Task.Status.Should().Be(TaskStatus.RanToCompletion);
        }

        [Fact]
        public void should_report_invalid_app_key()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.AcceptSocketAsync().ContinueWith(AcceptAndRead);
            var dto = CreateExceptionDTO();
            var e1 = new ErrorReportDTO("dsjklsdfl", dto,
                new[] { new ContextCollectionDTO("name1"), new ContextCollectionDTO("name2") });
            _statusCodeToReturn = "400 APP_KEY";

            var url = new Uri("http://localhost:" + port + "/");
            var sut = new UploadToCoderr(url, "cramply", "majs");
            Action e = () => sut.UploadReport(e1);


            e.ShouldThrow<InvalidApplicationKeyException>();
            listener.Stop();
        }
    }

#pragma warning restore 4014
}