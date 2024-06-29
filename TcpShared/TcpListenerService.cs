using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TcpShared
{
    public class TcpListenerService
    {
        Socket _socket;
        bool _listening;
        ICollection<InboundConnection> inboundConnections = new List<InboundConnection>();

        public async void Listen()
        {
            Console.WriteLine("Start Listening");
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 13);
            _socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(ipEndPoint);
            _socket.Listen(10);
            _listening = true;
            Console.WriteLine("Listening........");

            while (_listening)
            {
                try
                {
                    var client = await Task.Factory.FromAsync<Socket>(_socket.BeginAccept, _socket.EndAccept, null);
                     AcceptClient(client);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception 1: {e.Message}");
                }
            }
        }
        public void Stop()
        {
            _listening = false;
            _socket.Close();
        }   

        public void SendToAllClients(string data)
        {
            var i = 0;
            foreach (var connection in inboundConnections)
            {
                i++;
                Console.WriteLine($"Sending to client {i}");
                connection.SendString(data);
            }   
        }

        private async void AcceptClient(Socket client)
        {
            var inboundConnection = new InboundConnection(client);
            inboundConnections.Add(inboundConnection);
            Console.WriteLine($"Client connected -->  Total connections={inboundConnections.Count}");
            var buffer = BufferPool.Instance.Checkout();
            try
            {
                using (var ns = new NetworkStream(client, true))
                {
                    while (_listening && client.Connected)
                    {
                        try
                        {
                            var count = await ns.ReadAsync(buffer.Array, buffer.Offset, buffer.Count);
                            if (count == 0)
                            {
                                // Client disconnected normally.
                                Console.WriteLine("Client disconnected.....");
                                break;
                            }
                            else
                            {
                                var dataReceived = new ArraySegment<byte>(buffer.Array, buffer.Offset, count);
                                var data = System.Text.Encoding.UTF8.GetString(dataReceived.Array, dataReceived.Offset, dataReceived.Count);
                                Console.WriteLine($"Data received: {data}");
                                //OnDataRead(dataReceived);
                            }
                        }
                        catch (IOException e)
                        {
                            if (e.Message.Contains("An existing connection was forcibly closed by the remote host"))
                                Console.WriteLine($"The remote client died or crashed without disconnecting");
                            else
                                throw;
                        }
                    }
                }
            }
            finally
            {
                inboundConnections.Remove(inboundConnection);
                Console.WriteLine($"Client disconnected --> Total connections={inboundConnections.Count}");
                BufferPool.Instance.CheckIn(buffer);
            }
        }

    }
}
