using System;
using System.Timers;

namespace TcpShared
{
    public class ScaleSimulator : IDisposable
    {
        private readonly TcpListenerService _tcpListenerService;
        private readonly Timer timer;
        private int currentWeight = 100;

        public ScaleSimulator(int portNo)
        {
            _tcpListenerService = new TcpListenerService(portNo);
            // instantiate a timer that fires every 200 milliseconds

            // and sends a message to all connected clients
            timer = new Timer(200);
            timer.Elapsed += TimerElapsed;
            timer.Start();

        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (currentWeight > 56000)
                currentWeight = 100;
            currentWeight += 100;
            _tcpListenerService.SendToAllClients($"\u0002  {currentWeight} kg \u0003");
        }

        public void Dispose()
        {
            timer.Elapsed -= TimerElapsed;
            timer.Stop();
            timer.Dispose();
        }

        public void Start()
        {
            _tcpListenerService.Listen();
        }

    }
}
