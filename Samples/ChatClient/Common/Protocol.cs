using FlexNet.Core;
using FlexNet.Core.DefaultAccessors;
using FlexNet.Core.LengthBehaviours;
using FlexNet.Builders.ExpressionDelegateBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FlexNet.Samples.ChatClient.Common
{
    public static class Protocol
    {
        public static ProtocolDefinition Definition
        {
            get => ProtocolBuilder
                .Create<int>()
                .RegisterDefaultAccessors()
                .RegisterAccessor<String, UTF8StringAccessor>() // provided by Default Accessors
                .RegisterAccessor<DateTime, DateTimeAccessor>()
                .RegisterAccessor<Color, ColorAccessor>()
                .LengthBehaviour(new DefaultDynamicInt32LengthBehaviour())
                .UseIds(new DefaultIdHeader(), new DelegateIdMapper(
                    (id, def) => def.Packets.First(x => ((int)x.Id) == (int)id),
                    (packet, def) => packet.Id))
                .RegisterPacket((builder) => builder
                    .Id(0x00)
                    .BindingType<MessagePacket>()
                    .BindField(nameof(MessagePacket.Message))
                    .BindField(nameof(MessagePacket.Author))
                    .BindField(nameof(MessagePacket.CreationTime)))
                .RegisterPacket((builder) => builder
                    .Id(0x01)
                    .BindingType<BroadcastPacket>()
                    .BindField(nameof(BroadcastPacket.Content))
                    .BindField(nameof(BroadcastPacket.Color)))
                .Build(new ExpressionDelegateBuilder());
        }
    }
}
