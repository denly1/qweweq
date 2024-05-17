using System;
using System.Windows;

namespace Client_server1
{
    public partial class MainWindow : Window
    {
        private const string ServerIpAddress = "26.89.178.79";
        private const int ServerPort = 4899;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateChat_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Введите имя пользователя");
                return;
            }

            TcpServer server = new TcpServer(ServerIpAddress, ServerPort);
            server.Start();

            ChatWindow chatWindow = new ChatWindow(server, username);
            chatWindow.Show();
            this.Close();
        }

        private void ConnectChat_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Введите имя пользователя");
                return;
            }

            TcpClient client = new TcpClient(ServerIpAddress, ServerPort);
            client.Connect();

            ChatWindow chatWindow = new ChatWindow(client, username);
            chatWindow.Show();
            this.Close();
        }
    }
}
