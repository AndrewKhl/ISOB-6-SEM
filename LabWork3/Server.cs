﻿using System;
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
            while (_serverRun)
            {
                var message = _soket.GetMessage(Name);

                if (message == null)
                    await Task.Delay(200);
            }
        }
    }

    public class Soket
    {
        private Dictionary<string, List<string>> buffer;

        public Soket()
        {
            buffer = new Dictionary<string, List<string>>();
        }

        public void SendMessage(string client, string message)
        {
            if (!buffer.ContainsKey(client))
                buffer[client] = new List<string>();

            CreateTcpPakeges(GetTcpHead(client));
        }

        public string GetMessage(string client)
        {
            return null;
            //return buffer.ContainsKey(client) && buffer[client] != null ? buffer[client] : null;
        }

        private List<byte> GetTcpHead(string client)
        {
            return null;
        }

        private void CreateTcpPakeges(List<byte> head)
        {

        }
    }
}