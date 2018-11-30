using FlexNet.Builders.ExpressionDelegateBuilder;
using FlexNet.Core;
using FlexNet.Core.DefaultAccessors;
using FlexNet.Core.LengthBehaviours;
using FlexNet.Samples.SimpleTCP.SharedNetwork.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlexNet.Samples.SimpleTCP.SharedNetwork
{
    public static class Utils
    {
        public static ProtocolDefinition GetNetworkDefinition()
            => ProtocolBuilder
                .Create<int>()
                .LengthBehaviour(new DefaultDynamicInt32LengthBehaviour())
                .IdHeader(new DefaultIdHeader())
                .RegisterDefaultAccessors()
                .RegisterPacket((a) => a
                    .Id(0x00)
                    .BindingType<SingleByteTransferPacket>()
                    .BindField(nameof(SingleByteTransferPacket.Data))
                ).Build(new ExpressionDelegateBuilder());
    }
}
