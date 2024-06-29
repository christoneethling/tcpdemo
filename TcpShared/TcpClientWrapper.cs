using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace InfrastructureStandard.Instrumentation.Comms
{
    public delegate void CommPortDataReceived(byte[] data, int bytesRead);
    public class TcpClientWrapper : IDisposable
    {

        public event CommPortDataReceived? DataReceived;
        public event EventHandler? ErrorReceived;

        readonly int writeBufferSize = 2048;
        readonly int readBufferSize = 2048;
        readonly int port;
        readonly string hostname;

        TcpClient? tcpClient;
        NetworkStream? tcpClientStream;
        byte[] WriteBuffer;
        byte[] ReadBuffer;
        int CurrentWriteByteCount;
        bool started = false;

        public TcpClientWrapper(string hostname, int port)
        {
            this.hostname = hostname;
            this.port = port;
            WriteBuffer = new byte[writeBufferSize];
            ReadBuffer = new byte[readBufferSize];
            CurrentWriteByteCount = 0;
        }

        public void Open()
        {
            try
            {
                Console.WriteLine($"Opening TcpClientWrapper {hostname}:{port}");
                var ipEndPoint = new IPEndPoint(System.Net.IPAddress.Parse(hostname), port);
                if (tcpClient != null)
                {
                    if (!tcpClient.Connected)
                        tcpClient.Connect(ipEndPoint);
                    return;
                }
				tcpClient = new TcpClient();
                tcpClient.Connect(ipEndPoint);
                Console.WriteLine("Connected!");
                tcpClientStream = tcpClient.GetStream();
                Thread t = new Thread(new ThreadStart(ListenForPackets));
                started = true;
                t.Start();

                Console.WriteLine("################ TcpClientWrapper DONE!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("################ Error opening TcpClient: " + ex.Message);
            }   
        }


        public bool IsOpen => tcpClient == null ? false : tcpClient.Connected;

        public void Close()
        {
            if (tcpClient == null)
                return;
            Console.WriteLine($"Closing TcpClientWrapper {hostname}:{port}");
            tcpClient.Close();
            tcpClient.Dispose();
            tcpClient = null;
            started = false;
        }

        public void Write(byte[] data)
        {
            AddToBufferWriteAndFlush(data);
            WriteAndFlushData();
        }
        public void Write(string data)
        {
            var dataAsBytes = Encoding.ASCII.GetBytes(data); 
            AddToBufferWriteAndFlush(dataAsBytes);
            WriteAndFlushData();
        }

        private void ListenForPackets()
        {
            int bytesRead;
            while (started)
            {
                bytesRead = 0;
                try
                {
                    // Blocks until a message is received from the server
                    if (tcpClientStream != null)
                        bytesRead = tcpClientStream.Read(ReadBuffer, 0, readBufferSize);
                }
                catch
                {
                    //A socket error has occurred
                    Console.WriteLine("A socket error has occurred with the client socket " + tcpClient?.ToString());
                    break;
                }

                if (bytesRead == 0)
                {
                    //The server has disconnected
                    break;
                }
                var dataAsString = Encoding.ASCII.GetString(ReadBuffer, 0, bytesRead);
                Console.WriteLine("DataReceived event: " + dataAsString);

                if (DataReceived != null)
                {
                    // Send off the data for other classes to handle
                    DataReceived(ReadBuffer, bytesRead);
                }
                Thread.Sleep(15);
            }
            started = false;
            Close();
        }

        private void AddToBufferWriteAndFlush(byte[] data)
        {
            if (CurrentWriteByteCount + data.Length > WriteBuffer.Length)
                WriteAndFlushData();

            Array.ConstrainedCopy(data, 0, WriteBuffer, CurrentWriteByteCount, data.Length);
            CurrentWriteByteCount += data.Length;
        }
        private void WriteAndFlushData()
        {
            tcpClientStream?.Write(WriteBuffer, 0, CurrentWriteByteCount);
            tcpClientStream?.Flush();
            CurrentWriteByteCount = 0;
        }

        public void Dispose()
        {
            Close();
            tcpClient?.Dispose();
            tcpClientStream?.Dispose();
        }
    }
}






