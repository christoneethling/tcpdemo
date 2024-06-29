using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpShared
{
    public class TcpListenerServiceBasic
    {
        public async Task StartAsync()
        {
            Console.WriteLine("Listener");
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 13);
            TcpListener listener = new TcpListener(ipEndPoint);
            listener.Start();
            using TcpClient handler = await listener.AcceptTcpClientAsync();
            Console.WriteLine("ClientConnected!!!!");

            using NetworkStream stream = handler.GetStream();
            var message = $"📅 {DateTime.Now} 🕛";

            Console.Write("Press (S)end (R)eceive (D)isconnect, (C)onnect, or (Q)uit: ");
            var input = Console.ReadKey();
            try
            {
                while (input.Key != ConsoleKey.Q)
                {
                    if (input.Key == ConsoleKey.S)
                    {
                        var dateTimeBytes = Encoding.UTF8.GetBytes(message);
                        stream?.Write(dateTimeBytes, 0, dateTimeBytes.Length);
                        stream?.Flush();
                        Console.WriteLine($"Sent message: \"{message}\"");

                    }
                    else if (input.Key == ConsoleKey.D)
                    {
                        Console.WriteLine("Close!!!!");
                        handler.Close();
                    }
                    else if (input.Key == ConsoleKey.C)
                    {
                        handler.Connect("127.0.0.1", 13);
                    }
                    else
                    {
                        // create a new buffer
                        var buffer = new byte[1_024];
                        // read the message from the stream
                        var bytesRead = stream.Read(buffer, 0, 1024);
                        // convert the message to a string
                        var messageReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        // print the message
                        Console.WriteLine($"Message received: \"{messageReceived}\"");
                        // Sample output:
                        //     Message received: "📅 8/22/2022 9:07:17 AM 🕛"
                        // read the next key
                    }
                    Console.WriteLine("Press 'S' to send a message, 'D' to disconnect, 'C' to connect, or 'Q' to quit");
                    input = Console.ReadKey();
                }
            }
            finally
            {
                listener.Stop();
            }

        }
    }
}
