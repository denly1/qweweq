using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class TcpClient
{
    private System.Net.Sockets.TcpClient _client;
    private NetworkStream _stream;
    public event EventHandler<string> MessageReceived;

    private readonly string _ipAddress;
    private readonly int _port;

    public TcpClient(string ipAddress, int port)
    {
        _ipAddress = ipAddress;
        _port = port;
    }

    public async Task Connect()
    {
        _client = new System.Net.Sockets.TcpClient(_ipAddress, _port);
        _stream = _client.GetStream();
        await Task.Run(() => ReceiveMessages());
    }

    private async void ReceiveMessages()
    {
        byte[] buffer = new byte[1024];
        while (_client.Connected)
        {
            int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                MessageReceived?.Invoke(this, message);
            }
        }
    }

    public async Task SendMessage(string message)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        await _stream.WriteAsync(buffer, 0, buffer.Length);
    }

    public void Disconnect()
    {
        _client.Close();
    }
}
