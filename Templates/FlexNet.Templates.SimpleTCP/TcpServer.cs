using FlexNet.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlexNet.Templates.SimpleTCP
{
    public class TcpServer : IServer
    {
        public event TcpClient.PacketReceived OnPacketReceived;
        public event ClientConnected OnClientConnected;
        public delegate void ClientConnected(TcpClient client);
        public ProtocolDefinition Protocol { private get; set; }
        private TcpListener _listener;
        private List<TcpClient> _clients = new List<TcpClient>();
        private CancellationTokenSource _cts;
        private Task _listenerTask;

        public void Start(IPEndPoint endpoint)
        {
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(endpoint);
            _listener.Start();
            _listenerTask = ListeningLoop(_cts.Token);
        }

        private async Task ListeningLoop(CancellationToken ct)
        {
            while (!_cts.IsCancellationRequested)
            {
                while (!_listener.Pending())
                    await Task.Delay(1000);


                var newClient = await _listener.AcceptTcpClientAsync();
                var client = new TcpClient(newClient, this.Protocol);
                client.OnPacketReceived += OnPacketReceived;
                OnClientConnected?.Invoke(client);
                _clients.Add(client);
            }
        }

        public void Dispose()
        {
            _listener.Stop();
            _cts.Cancel();
            _cts.Dispose();
            foreach (var v in _clients)
                v.Dispose();
            _clients.Clear();
        }
    }
}
