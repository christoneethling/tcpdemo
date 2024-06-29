using System;
using System.Net.Sockets;

namespace TcpShared
{
    public class InboundConnection
    {
        private Socket _socket;
        private ArraySegment<byte> _buffer;

        public InboundConnection(Socket clientSocket)
        {
            _socket = clientSocket;
            _buffer = new ArraySegment<byte>(new byte[4096], 0, 4096);
            StartReceive(); // Start the read async loop.
        }

        private void StartReceive() { }

        private void ReceiveAsyncLoop() { }

        private void OnDataReceived() { }

        public void SendString(string data) 
        { 
            var dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
            _socket.Send(dataBytes);
        }

        public void Send(byte[] data)
        {
            _socket.Send(data);
        }
    }
}
