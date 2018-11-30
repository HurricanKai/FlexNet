using System;
using System.Collections.Generic;
using System.Text;

namespace FlexNet.Core
{
    public class DelegateIdMapper : IIdMapper
    {
        private readonly Func<Object, ProtocolDefinition, PacketDefinition> _idToPacket;
        private readonly Func<PacketDefinition, ProtocolDefinition, Object> _packetToId;

        public ProtocolDefinition Protocol { private get; set; }

        public PacketDefinition MapIdToPacket(Object id) => _idToPacket(id, Protocol);

        public Object MapPacketToId(PacketDefinition def) => _packetToId(def, Protocol);

        public DelegateIdMapper(Func<object, ProtocolDefinition, PacketDefinition> idToPacket, Func<PacketDefinition, ProtocolDefinition, Object> packetToId)
        {
            _idToPacket = idToPacket;
        }
    }
}
