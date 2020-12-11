using System;
using System.Net.Sockets;
using System.Threading;
using Cowboy.Contracts;
using TcpClient = NetCoreServer.TcpClient;

namespace Client
{
    class ChatClient : TcpClient
    {
        public ChatClient(string address, int port) : base(address, port) {}

        public void DisconnectAndStop()
        {
            stop = true;
            DisconnectAsync();
            while (IsConnected)
                Thread.Yield();
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"Chat TCP client connected a new session with Id {Id}");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Chat TCP client disconnected a session with Id {Id}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again
            if (!stop)
                ConnectAsync();
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            var record = ChatMessage.Decode(buffer);
            Console.WriteLine(record.Text);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP client caught an error with code {error}");
        }

        private bool stop;
    }
}