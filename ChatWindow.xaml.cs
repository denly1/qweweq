using System;
using System.Windows;
using System.Windows.Input;

namespace Client_server1
{
    public partial class ChatWindow : Window
    {
        private TcpClient _client;
        private TcpServer _server;
        private string _username;

        public ChatWindow(TcpClient client, string username)
        {
            InitializeComponent();
            _client = client;
            _username = username;
            _client.MessageReceived += OnMessageReceived;
        }

        public ChatWindow(TcpServer server, string username)
        {
            InitializeComponent();
            _server = server;
            _username = username;
            _server.MessageReceived += OnMessageReceived;
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButton_Click(sender, e);
            }
        }

        private void OnMessageReceived(object sender, string message)
        {
            Dispatcher.Invoke(() => MessagesListBox.Items.Add(message));
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = $"{_username}: {MessageTextBox.Text}";
            if (_client != null)
            {
                _client.SendMessage(message);
            }
            else if (_server != null)
            {
                _server.BroadcastMessage(message);
            }
            MessagesListBox.Items.Add(message);
            MessageTextBox.Clear();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _client?.Disconnect();
            _server?.Stop();
        }
    }
}
