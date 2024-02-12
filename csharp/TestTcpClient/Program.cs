using nic_weber.TestTcpClient;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices(services =>
{
    services.AddHostedService<Worker>();
});

IHost host = builder.Build();
host.Run();

