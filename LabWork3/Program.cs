using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabWork3
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client("8080");
            var _soket = new Soket();

            TcpServer.CreateConnection(_soket);

            _soket.SendMessage(TcpServer.Name, client.Name, "Palina Solnishko");

            Console.Read();
            TcpServer.ServerStop();
        }
    }
}
