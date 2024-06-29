using Microsoft.Extensions.Configuration;
using TcpShared;

namespace ScaleSimulatorWindowsService
{
    public class ScaleSimulatorWorker : BackgroundService
    {
        private readonly ILogger<ScaleSimulatorWorker> _logger;
        private readonly IConfiguration _configuration;
        private bool initialized = false;

        public ScaleSimulatorWorker(ILogger<ScaleSimulatorWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                if (!initialized)
                {
                    initialized = true;
                    var scaleSettings = _configuration.GetSection("ScaleSettings").Get<List<ScaleParameters>>();
                    foreach (var scaleSetting in scaleSettings)
                    {
                        _logger.LogInformation("Starting Scale: {name} Port: {port}", scaleSetting.Name, scaleSetting.PortNo);  
                        var scaleSimulator = new ScaleSimulator(scaleSetting.PortNo);
                        scaleSimulator.Start();
                    }

                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    public class ScaleParameters
    {
        public string Name { get; set; }
        public int PortNo { get; set; }
    }
}
