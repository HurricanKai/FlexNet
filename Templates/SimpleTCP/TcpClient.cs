using FlexNet.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlexNet.Templates.SimpleTCP
{
    public class TcpClient : IClient
    {
        private struct PacketSendInfo
        {
            public object obj;
            public Type t;
            public PacketDefinition def;
        }


        public ProtocolDefinition Protocol { private get; set; }
        public event PacketReceived OnPacketReceived;
        public delegate void PacketReceived(object data, Type packetType);
        private System.Net.Sockets.TcpClient _client;
        private bool _isRemoteClient; // If this Client was Created using a Pre-Accepted Client.
        private CancellationTokenSource _cts;
        private Task readTask;
        private Task writeTask;
        private NetworkStream _stream;
        private Queue<PacketSendInfo> _sendQueue;
        private SemaphoreSlim _streamSemaphore;

        /// <summary>
        /// Create a Client using a Pre-accepted TcpClient
        /// </summary>
        /// <param name="client">the Pre-Accepted client.</param>
        internal TcpClient(System.Net.Sockets.TcpClient client, ProtocolDefinition def, PacketReceived packetReceived)
        {
            _cts = new CancellationTokenSource();
            _client = client;
            _stream = _client.GetStream();
            _isRemoteClient = true;
            _sendQueue = new Queue<PacketSendInfo>();
            _streamSemaphore = new SemaphoreSlim(1, 1);
            Protocol = def;
            OnPacketReceived += packetReceived;
            SetupThreads();
        }

        public TcpClient()
        {
            _client = new System.Net.Sockets.TcpClient();
            _isRemoteClient = false;
            _cts = new CancellationTokenSource();
            _isRemoteClient = false;
            _sendQueue = new Queue<PacketSendInfo>();
            _streamSemaphore = new SemaphoreSlim(1, 1);
        }

        public void Connect(IPEndPoint remoteEP)
        {
            if (_isRemoteClient)
                throw new InvalidOperationException("Remote Clients may not be reconnected");

            _cts.Cancel();
            _cts = new CancellationTokenSource();
            _client.Connect(remoteEP);
            _stream = _client.GetStream();
            SetupThreads();
        }

        public void SendPacket<T>(T obj) => SendPacket(typeof(T), obj);

        public void SendPacket(Type t, object obj)
        {
            var def = Protocol.Packets.FirstOrDefault(x => x.Binding == t);
            if (def.Id == null)
                throw new ArgumentException($"Unknown Packet Type \"{t.FullName}\"", nameof(t));
            _sendQueue.Enqueue(new PacketSendInfo()
            {
                t = t,
                obj = obj,
                def = def
            });
        }

        private void SetupThreads()
        {
            readTask = ReadLoop(_cts.Token);
            writeTask = WriteLoop(_cts.Token);
        }

        private async Task WriteLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                while (_sendQueue.Count <= 0)
                    await Task.Delay(10);
                var info = _sendQueue.Dequeue();
                using (var initDataStream = new MemoryStream())
                {
                    Protocol.IdHeader.Write(initDataStream, Protocol.IdResolver.MapPacketToId(info.def));
                    info.def.WriteDelegate(initDataStream, info.obj);
                    using (var lengthStream = new MemoryStream())
                    {
                        Protocol.LengthHeader.Write(lengthStream, (int)initDataStream.Position);
                        initDataStream.Position = 0;
                        await initDataStream.CopyToAsync(lengthStream);
                        int totalLength = (int)lengthStream.Position;
                        lengthStream.Position = 0;
                        await _streamSemaphore.WaitAsync();
                        try
                        {
                            await lengthStream.CopyToAsync(_stream, totalLength, ct);
                        }
                        finally
                        {
                            _streamSemaphore.Release();
                        }
                    }
                }
            }
        }

        private async Task ReadLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                while (_client.Available <= 0)
                    await Task.Delay(10);
                int length;
                byte[] data;
                await _streamSemaphore.WaitAsync(ct);
                try
                {
                    length = Protocol.LengthHeader.Read(_stream);
                    data = new byte[length];
                    await _stream.ReadAsync(data, 0, length);
                }
                finally
                {
                    _streamSemaphore.Release();
                }
                using (var stream = new MemoryStream(data))
                {
                    var id = Protocol.IdHeader.Read(stream);
                    var packetDef = Protocol.IdResolver.MapIdToPacket(id);
                    var obj = packetDef.ReadDelegate(stream);
                    OnPacketReceived?.Invoke(obj, packetDef.Binding);
                }
            }
        }

        public void Dispose()
        {
            _sendQueue.Clear();
            _cts.Cancel();
            _cts?.Dispose();
            _client?.Dispose();
            _stream?.Dispose();
            _streamSemaphore?.Dispose();
        }
    }
}
