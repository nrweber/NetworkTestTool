namespace nic_weber.NetworkTestTool;

public class NetworkToolHostedService : IHostedService
{
    private readonly ILogger<NetworkToolHostedService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private TestTcpSender _sender = default!;
    private TestTcpListener _listener = default!;

    public NetworkToolHostedService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<NetworkToolHostedService>();
        _loggerFactory = loggerFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted Service started");

        _listener = new TestTcpListener(5000, _loggerFactory.CreateLogger<TestTcpListener>());
        _listener.Start();
        _sender = new TestTcpSender("127.0.0.1", 5000, _loggerFactory.CreateLogger<TestTcpSender>());
        _sender.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted Service stopped");
        _listener.Stop();
        _sender.Stop();

        return Task.CompletedTask;
    }
}
