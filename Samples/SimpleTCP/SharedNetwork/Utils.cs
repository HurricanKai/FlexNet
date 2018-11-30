using FlexNet.Builders.ExpressionDelegateBuilder;
using FlexNet.Core;
using FlexNet.Core.DefaultAccessors;
using FlexNet.Core.LengthBehaviours;
using FlexNet.Samples.SimpleTCP.SharedNetwork.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlexNet.Samples.SimpleTCP.SharedNetwork
{
    public static class Utils
    {
        public static ProtocolDefinition GetNetworkDefinition()
            => ProtocolBuilder
                .Create<int>()
                .LengthBehaviour(new DefaultDynamicInt32LengthBehaviour())
                .UseIds(new DefaultIdHeader(), new DelegateIdMapper((id, protocol)
                    => protocol.Packets.First(x => ((int)x.Id) == (int)id),
                    (def, protocol) => def.Id))
                .RegisterDefaultAccessors()
                .RegisterPacket((a) => a
                    .Id(0x00)
                    .BindingType<SingleByteTransferPacket>()
                    .BindField(nameof(SingleByteTransferPacket.Data))
                ).Build(new ExpressionDelegateBuilder());
    }
}
