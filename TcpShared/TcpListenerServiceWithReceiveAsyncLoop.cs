using System;
using System.Net.Sockets;

namespace TcpShared
{
    public class TcpListenerServiceWithReceiveAsyncLoop
    {
        private Socket _socket;
        private ArraySegment<byte> _buffer;
        public void StartReceive()
        {
            ReceiveAsyncLoop(null);
        }

        // Note that this method is not guaranteed (in fact
        // unlikely) to remain on a single thread across
        // async invocations.
        private void ReceiveAsyncLoop(IAsyncResult result)
        {
            try
            {
                if (result != null)
                {
                    int numberOfBytesRead = _socket.EndReceive(result);
                    if (numberOfBytesRead == 0)
                    {
                        ///OnDisconnected(null); // 'null' being the exception. The client disconnected normally in this case.
                        return;
                    }

                    var newSegment = new ArraySegment<byte>(_buffer.Array, _buffer.Offset, numberOfBytesRead);
                    ///OnDataReceived(newSegment);
                }
                _socket.BeginReceive(_buffer.Array, _buffer.Offset, _buffer.Count, SocketFlags.None, ReceiveAsyncLoop, null);
            }
            catch (Exception ex)
            {
                // Socket error handling here.
            }
        }



        //private Socket _socket;
        //private Stream _targetStream;
        //private SslStream _tlsStream;

        //public void BeginStartTls(AsyncCallback callback, object asyncState)
        //{
        //    _targetStream = _tlsStream = new SslStream(_targetStream);
        //    _tlsStream.BeginAuthenticateAsServer(GetServerCerticate(), callback, asyncState);
        //}




    }
}
