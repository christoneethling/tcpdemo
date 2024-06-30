using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        Socket _socket;
        bool _listening;
        public int PortNo { get; }
        ICollection<InboundConnection> inboundConnections = new List<InboundConnection>();

        public TcpListenerService(int portNo, ILogger logger)
        {
            this._logger = logger;
            PortNo = portNo;
        }   
        public async void Listen()
        {
            _logger?.LogInformation($"TcpListenerService {PortNo}: Start Listening");
            var ipEndPoint = new IPEndPoint(IPAddress.Any, PortNo);
            _socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(ipEndPoint);
            _socket.Listen(10);
            _listening = true;
           _logger?.LogInformation($"TcpListenerService {PortNo}: Listening........");

            while (_listening)
            {
                try
                {
                    var client = await Task.Factory.FromAsync<Socket>(_socket.BeginAccept, _socket.EndAccept, null);
                     AcceptClient(client);
                }
                catch (Exception e)
                {
                   _logger?.LogError(e, $"TcpListenerService {PortNo}: Exception 1: {e.Message}");
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
               _logger?.LogDebug($"TcpListenerService {PortNo}: Sending to client {i}");
                connection.SendString(data);
            }   
        }

        private async void AcceptClient(Socket client)
        {
            var inboundConnection = new InboundConnection(client);
            inboundConnections.Add(inboundConnection);
           _logger?.LogInformation($"TcpListenerService {PortNo}: Client connected -->  Total connections={inboundConnections.Count}");
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
                               _logger?.LogInformation($"TcpListenerService {PortNo}: Client disconnected.....");
                                break;
                            }
                            else
                            {
                                var dataReceived = new ArraySegment<byte>(buffer.Array, buffer.Offset, count);
                                var data = System.Text.Encoding.UTF8.GetString(dataReceived.Array, dataReceived.Offset, dataReceived.Count);
                               _logger?.LogInformation($"TcpListenerService {PortNo}: Data received: {data}");
                                //OnDataRead(dataReceived);
                            }
                        }
                        catch (IOException e)
                        {
                            if (e.Message.Contains("An existing connection was forcibly closed by the remote host"))
                               _logger?.LogInformation($"TcpListenerService {PortNo}: The remote client died or crashed without disconnecting 1");
                            else
                                throw new Exception("IOException in AcceptClient of Wayware", e);
                        }
                        catch (SocketException e2)
                        {
                            if (e2.Message.Contains(" the connected party did not properly respond after a period of time, or established connection failed "))
                               _logger?.LogInformation($"TcpListenerService {PortNo}: The remote client died or crashed without disconnecting 2");
                            else
                                throw new Exception("SocketException in AcceptClient of Wayware", e2);
                        }
                    }
                }
            }
            catch (Exception e3)
            {
                throw new Exception("Other exception in AcceptClient of Wayware", e3);
            }
            finally
            {
                inboundConnections.Remove(inboundConnection);
               _logger?.LogInformation($"TcpListenerService {PortNo}: Client disconnected --> Total connections={inboundConnections.Count}");
                BufferPool.Instance.CheckIn(buffer);
            }
        }

    }
}
