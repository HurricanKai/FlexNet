using FlexNet.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexNet.Core.Tests
{
    public class PacketBuilderTests
    {
        private IDelegateBuilder nullBuilder;
        private Random rnd;

        [SetUp]
        public void Setup()
        {
            rnd = new Random();
            nullBuilder = new NullDelegateBuilder();
        }

        [Test]
        public void IdTypeNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ProtocolBuilder.Create(null));
        }

        [Test]
        public void CreateGeneric()
        {
            Assert.AreEqual(typeof(Int32), ProtocolBuilder.Create<Int32>().Build(nullBuilder).IdType);
        }

        [Test]
        public void CreateExplicit()
        {
            Assert.AreEqual(typeof(Int32), ProtocolBuilder.Create(typeof(Int32)).Build(nullBuilder).IdType);
        }

        [Test]
        public void AddPacket()
        {
            Assert.True(ProtocolBuilder
                .Create<int>()
                .RegisterPacket((a) => { })
                .Build(nullBuilder).Packets.Length == 1);
        }

        [Test]
        public void RegisterAccessorTypeNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            ProtocolBuilder
                .Create<int>()
                .RegisterAccessor(null, null));
        }

        [Test]
        public void RegisterAccessorAccessorNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            ProtocolBuilder
                .Create<int>()
                .RegisterAccessor(typeof(Byte), null));
        }

        [Test]
        public void RegisterAccessorInvalidAccessorTypeException()
        {
            Assert.Throws<ArgumentException>(() =>
            ProtocolBuilder
                .Create<int>()                  // Clearly not implementing INetworkAccessor
                .RegisterAccessor(typeof(Byte), typeof(Byte)));
        }

        [Test]
        public void RegisterAccessorCannotConvertAccessorTypeException()
        {
            Assert.Throws<ArgumentException>(() =>
            ProtocolBuilder
                .Create<int>()                  // Only "Implementing" Byte
                .RegisterAccessor(typeof(Int32), typeof(NullAccessor)));
        }


        [Test]
        public void IdWrongTypeException()
        {
            Assert.Throws<ArgumentException>(() =>
            ProtocolBuilder
                .Create<int>()
                .RegisterPacket((a) => a
                    .Id("this is the wrong type")));
        }

        [Test]
        public void IdSaved()
        {
            int Id = rnd.Next();
            Assert.AreEqual(Id, ProtocolBuilder
                .Create<int>()
                .RegisterPacket((a) => a
                    .Id(Id)).Build(nullBuilder).Packets.First().Id);
        }

        [Test]
        public void IdNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            ProtocolBuilder
                .Create<int>()
                .RegisterPacket((a) => a
                    .Id(null)));
        }

        [Test]
        public void BindingTypeNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            ProtocolBuilder
                .Create<int>()
                .RegisterPacket((a) => a
                    .BindingType(null)));
        }

        [Test]
        public void SetBindingTypeBeforeFieldBinding()
        {
            Assert.Throws<InvalidOperationException>(() =>
            ProtocolBuilder
                .Create<int>()
                .RegisterPacket((a) => a
                    .BindField(nameof(SamplePacket.Field))));
        }

        [Test]
        public void SetBindingTypeBeforePropertyBinding()
        {
            Assert.Throws<InvalidOperationException>(() =>
            ProtocolBuilder
                .Create<int>()
                .RegisterPacket((a) => a
                    .BindProperty(nameof(SamplePacket.Field))));
        }

        [Test]
        public void BindingType()
        {
            var def = ProtocolBuilder
                .Create<int>()
                .RegisterPacket((a) => a
                    .Id(0)
                    .BindingType<SamplePacket>()
                ).Build(nullBuilder);
            Assert.True(def.Packets.First().Binding == typeof(SamplePacket));
        }

        [Test]
        public void FieldBinding()
        {
            var def = ProtocolBuilder
                .Create<int>()
                .RegisterPacket((a) => a
                    .Id(0)
                    .BindingType<SamplePacket>()
                    .BindField(nameof(SamplePacket.Field))
                ).Build(nullBuilder);
            Assert.True(def.Packets.First().Bindings.Length == 1, "Wrong Length");
            Assert.True(def.Packets.First().Bindings.First() == typeof(SamplePacket).GetField(nameof(SamplePacket.Field)), "Binding Type Wrong");
        }

        [Test]
        public void FieldBindingPropertyException()
        {
            Assert.Throws<ArgumentException>(() => ProtocolBuilder
                .Create<int>()
                .RegisterPacket((a) => a
                    .Id(0)
                    .BindingType<SamplePacket>()
                    .BindField(nameof(SamplePacket.Property))
                ));
        }

        [Test]
        public void PropertyBinding()
        {
            var def = ProtocolBuilder
                .Create<int>()
                .RegisterPacket((a) => a
                    .Id(0)
                    .BindingType<SamplePacket>()
                    .BindProperty(nameof(SamplePacket.Property))
                ).Build(nullBuilder);
            Assert.True(def.Packets.First().Bindings.Length == 1, "Wrong Length");
            Assert.True(def.Packets.First().Bindings.First() == typeof(SamplePacket).GetProperty(nameof(SamplePacket.Property)), "Binding Type Wrong");
        }

        [Test]
        public void PropertyBindingFieldException()
        {
            Assert.Throws<ArgumentException>(() => ProtocolBuilder
                .Create<int>()
                .RegisterPacket((a) => a
                    .Id(0)
                    .BindingType<SamplePacket>()
                    .BindProperty(nameof(SamplePacket.Field))
                ));
        }
    }
}