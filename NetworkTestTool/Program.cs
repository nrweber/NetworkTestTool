using NetworkTestTool;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<NetworkToolHostedService>();

var host = builder.Build();
host.Run();
