using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LabWork3
{
    static class TcpServer
    {
        public static string Name { get; private set; }

        private static Soket _soket;
        private static bool _serverRun = true;

        static TcpServer()
        {
            Name = "TcpServer";
        }

        public static void CreateConnection(Soket soket)
        {
            _soket = soket;

            ThreadPool.QueueUserWorkItem(Listener);
        }

        public static void ServerStop()
        {
            _serverRun = false;
        }

        private static async void Listener(object obj)
        {
            var packages = new List<string>();

            while (_serverRun)
            {
                var message = _soket.GetMessage(Name);

                if (message != null)
                {
                    _soket.ReceivedMessage(Name);

                    packages.Add(message);
                    if (IsFinPackage(message))
                    {
                        PackageBuild(packages);
                        packages.Clear();
                    }
                }
                else
                    await Task.Delay(200);

            }
        }

        private static bool IsFinPackage(string packege)
        {
            return packege[110] == '1';
        }

        private static void PackageBuild(List<string> packages)
        {
            string message = string.Empty;

            foreach (var pac in packages)
                message += pac.Substring(160);

            Console.WriteLine($"Send message: {Encoding.Default.GetString(Encoding.Default.GetBytes(message))}");
        }
    }


    public class Soket
    {
        private Dictionary<string, List<string>> _buffer;
        private Dictionary<string, bool> _receivedMessages;
        private int _sn = 0;

        public Soket()
        {
            _buffer = new Dictionary<string, List<string>>();
            _receivedMessages = new Dictionary<string, bool>();
        }

        public async void SendMessage(string client, string sender, string message)
        {
            if (!_buffer.ContainsKey(client))
                _buffer[client] = new List<string>();

            var byteMessage = Encoding.Default.GetBytes(message).ToString();

            for (int i = 0; i < byteMessage.Length; i += 40)
            {
                string tcpHead = GetTcpHead(client, sender, i + 40 < byteMessage.Length - 1);
                string data = byteMessage.Substring(i);

                if (data.Length > 40)
                    data = data.Substring(0, 40);

                _buffer[client].Add(tcpHead + data);

                await Task.Delay(100);
            }

        }

        public string GetMessage(string client)
        {
            if (!_buffer.ContainsKey(client) || !_receivedMessages[client] || _buffer[client].Count == 0)
                return null;

            _receivedMessages[client] = false;
            
            string message = _buffer[client].FirstOrDefault();

            _buffer[client].Remove(message);

            return message;
        }

        public void ReceivedMessage(string client)
        {
            if (!_receivedMessages.ContainsKey(client))
                return;

            _receivedMessages[client] = true;
        }

        private string GetTcpHead(string client, string sender, bool fin = false)
        {
            return null;
        }
    }
}
