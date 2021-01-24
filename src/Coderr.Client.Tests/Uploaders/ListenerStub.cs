using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Coderr.Client.Tests.Uploaders
{
    public class ListenerStub : IDisposable
    {
        private readonly byte[] _readBuffer = new byte[65535];
        private readonly TcpListener _listener;
        private readonly string _statusCodeToReturn;
        private readonly ManualResetEvent _triggerEvent = new ManualResetEvent(false);

        public ListenerStub(string statusCode = "204 OK")
        {
            _statusCodeToReturn = statusCode;
            _listener = new TcpListener(IPAddress.Loopback, 0);
            _listener.Start();
            ListenerPort = ((IPEndPoint)_listener.LocalEndpoint).Port;
            _listener.BeginAcceptSocket(OnSocket, null);
        }

        public int ListenerPort { get; }

        public void Dispose()
        {
            _triggerEvent?.Dispose();
            _listener.Stop();
        }

        private void OnSocket(IAsyncResult ar)
        {
            var socket = _listener.EndAcceptSocket(ar);
            socket.Receive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None);
            socket.Shutdown(SocketShutdown.Receive);

            var resp =
                $"HTTP/1.1 {_statusCodeToReturn}\r\nDate: Sun, 10 Mar 2013 19:20:58 GMT\r\nServer: Jonas l33tServer\r\nContent-Length: 0\r\nContent-Type: text/plain\r\nConnection: Close\r\n\r\n";
            var buffer = Encoding.ASCII.GetBytes(resp);
            socket.Send(buffer);
            Thread.Sleep(100);
            socket.Dispose();
            _triggerEvent.Set();
        }

        public bool Wait(int timeout)
        {
            return _triggerEvent.WaitOne(timeout);
        }
    }
}