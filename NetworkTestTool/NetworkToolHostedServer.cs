namespace NetworkTestTool;

public class NetworkToolHostedService : IHostedService
{
    private readonly ILogger<NetworkToolHostedService> _logger;

    public NetworkToolHostedService(ILogger<NetworkToolHostedService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted Service started");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted Service stopped");

        return Task.CompletedTask;
    }
}
