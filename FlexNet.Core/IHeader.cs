using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexNet.Core
{
    public interface IHeader<T>
    {
        T Read(Stream stream);
        void Write(Stream stream, T value);

        ProtocolDefinition Protocol { set; }
    }
}
