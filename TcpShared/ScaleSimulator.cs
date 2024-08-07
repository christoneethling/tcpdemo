﻿using Microsoft.Extensions.Logging;
using System;
using System.Timers;

namespace TcpShared
{
    public class ScaleSimulator : IDisposable
    {
        private readonly TcpListenerService _tcpListenerService;
        private readonly ILogger logger;
        private readonly Timer timer;
        private int portNo;
        private int currentWeight = 1000;
        private int howManyTimesBeforeWeChange;

        public ScaleSimulator(int portNo, ILogger logger)
        {
            this.portNo = portNo;
            this.logger = logger;
            this._tcpListenerService = new TcpListenerService(portNo, logger);
            // instantiate a timer that fires every 200 milliseconds

            // and sends a message to all connected clients
            timer = new Timer(500);
            timer.Elapsed += TimerElapsed;
            timer.Start();

        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            howManyTimesBeforeWeChange--;
            if (howManyTimesBeforeWeChange <= 0)
            {
                howManyTimesBeforeWeChange = 20;
                currentWeight += 1000;
                if (currentWeight > 56000)
                    currentWeight = 1000;

            }
            var message = $"\u0002  {currentWeight} kg \u0003\r\n";
            logger.LogDebug($"ScaleSimulator {portNo}: Send:{message.Replace("\r\n","")}");
            _tcpListenerService.SendToAllClients(message);
        }

        public void Dispose()
        {
            logger.LogInformation($"ScaleSimulator {portNo}: ScaleSimulator DISPOSE");
            timer.Elapsed -= TimerElapsed;
            timer.Stop();
            timer.Dispose();
        }

        public void Start()
        {
            logger.LogInformation($"ScaleSimulator {portNo}: ScaleSimulator START");
            _tcpListenerService.Listen();
        }

    }
}
