using System;
using System.Net.Sockets;
using System.Text;
using Bebop.Runtime;
using Cowboy.Contracts;
using NetCoreServer;

namespace Server
{
    public class ChatSession : TcpSession
    {
        public ChatSession(TcpServer server) : base(server) {}

        protected override void OnConnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} connected!");

            // Send invite message
            string message = "Hello from TCP chat! Please send a message or '!' to disconnect the client!";
            SendAsync(message);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            // todo: replace with Bebop
            var record = ChatMessage.Decode(buffer);
            
            //string message =  Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + record.Text);

            // Multicast message to all connected sessions
            var message = ChatMessage.Encode(new ChatMessage {Text =$"Server says {record.Text}" });
            Server.Multicast(message);

            // If the buffer starts with '!' the disconnect the current session
            if (record.Text == "!")
                Disconnect();
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP session caught an error with code {error}");
        }
    }
}