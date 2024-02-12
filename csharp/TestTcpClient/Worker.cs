namespace nic_weber.TestTcpClient;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using System.Net.Sockets;
using System.Text;

public class Worker : IHostedService, IDisposable
{
    private Thread? _runningThread = null;
    private CancellationTokenSource _tokenSource = new();

    private readonly ILogger<Worker> _logger;
    private readonly string _ipAddress;
    private readonly int _port;

    private bool _disposed;
    
    public Worker(ILogger<Worker> logger, IConfiguration configruation)
    {
        _logger = logger;

        var tempAddress = configruation.GetValue<string>("IpAddress");
        var tempPort = configruation.GetValue<int?>("Port");

        if(tempAddress is null || tempPort is null)
        {
            throw new ArgumentException("IpAddress and/or Port not defined in appsettings.json");
        }

        _ipAddress = tempAddress;
        _port = (int)tempPort;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if(_runningThread is null)
        {
            _logger.LogInformation("TCP Client starting");
            _tokenSource.Dispose();
            _tokenSource = new();
            _runningThread = new Thread(new ThreadStart(WorkerThread)) { IsBackground = true };
            _runningThread.Start();
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if(_runningThread is not null)
        {
            _logger.LogInformation("TCP Client stopping");
            _tokenSource.Cancel();
            _runningThread.Join();
            _runningThread = null;
        }

        return Task.CompletedTask;
    }

    private void WorkerThread()
    {
        _logger.LogInformation("Starting WorkerThread");
        while(_tokenSource.IsCancellationRequested == false)
        {
            try
            {
                TcpClient client = new(_ipAddress, _port);
                NetworkStream stream = client.GetStream();

                _logger.LogInformation("Connected to {_ipAddress}:{_port}", _ipAddress, _port);

                while(_tokenSource.IsCancellationRequested == false)
                {
                    String testString = $"Data sent @ {DateTime.Now}";
                    var data = Encoding.ASCII.GetBytes(testString);

                    stream.Write(data, 0, data.Length);

                    _logger.LogInformation($"Data sent: '{testString}'");


                    Thread.Sleep(1000);
                }

            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("thread was canceled");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Connection failed");
            }
            
            //Unless the token was cancelled, wait a second before you try to connect again
            if(_tokenSource.IsCancellationRequested == false)
            {
                Thread.Sleep(1000);
            }

        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed == false)
        {
            _disposed = true;
            if (disposing)
            {
                if(_runningThread is not null)
                {
                    _tokenSource.Cancel();
                    _runningThread.Join(1000);
                }
            }
        }
    }


    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
