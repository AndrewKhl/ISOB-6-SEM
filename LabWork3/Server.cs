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
            Name = "1024";
        }

        public static void CreateConnection(Soket soket)
        {
            _soket = soket;

            ThreadPool.QueueUserWorkItem(Listener);

            Console.WriteLine("TcpServer: Connection succesfull");
        }

        public static void ServerStop()
        {
            if (_serverRun)
                Console.WriteLine("TcpServer: connection closed");
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

                await Task.Delay(500);
            }
        }

        private static bool IsFinPackage(string package)
        {
            int number = Convert.ToInt32(package.Substring(32, 32), 2);

            Console.WriteLine($"TCP server: package accept {number}");

            if (package[109] == '1')
                ServerStop();

            return package[111] == '1';
        }

        private static void PackageBuild(List<string> packages)
        {
            string message = string.Empty;

            foreach (var pac in packages)
            {
                for (int i = 160; i < pac.Length; i += 8)
                    message += (char)Convert.ToInt32(pac.Substring(i, 8), 2);
            }

            Console.WriteLine($"Received message: {message}");
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

            var byteMessage = GetByteArray(message);
            _receivedMessages[client] = true;

            for (int i = 0; i < byteMessage.Length; i += 40)
            {
                string tcpHead = GetTcpHead(client, sender, i + 40 >= byteMessage.Length - 1);
                string data = byteMessage.Substring(i);

                if (data.Length > 40)
                    data = data.Substring(0, 40);

                _buffer[client].Add(tcpHead + data);

                await Task.Delay(200);
            }
        }

        private string GetByteArray(string message)
        {
            var bytes = Encoding.Default.GetBytes(message);

            string ans = string.Empty;

            foreach (var b in bytes)
                ans += Convert.ToString(b, 2).PadLeft(8, '0');

            return ans;
        }

        public string GetMessage(string client)
        {
            if (!_buffer.ContainsKey(client) || !_receivedMessages.ContainsKey(client) || !_receivedMessages[client] || _buffer[client].Count == 0)
                return null;

            _receivedMessages[client] = false;
            
            string message = _buffer[client].FirstOrDefault();

            int number = Convert.ToInt32(message.Substring(32, 32), 2);

            if (Convert.ToInt32(message.Substring(16, 16), 2).ToString() != "0")
                Console.WriteLine($"Client: package sent {number}");

            _buffer[client].Remove(message);

            return message;
        }

        public void ReceivedMessage(string client)
        {
            _receivedMessages[client] = true;
        }

        private string GetTcpHead(string client, string sender, bool fin = false)
        {
            var destinationPort = Convert.ToString(int.Parse(client), 2).PadLeft(16, '0');
            var sourcePort = Convert.ToString(int.Parse(sender), 2).PadLeft(16, '0');

            var sequenseNumber = Convert.ToString(_sn, 2).PadLeft(32, '0');
            var acknowledgmentNumber = Convert.ToString(_sn, 2).PadLeft(32, '0');

            var dateOffset = "1111";
            var reserved = "000000";
            var URG = "0";
            var ACK = "0";
            var PSH = "0";
            var RST = "0";
            var SYN = (_sn++ == 0) ? "1" : "0";
            var FIN = (fin) ? "1" : "0";

            var window = Convert.ToString(123, 2).PadLeft(16, '0');
            var checksum = "0101010101010101";
            var urgentPointer = "0".PadLeft(16, '0');

            return destinationPort + sourcePort + sequenseNumber + acknowledgmentNumber + dateOffset + reserved +
                URG + ACK + PSH + RST + SYN + FIN + window + checksum + urgentPointer;
        }

        public Dictionary<string, List<string>> GetBuffer()
        {
            return _buffer;
        }
    }
}
