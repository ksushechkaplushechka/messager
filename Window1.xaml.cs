using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace radmin
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        public Window1(string[] args)
        {
            // Создаем TCP сервер и привязываем его к локальному адресу и порту
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
            server.Start();
            Console.WriteLine("Server started.");

            // Список подключенных пользователей
            List<TcpClient> clients = new List<TcpClient>();

            // Слушаем порт и принимаем подключения
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                // Добавляем нового клиента в список
                clients.Add(client);

                // Создаем новый поток для обработки сообщений от клиента
                Task.Run(() => HandleClient(client, clients));
            }
        }

        static void HandleClient(TcpClient client, List<TcpClient> clients)
        {
            // Получаем имя пользователя от клиента
            byte[] usernameBytes = new byte[1024];
            int bytesRead = client.GetStream().Read(usernameBytes, 0, usernameBytes.Length);
            string username = Encoding.ASCII.GetString(usernameBytes, 0, bytesRead);
            Console.WriteLine($"{username} connected.");

            // Отправляем сообщение о подключении нового пользователя остальным клиентам
            byte[] connectBytes = Encoding.ASCII.GetBytes($"{username} connected.");
            foreach (TcpClient c in clients)
            {
                if (c != client)
                {
                    c.GetStream().Write(connectBytes, 0, connectBytes.Length);
                }
            }

            // Получаем сообщения от клиента и отправляем их остальным клиентам
            while (true)
            {
                byte[] messageBytes = new byte[1024];
                int bytesRead2 = client.GetStream().Read(messageBytes, 0, messageBytes.Length);
                string message = Encoding.ASCII.GetString(messageBytes, 0, bytesRead2);
                Console.WriteLine($"{username}: {message}");

                byte[] sendBytes = Encoding.ASCII.GetBytes($"{username}: {message}");
                foreach (TcpClient c in clients)
                {
                    if (c != client)
                    {
                        c.GetStream().Write(sendBytes, 0, sendBytes.Length);
                    }
                }
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
