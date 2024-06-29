// See https://aka.ms/new-console-template for more information
using System.Text;
using InfrastructureStandard.Instrumentation.Comms;

Console.WriteLine("Client2 waiting...");
Task.Delay(4000).Wait();
Console.WriteLine("Client2 starting...");
var tcpClientWrapper = new TcpClientWrapper("127.0.0.1", 13);
tcpClientWrapper.Open();
Console.WriteLine("Client2 Opnened");
Console.Write("Press (S)end (R)eceive (D)isconnect, (C)onnect, or (Q)uit: ");

var input = Console.ReadKey();
while (input.Key != ConsoleKey.Q)
{
    if (input.Key == ConsoleKey.S)
    {
        // send the message to the server
        var message = "Hello from client2";
        var messageBytes = Encoding.UTF8.GetBytes(message);
        tcpClientWrapper.Write(messageBytes);
        Console.WriteLine($"Client2 sent: \"{message}\"");
    }
    else if (input.Key == ConsoleKey.D)
    {
        Console.WriteLine($"disconnect\\Close!!!");
        tcpClientWrapper.Close();
    }
    else if (input.Key == ConsoleKey.C)
    {
        Console.WriteLine($"Connect!!!");
        tcpClientWrapper.Open();
    }
    else
    {
        Console.WriteLine($"Unknown command: {input}");
    }
    Console.Write("Press (S)end (R)eceive (D)isconnect, (C)onnect, or (Q)uit: ");
    input = Console.ReadKey();
}

tcpClientWrapper.Close();
tcpClientWrapper.Dispose();

Console.WriteLine("Client2 Done");
Console.ReadLine();
