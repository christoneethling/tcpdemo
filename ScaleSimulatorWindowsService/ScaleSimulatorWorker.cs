namespace ScaleSimulatorWindowsService
{
    public class ScaleSimulatorWorker : BackgroundService
    {
        private readonly ILogger<ScaleSimulatorWorker> _logger;

        public ScaleSimulatorWorker(ILogger<ScaleSimulatorWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
