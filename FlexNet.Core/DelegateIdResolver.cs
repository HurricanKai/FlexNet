using System;
using System.Collections.Generic;
using System.Text;

namespace FlexNet.Core
{
    public class DelegateIdResolver : IIdResolver
    {
        private readonly Func<Object, ProtocolDefinition, PacketDefinition> _func;

        public ProtocolDefinition Protocol { private get; set; }

        public PacketDefinition ResolveId(Object id) => _func(id, Protocol);

        public DelegateIdResolver(Func<object, ProtocolDefinition, PacketDefinition> func)
        {
            _func = func;
        }
    }
}
