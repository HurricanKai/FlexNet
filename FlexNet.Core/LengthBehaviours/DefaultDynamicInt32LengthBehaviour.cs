using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexNet.Core.LengthBehaviours
{
    public class DefaultDynamicInt32LengthBehaviour : ILengthHeader
    {
        public LengthBehaviour Behaviour => LengthBehaviour.Dynamic;

        public ProtocolDefinition Protocol { private get; set; }

        public Int32 Read(Stream stream)
            => (Int32)Protocol.Accessors[typeof(Int32)].Read(stream);

        public void Write(Stream stream, Int32 value)
            => Protocol.Accessors[typeof(Int32)].Write(stream, value);
    }
}
