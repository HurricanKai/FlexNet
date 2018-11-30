using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexNet.Core
{
    public class DefaultIdHeader : IIdHeader
    {
        public ProtocolDefinition Protocol { private get; set; }

        public Object Read(Stream stream)
            => Protocol.Accessors[Protocol.IdType].Read(stream);

        public void Write(Stream stream, Object value)
            => Protocol.Accessors[Protocol.IdType].Write(stream, value);
    }
}
