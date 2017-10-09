using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using codeRR.Client.Contracts;
using codeRR.Client.Uploaders;
using Xunit;

namespace codeRR.Client.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class ReportToCoderrTests
    {
        [Fact]
        public void SubmitShouldCorrectlyBuild()
        {
            var apiKey = Guid.NewGuid();
            const string sharedSecret = "SomeSharedSecret";
            var url = new Uri("http://localhost");
            var reporter = new UploadToCoderr(url, apiKey.ToString(), sharedSecret);
            ExceptionDTO DTO = null;
            try
            {
                int a = 100;
                int b = 200;
                var c = a/(b - 200);
            }
            catch (Exception e)
            {
                DTO =new ExceptionDTO(e);
            }

            ErrorReportDTO e1 = new ErrorReportDTO("dsadasdas", DTO,
                                             new[] {new ContextCollectionDTO("name1"), new ContextCollectionDTO("name2")});

            var compress = reporter.CompressErrorReport(e1);
            var deflated = reporter.DeflateErrorReport(compress);

            Assert.True(compress.Length >= 200);
            Assert.Contains("dsadasdas", deflated);
        }

        [Fact]
        public void Submit()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint) listener.LocalEndpoint).Port;
            listener.BeginAcceptSocket(AcceptAndRead, listener);
            ExceptionDTO DTO = null;
            try
            {
                int a = 100;
                int b = 200;
                var c = a / (b - 200);
            }
            catch (Exception e)
            {
                DTO = new ExceptionDTO(e);
            }
            var e1 = new ErrorReportDTO("dsjklsdfl", DTO, new[] { new ContextCollectionDTO("name1"), new ContextCollectionDTO("name2") });

            var url = new Uri("http://localhost:" + port + "/receiver");
            var sut = new UploadToCoderr(url, "cramply", "majs");
            sut.UploadReport(e1);
        }

        byte[] _readBuffer = new byte[65535];
        ManualResetEvent _readEvent=new ManualResetEvent(false);

        private void AcceptAndRead(IAsyncResult ar)
        {
            var listener = (TcpListener) ar.AsyncState;
            var client = listener.EndAcceptSocket(ar);

            client.Receive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None);
            _readEvent.Set();

            var resp =
                "HTTP/1.1 204 OK\r\nDate: Sun, 10 Mar 2013 19:20:58 GMT\r\nServer: Jonas l33tServer\r\nContent-Length: 0\r\nContent-Type: text/plain\r\nConnection: Close\r\n\r\n";
            var buffer = Encoding.ASCII.GetBytes(resp);
            client.Send(buffer);
            client.Shutdown(SocketShutdown.Receive);
            Thread.Sleep(1000);
            client.Disconnect(false);
            client.Dispose();


        }

        

    }
}
