using System;
using System.Threading.Tasks;
using Bebop.Attributes;
using Bebop.Runtime;
using Cowboy.Contracts;

namespace Server
{
    [RecordHandler]
    public static class ChatMessageHandler
    {
        [BindRecord(typeof(BebopRecord<ChatMessage>))]
        public static Task HandleChatMessage(object state, ChatMessage message)
        {
            var session = (ChatSession) state;
            
            //string message =  Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message.Text);

            // Multicast message to all connected sessions
            var response = ChatMessage.Encode(new ChatMessage {Text =$"Server says {message.Text}" });
            session.Server.Multicast(response);

            // If the buffer starts with '!' the disconnect the current session
            if (message.Text == "!")
                session.Disconnect();

            return Task.CompletedTask;
        }
    }
}