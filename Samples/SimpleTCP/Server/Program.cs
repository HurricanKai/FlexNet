using FlexNet.Samples.SimpleTCP.SharedNetwork;
using FlexNet.Templates.SimpleTCP;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FlexNet.Samples.SimpleTCP.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "SimpleTCP Server";
            Console.WriteLine("SimpleTCP Server");

            using (var server = Utils.GetNetworkDefinition()
                                        .CreateServer<TcpServer>())
            {
                server.Start(new IPEndPoint(IPAddress.Any, 1234));
                server.OnClientConnected += Server_OnClientConnected;
                server.OnPacketReceived += Server_OnPacketReceived;
                while (true)
                    await Task.Delay(1000);
            }
        }

        private static void Server_OnPacketReceived(Object data, Type packetType)
        {
            Console.WriteLine("Received Packet:");
            Console.WriteLine("Type: " + packetType.Name);
            Console.WriteLine("Data: " + data.ToString());
        }

        private static void Server_OnClientConnected(TcpClient client)
        {
            Console.WriteLine("New Client Accepted");
        }
    }
}
