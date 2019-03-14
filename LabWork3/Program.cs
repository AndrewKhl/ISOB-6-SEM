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
            Console.WriteLine("Please, input message:");
            var message = Console.ReadLine();
            var client = new Client("8080");
            var _soket = new Soket();

            TcpServer.CreateConnection(_soket);

            _soket.SendMessage(TcpServer.Name, client.Name, message);

            var hacker = new Hacker(_soket);
            //hacker.TCPReset();
            // hacker.TCPFlooding();
            hacker.TCPHijacking();

            Console.Read();
            TcpServer.ServerStop();
        }
    }
}
