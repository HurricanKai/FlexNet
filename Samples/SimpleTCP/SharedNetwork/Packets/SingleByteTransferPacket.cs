using System;
using System.Collections.Generic;
using System.Text;

namespace FlexNet.Samples.SimpleTCP.SharedNetwork.Packets
{
    public struct SingleByteTransferPacket
    {
        public byte Data;

        public override String ToString()
        {
            return $"SingleByte: {Data}";
        }
    }
}
