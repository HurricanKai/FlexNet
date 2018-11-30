using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexNet.Core.DefaultAccessors
{
    public class ByteAccessor : INetworkAccessor
    {
        public Type[] ProcessableTypes { get; } = new Type[] { typeof(Byte) };

        public Object Read(Stream stream)
        {
            var v = stream.ReadByte();
            if (v == -1)
                throw new EndOfStreamException();
            return (byte)v;
        }

        public void Write(Stream stream, Object obj) => stream.WriteByte((byte)obj);
    }
}
