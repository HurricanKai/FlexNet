using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexNet.Core
{
    public interface INetworkAccessor
    {
        Type[] ProcessableTypes { get; }
        object Read(Stream stream);
        void Write(Stream stream, object obj);
    }
}
