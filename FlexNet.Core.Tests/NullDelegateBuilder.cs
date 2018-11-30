using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexNet.Core.Tests
{
    internal class NullDelegateBuilder : IDelegateBuilder
    {
        public Func<Stream, Object> BuildReadDelegate(PacketDefinition definition, Dictionary<Type, INetworkAccessor> accessors) => null;

        public Action<Stream, Object> BuildWriteDelegate(PacketDefinition definition, Dictionary<Type, INetworkAccessor> accessors) => null;
    }
}
