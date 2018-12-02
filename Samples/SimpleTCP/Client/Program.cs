using FlexNet.Samples.SimpleTCP.SharedNetwork;
using FlexNet.Samples.SimpleTCP.SharedNetwork.Packets;
using FlexNet.Templates.SimpleTCP;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FlexNet.Samples.SimpleTCP.Client
{
    class Program
    {
        private static Random rnd = new Random();
        static async Task Main(string[] args)
        {
            Console.Title = "SimpleTCP Client";
            Console.WriteLine("SimpleTCP Client");

            using (var client = Utils.GetNetworkDefinition()
                                        .CreateClient<TcpClient>())
            {
                client.Connect(new IPEndPoint(IPAddress.Loopback, 1234));

                client.OnPacketReceived += Client_OnPacketReceived;

                while (true)
                {
                    var vals = new byte[1];
                    rnd.NextBytes(vals);
                    var val = vals[0];
                    client.SendPacket<SingleByteTransferPacket>(new SingleByteTransferPacket()
                    {
                        Data = val
                    });
                    Console.WriteLine($"Sending {val}");
                    await Task.Delay(1000);
                }
            }
        }

        private static void Client_OnPacketReceived(Object data, Type packetType)
        {
            Console.WriteLine("Received Packet:");
            Console.WriteLine("Type: " + packetType.Name);
            Console.WriteLine("Data: " + data.ToString());
        }
    }
}
