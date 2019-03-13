using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
