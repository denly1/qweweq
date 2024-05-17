using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class TcpServer
{
    private TcpListener _listener;
    private List<System.Net.Sockets.TcpClient> _clients;
    private CancellationTokenSource _cancellationTokenSource;
    public event EventHandler<string> MessageReceived;

    private readonly string _ipAddress;
    private readonly int _port;

    public TcpServer(string ipAddress, int port)
    {
        _ipAddress = ipAddress;
        _port = port;
        _clients = new List<System.Net.Sockets.TcpClient>();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Start()
    {
        _listener = new TcpListener(IPAddress.Parse(_ipAddress), _port);
        _listener.Start();
        Task.Run(() => ListenForClientsAsync(_cancellationTokenSource.Token));
    }

    private async Task ListenForClientsAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var client = await _listener.AcceptTcpClientAsync();
            _clients.Add(client);
            Task.Run(() => HandleClientAsync(client, token));
        }
    }

    private async Task HandleClientAsync(System.Net.Sockets.TcpClient client, CancellationToken token)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        while (!token.IsCancellationRequested)
        {
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
            if (bytesRead > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                MessageReceived?.Invoke(this, message);
                BroadcastMessage(message);
            }
        }
    }

    public void BroadcastMessage(string message)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        foreach (var client in _clients)
        {
            if (client.Connected)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        _listener.Stop();
        foreach (var client in _clients)
        {
            client.Close();
        }
    }
}
