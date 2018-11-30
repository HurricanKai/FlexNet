using System;
using System.Collections.Generic;
using System.Text;

namespace FlexNet.Core
{
    public interface IIdMapper
    {
        ProtocolDefinition Protocol { set; }
        PacketDefinition MapIdToPacket(object id);
        object MapPacketToId(PacketDefinition def);
    }
}
