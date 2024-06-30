using TcpShared;

namespace ScaleSimulatorWindowsService
{
    public class ScaleSimulatorWorker : BackgroundService
    {
        private readonly ILogger<ScaleSimulatorWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IList<ScaleSimulator> scales = new List<ScaleSimulator>();
        private bool initialized = false;

        public ScaleSimulatorWorker(ILogger<ScaleSimulatorWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);

                    if (!initialized)
                    {
                        initialized = true;
                        var scaleSettings = _configuration.GetSection("ScaleSettings").Get<List<ScaleSimulatorConfig>>();
                        if (scaleSettings == null || scaleSettings.Count == 0)
                            _logger.LogError("No Scale Settings Found");
                        else
                            foreach (var scaleSetting in scaleSettings)
                            {
                                _logger.LogInformation("Starting Scale: {name} Port: {port}", scaleSetting?.Name, scaleSetting?.PortNo);
                                if (scaleSetting == null || scaleSetting.PortNo == 0)
                                {
                                    _logger.LogError("Invalid Scale Setting: {name} Port: {port}", scaleSetting?.Name, scaleSetting?.PortNo);
                                    continue;
                                }
                                var scaleSimulator = new ScaleSimulator(scaleSetting.PortNo, _logger);
                                scaleSimulator.Start();
                                scales.Add(scaleSimulator!);
                            }
                    }
                    await Task.Delay(10000, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }


  
        }
    }

    public class ScaleSimulatorConfig
    {
        public string Name { get; set; }
        public int PortNo { get; set; }
    }
}
