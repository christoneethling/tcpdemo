using ScaleSimulatorWindowsService;
using Serilog;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options => { options.ServiceName = "Wayware Scale Simulator"; });
builder.Services.AddSerilog(loggerConfiguration => loggerConfiguration.ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext());
builder.Services.AddHostedService<ScaleSimulatorWorker>();
IHost host = builder.Build();
host.Run();

