namespace nic_weber.NetworkTestTool;

using System.Net;
using System.Net.Sockets;

public class TestTcpListner : IDisposable
{
    private Thread? _runningThread = null;
    private CancellationTokenSource _tokenSource = new();

    private readonly ILogger<TestTcpListner> _logger;

    private IPEndPoint _endPoint;

    private bool _disposed;
    
    public TestTcpListner(int port, ILogger<TestTcpListner> logger)
    {
        _logger = logger;

        _endPoint = new(IPAddress.Any, port);
    }

    public TestTcpListner(string ipAddress, int port, ILogger<TestTcpListner> logger)
    {
        _logger = logger;

        _endPoint = new(IPAddress.Parse(ipAddress), port);
    }

    public void Start()
    {
        if(_runningThread is null)
        {
            _tokenSource.Dispose();
            _tokenSource = new();
            _runningThread = new Thread(new ThreadStart(WorkerThread)) { IsBackground = true };
            _runningThread.Start();
        }
    }

    public void Stop()
    {
        if(_runningThread is not null)
        {
            _tokenSource.Cancel();
            _runningThread.Join();
            _runningThread = null;
        }
    }

    private void WorkerThread()
    {
        try
        {
            TcpListener listener = new(_endPoint);
            listener.Start();

            while(_tokenSource.IsCancellationRequested)
            {
                TcpClient client = listener.AcceptTcpClient();

                NetworkStream stream = client.GetStream();

                while(_tokenSource.IsCancellationRequested == false)
                {
                    var data = new Byte[512];
                    Int32 bytes = stream.ReadAsync(data, 0, data.Length, _tokenSource.Token).Result;

                    if(bytes == 0)
                        break;

                    //Just print out what was received as a string
                    _logger.LogInformation(System.Text.Encoding.Default.GetString(data));
                }
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
                    _runningThread.Join(500);
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
