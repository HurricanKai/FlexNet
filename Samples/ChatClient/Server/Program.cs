using FlexNet.Samples.ChatClient.Common;
using FlexNet.Templates.SimpleTCP;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FlexNet.Samples.ChatClient.Server
{
    class Program
    {
        private static TcpServer server;
        static async Task Main(string[] args)
        {
            Console.Title = "Chat Server";
            Console.WriteLine("Chat Server Starting");

            using (server = Protocol.Definition.CreateServer<TcpServer>())
            {
                server.OnPacketReceived += Server_OnPacketReceived;
                server.OnClientConnected += Server_OnClientConnected;
                server.Start(new IPEndPoint(IPAddress.Any, 1234));
                Console.WriteLine("Clients can now Connect");

                while (true)
                {
                    await Task.Delay(1000);
                }
            }
        }

        private static void Server_OnClientConnected(TcpClient client)
        {
            Console.WriteLine("New Client Connected");
        }

        private static void Server_OnPacketReceived(Object data, Type packetType)
        {
            foreach (var client in server.Clients)
            {
                client.SendPacket(packetType, data);

                Console.WriteLine("Broadcasting " + packetType.Name);
            }
        }
    }
}
