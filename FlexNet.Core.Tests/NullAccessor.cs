using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexNet.Core.Tests
{
    public class NullAccessor : INetworkAccessor
    {
        public Type[] ProcessableTypes { get; } = new Type[] { typeof(Byte) };

        public Object Read(Stream stream) => null;

        public void Write(Stream stream, Object obj) { }
    }
}
