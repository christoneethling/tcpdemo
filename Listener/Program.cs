using System.IO;
using System.Text;
using TcpShared;

Console.WriteLine("Listener");

var tcpListenerService = new TcpListenerService();
Task.Run(() =>
{
    //var tcpListenerServiceBasic = new TcpListenerServiceBasic();
    //await tcpListenerServiceBasic.StartAsync();
    
    tcpListenerService.Listen();
});




Console.Write("Press (S)end (R)eceive (D)isconnect, (C)onnect, or (Q)uit: ");
var input = Console.ReadKey();
try
{
    while (input.Key != ConsoleKey.Q)
    {
        if (input.Key == ConsoleKey.S)
        {
            tcpListenerService.SendToAllClients("Hello"); 
            Console.WriteLine($"Wrote");
        }
        else
            Console.WriteLine($"Unknown command: {input}");
        Console.WriteLine("Press 'S' to send a message, 'D' to disconnect, 'C' to connect, or 'Q' to quit");
        input = Console.ReadKey();
    }
}
finally
{
    tcpListenerService.Stop();
}


Console.WriteLine("Listener Done");
Console.ReadLine();

