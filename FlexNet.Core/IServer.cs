using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FlexNet.Core
{
    public interface IServer : IDisposable
    {
        ProtocolDefinition Protocol { set; }
        void Start(IPEndPoint endpoint);
    }
}
