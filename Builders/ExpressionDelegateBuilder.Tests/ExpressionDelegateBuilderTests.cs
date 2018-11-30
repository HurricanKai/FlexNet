using FlexNet.Core;
using FlexNet.Core.DefaultAccessors;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlexNet.Builders.ExpressionDelegateBuilder.Tests
{
    public class ExpressionDelegateBuilderTests
    {
        private Random rnd;

        [SetUp]
        public void Setup()
        {
            rnd = new Random();
        }

        [Test]
        public void ReadField()
        {
            var def = ProtocolBuilder
                .Create<int>()
                .RegisterDefaultAccessors()
                .RegisterPacket((a) => a
                    .Id(0x00)
                    .BindingType<SamplePacket>()
                    .BindField(nameof(SamplePacket.Field))
                ).Build(new ExpressionDelegateBuilder());

            var packetDef = def.Packets.First();

            var vals = new byte[1];
            rnd.NextBytes(vals);
            var val = vals[0];
            using (var stream = new MemoryStream(new byte[] { val }))
            {
                var readPacket = (SamplePacket)packetDef.ReadDelegate(stream);
                Assert.NotNull(readPacket);
                Assert.AreEqual(val, readPacket.Field);
            }
        }

        [Test]
        public void ReadProperty()
        {
            var def = ProtocolBuilder
                .Create<int>()
                .RegisterDefaultAccessors()
                .RegisterPacket((a) => a
                    .Id(0x00)
                    .BindingType<SamplePacket>()
                    .BindProperty(nameof(SamplePacket.Property))
                ).Build(new ExpressionDelegateBuilder());

            var packetDef = def.Packets.First();

            var vals = new byte[1];
            rnd.NextBytes(vals);
            var val = vals[0];
            using (var stream = new MemoryStream(new byte[] { val }))
            {
                var readPacket = (SamplePacket)packetDef.ReadDelegate(stream);
                Assert.NotNull(readPacket);
                Assert.AreEqual(val, readPacket.Property);
            }
        }

        [Test]
        public void WriteField()
        {
            var def = ProtocolBuilder
                .Create<int>()
                .RegisterDefaultAccessors()
                .RegisterPacket((a) => a
                    .Id(0x00)
                    .BindingType<SamplePacket>()
                    .BindField(nameof(SamplePacket.Field))
                ).Build(new ExpressionDelegateBuilder());

            var packetDef = def.Packets.First();

            var vals = new byte[1];
            rnd.NextBytes(vals);
            var val = vals[0];
            using (var stream = new MemoryStream())
            {
                var packet = new SamplePacket()
                {
                    Field = val,
                };
                packetDef.WriteDelegate(stream, packet);
                stream.Position = 0;
                Assert.AreEqual(val, (byte)stream.ReadByte());
            }
        }

        [Test]
        public void WriteProperty()
        {
            var def = ProtocolBuilder
                .Create<int>()
                .RegisterDefaultAccessors()
                .RegisterPacket((a) => a
                    .Id(0x00)
                    .BindingType<SamplePacket>()
                    .BindProperty(nameof(SamplePacket.Property))
                ).Build(new ExpressionDelegateBuilder());

            var packetDef = def.Packets.First();

            var vals = new byte[1];
            rnd.NextBytes(vals);
            var val = vals[0];
            using (var stream = new MemoryStream())
            {
                var packet = new SamplePacket()
                {
                    Property = val,
                };
                packetDef.WriteDelegate(stream, packet);
                stream.Position = 0;
                Assert.AreEqual(val, (byte)stream.ReadByte());
            }
        }
    }
}
