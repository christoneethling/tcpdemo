﻿using System.IO;
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




Console.WriteLine("Press (S)end (Q)uit: ");
var input = Console.ReadKey();
Console.WriteLine();
try
{
    while (input.Key != ConsoleKey.Q)
    {
        if (input.Key == ConsoleKey.S)
        {
            tcpListenerService.SendToAllClients("Hello"); 
        }
        else
            Console.WriteLine($"Unknown command: {input}");
        Console.WriteLine("Press (S)end  (Q)uit: ");
        input = Console.ReadKey();
        Console.WriteLine();
    }
}
finally
{
    tcpListenerService.Stop();
}


Console.WriteLine("Listener Done");
Console.ReadLine();

