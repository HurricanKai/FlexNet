using FlexNet.Samples.ChatClient.Common;
using FlexNet.Templates.SimpleTCP;
using System;
using System.Net;

namespace FlexNet.Samples.ChatClient.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Chat Client";
            Console.WriteLine("Chat Client Starting...");

            using (var client = Protocol.Definition.CreateClient<TcpClient>())
            {
                Console.WriteLine("Enter Server IP:");
                client.OnPacketReceived += Client_OnPacketReceived;
                client.Connect(new IPEndPoint(IPAddress.Parse(Console.ReadLine()), 1234));
                Console.WriteLine("Connected");

                Console.WriteLine("Enter your name");
                var name = Console.ReadLine();

                Console.WriteLine("You can now send and receive messages");

                while (true)
                {
                    var message = Console.ReadLine();
                    client.SendPacket(new MessagePacket()
                    {
                        Author = name,
                        CreationTime = DateTime.UtcNow,
                        Message = message
                    });
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                }
            }
        }

        private static void Client_OnPacketReceived(Object data, Type packetType)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            if (packetType == typeof(MessagePacket))
            {
                var v = (MessagePacket)data;
                Console.WriteLine($"[{v.CreationTime}] {v.Author} | {v.Message}");
            }

            if (packetType == typeof(BroadcastPacket))
            {
                var v = (BroadcastPacket)data;
                Console.WriteLine();
                Console.WriteLine(v.Content);
                Console.WriteLine();
            }
        }
    }
}
