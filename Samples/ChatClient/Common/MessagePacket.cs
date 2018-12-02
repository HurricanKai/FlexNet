using System;
using System.Collections.Generic;
using System.Text;

namespace FlexNet.Samples.ChatClient.Common
{
    public class MessagePacket
    {
        public string Message;
        public string Author;
        public DateTime CreationTime;
    }
}
