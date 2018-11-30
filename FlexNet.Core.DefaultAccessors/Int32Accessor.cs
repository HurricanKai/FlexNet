using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexNet.Core.DefaultAccessors
{
    public class Int32Accessor : INetworkAccessor
    {
        public Type[] ProcessableTypes { get; } = new Type[] { typeof(Int32) };

        public Object Read(Stream stream)
        {
            var bytes = new byte[4];
            stream.Read(bytes, 0, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        public void Write(Stream stream, Object obj)
        {
            var bytes = BitConverter.GetBytes((Int32)obj);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
