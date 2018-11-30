using System;
using System.Collections.Generic;
using System.Text;

namespace FlexNet.Core
{
    public interface IIdResolver
    {
        ProtocolDefinition Protocol { set; }
        PacketDefinition ResolveId(object id);
    }
}
