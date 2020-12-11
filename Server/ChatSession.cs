using System;
using System.Linq;
using System.Net.Sockets;
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
            var message = "Hello from TCP chat! Please send a message or '!' to disconnect the client!";
            SendAsync(message);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            var message = NetworkMessage.Decode(buffer);
            
            // if the record is ChatMessage then HandleChat is invoked..
            BebopMirror.HandleRecord(
                message.IncomingRecord.ToArray(),
                (uint)message.IncomingOpCode.GetValueOrDefault(),
                this
            );
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP session caught an error with code {error}");
        }
    }
}