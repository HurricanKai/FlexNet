using System;
using System.Collections.Generic;

namespace FlexNet.Core
{
    public struct ProtocolDefinition
    {
        public PacketDefinition[] Packets { get; internal set; }
        public Type IdType { get; internal set; }
        public ILengthHeader LengthHeader { get; internal set; }
        public IIdHeader IdHeader { get; internal set; }
        internal Dictionary<Type, INetworkAccessor> Accessors { get; set; }

        public T CreateServer<T>() where T : IServer, new()
        {
            var v = new T();
            v.Protocol = this;
            return v;
        }

        public T CreateClient<T>() where T : IClient, new()
        {
            var v = new T();
            v.Protocol = this;
            return v;
        }
    }
}