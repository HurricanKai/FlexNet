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
        internal TcpClient(System.Net.Sockets.TcpClient client, ProtocolDefinition def)
        {
            _cts = new CancellationTokenSource();
            _client = client;
            _stream = _client.GetStream();
            _isRemoteClient = true;
            _sendQueue = new Queue<PacketSendInfo>();
            _streamSemaphore = new SemaphoreSlim(1, 1);
            Protocol = def;
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

        public void SendMessage<T>(T obj) => SendMessage(typeof(T), obj);

        public void SendMessage(Type t, object obj)
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
                if (_sendQueue.Count <= 0)
                    await Task.Delay(10);
                var info = _sendQueue.Dequeue();
                using (var initDataStream = new MemoryStream())
                {
                    Protocol.IdHeader.Write(initDataStream, info.def.Id);
                    info.def.WriteDelegate(initDataStream, info.obj);
                    using (var lengthStream = new MemoryStream())
                    {
                        Protocol.LengthHeader.Write(lengthStream, (int)initDataStream.Position);
                        initDataStream.Position = 0;
                        await initDataStream.CopyToAsync(lengthStream);
                        lengthStream.Position = 0;
                        await _streamSemaphore.WaitAsync();
                        try
                        {
                            await lengthStream.CopyToAsync(_stream);
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
                while (!_stream.DataAvailable)
                    await Task.Delay(10);
                int length;
                await _streamSemaphore.WaitAsync(ct);
                try
                {
                    length = Protocol.LengthHeader.Read(_stream);
                }
                finally
                {
                    _streamSemaphore.Release();
                }
                var bytes = new byte[length];
                await _streamSemaphore.WaitAsync();
                try
                {
                    await _stream.ReadAsync(bytes, 0, length);
                }
                finally
                {
                    _streamSemaphore.Release();
                }
                using (var stream = new MemoryStream(bytes))
                {

                    var id = Protocol.IdHeader.Read(stream); // we can safely assume id is the right Type (idType)
                    
                    var packetDef = Protocol.Packets.FirstOrDefault(x => (dynamic)x.Id == (dynamic)id);
                    if (packetDef.Id is null)
                        throw new InvalidOperationException("Unknown Id received");
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
