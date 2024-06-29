using InfrastructureStandard.Instrumentation.Comms;
using System;
using System.Text;
using System.Threading.Tasks;


namespace TcpShared
{
    public static class TcpClientTester
    {

        public static void Run(string clientName, string ip, int port)
        {
            Console.WriteLine($"{clientName} waiting...");
            Task.Delay(4000).Wait();
            Console.WriteLine($"{clientName} starting...");
            var tcpClientWrapper = new TcpClientWrapper(ip, port);
            tcpClientWrapper.Open();
            Console.WriteLine($"{clientName} Opened");
            Console.WriteLine("Press (S)end (D)isconnect, (C)onnect or (Q)uit: ");

            var input = Console.ReadKey();
            Console.WriteLine();
            while (input.Key != ConsoleKey.Q)
            {
                if (input.Key == ConsoleKey.S)
                {
                    // send the message to the server
                    var message = $"Hello from {clientName}";
                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    tcpClientWrapper.Write(messageBytes);
                    Console.WriteLine($"{clientName} sent: \"{message}\"");
                }
                else if (input.Key == ConsoleKey.D)
                {
                    Console.WriteLine($"Disconnecting.... ");
                    tcpClientWrapper.Close();
                }
                else if (input.Key == ConsoleKey.C)
                {
                    Console.WriteLine($"Connecting...");
                    tcpClientWrapper.Open();
                }
                else
                {
                    Console.WriteLine($"Unknown command: {input}");
                }
                Console.WriteLine("Press (S)end (D)isconnect, (C)onnect or (Q)uit: ");
                input = Console.ReadKey();
                Console.WriteLine();
            }

            tcpClientWrapper.Close();
            tcpClientWrapper.Dispose();

            Console.WriteLine($"{clientName} Done");
        }
    }
}
