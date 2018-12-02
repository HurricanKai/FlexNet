using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexNet.Core.DefaultAccessors
{
    public class UTF8StringAccessor : INetworkAccessor
    {
        public Type[] ProcessableTypes { get; } = new Type[] { typeof(String) };

        public Object Read(Stream stream)
        {
            var intBytes = new byte[4];
            stream.Read(intBytes, 0, 4);
            var length = BitConverter.ToInt32(intBytes, 0);

            var bytes = new Byte[length];
            stream.Read(bytes, 0, length);
            var s = Encoding.UTF8.GetString(bytes);
            return s;
        }

        public void Write(Stream stream, Object obj)
        {
            var v = (string)obj;
            var bytes = Encoding.UTF8.GetBytes(v);

            var intBytes = BitConverter.GetBytes(bytes.Length);
            stream.Write(intBytes, 0, intBytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
