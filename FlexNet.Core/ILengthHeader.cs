using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexNet.Core
{
    public interface ILengthHeader : IHeader<Int32>
    {
        LengthBehaviour Behaviour { get; }
    }
}
