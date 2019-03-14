using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LabWork3
{
    public class Client
    {
        public string Name { get; private set; }

        private Soket _soket;
        private bool _clientRun = true;

        public Client(string name)
        {
            Name = name;
        }

        public void CreateConnection(Soket soket)
        {
            _soket = soket;
        }

        private async void Listener(object obj)
        {
            while (_clientRun)
            {
                var message = _soket.GetMessage(Name);

                if (message == null)
                    await Task.Delay(200);
            }
        }

        public void ClientStop()
        {
            _clientRun = false;
        }

        ~Client()
        {
            ClientStop();
        }
    }

    public class Hacker
    {
        private Soket _soket;
        private bool _stop = false;

        public Hacker(Soket soket)
        {
            _soket = soket;
        }

        public void TCPReset()
        {
            ThreadPool.QueueUserWorkItem(Reset);
        }

        public void TCPFlooding()
        {
            ThreadPool.QueueUserWorkItem(Flooding);
        }

        public void TCPHijacking()
        {
            ThreadPool.QueueUserWorkItem(Hijactiong);
        }

        private void Flooding(object obj)
        {
            var connection = _soket.GetBuffer().FirstOrDefault();
            var flood = new string('0', 170);
            var count = 0;

            while (!_stop)
            {
                connection.Value.Insert(0, flood);
                Console.WriteLine($"Hacker: flood package number {++count}");
            }
        }

        private void Hijactiong(object obj)
        {
            var connection = _soket.GetBuffer().FirstOrDefault();

            while (!_stop)
            {
                try
                {
                    var package = connection.Value.First();
                    connection.Value.Clear();

                    var message = string.Empty;
                    for (int i = 160; i < package.Length; i += 8)
                        message += (char)Convert.ToInt32(package.Substring(i, 8), 2);

                    Console.WriteLine($"Hacker: Intercepted packet {message}");
                }
                catch { }
            }
        }

        private void Reset(object obj)
        {
            var buffer = _soket.GetBuffer();

            while (!_stop)
            {
                try
                {
                    var connection = buffer.FirstOrDefault();
                    StringBuilder con = new StringBuilder(connection.Value[0]);
                    con[109] = '1';
                    connection.Value[0] = con.ToString();
                    break;
                }
                catch { }
            }

            Console.WriteLine("Hacker: TCP Reset!");
        }

        public void HackStop()
        {
            _stop = true;
        }
    }
}
