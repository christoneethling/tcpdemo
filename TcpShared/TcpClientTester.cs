using InfrastructureStandard.Instrumentation.Comms;
using System;


namespace TcpShared
{
    public class TcpClientTester
    {
        private readonly TcpClientWrapper tcpClientWrapper;

        public TcpClientTester(string hostName, int port)
        {
            this.tcpClientWrapper = new TcpClientWrapper(hostName, port);
            tcpClientWrapper.DataReceived += TcpClientWrapper_DataReceived;
        }

        private void TcpClientWrapper_DataReceived(byte[] data, int bytesRead)
        {
            var dataAsString = System.Text.Encoding.UTF8.GetString(data, 0, bytesRead);
            Console.WriteLine($"Data received: {data}");
        }


        public void Send(string message)
        {
            Console.WriteLine($"Sending message: {message}");
            tcpClientWrapper.Write(message);
        }

        public void Open()
        {
            tcpClientWrapper.Open();
        }
    }
}
