using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FlexNet.Core
{
    public interface IClient : IDisposable
    {
        ProtocolDefinition Protocol { set; }
        void Connect(IPEndPoint remoteEP);
    }
}
