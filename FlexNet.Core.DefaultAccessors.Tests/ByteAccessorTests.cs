using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexNet.Core.DefaultAccessors.Tests
{
    public class ByteAccessorTests
    {
        private INetworkAccessor accessor;
        private Random rnd;

        [SetUp]
        public void Setup()
        {
            accessor = new ByteAccessor();
            rnd = new Random();
        }

        [Test]
        public void Reading()
        {
            var vals = new byte[1];
            rnd.NextBytes(vals);
            var val = vals[0];

            using (var stream = new MemoryStream(new byte[] { val }))
            {
                Assert.AreEqual(val, accessor.Read(stream));
            }
        }

        [Test]
        public void Writing()
        {
            var vals = new byte[1];
            rnd.NextBytes(vals);
            var val = vals[0];

            using (var stream = new MemoryStream())
            {
                accessor.Write(stream, val);
                stream.Position = 0;
                Assert.AreEqual(val, (byte)stream.ReadByte());
            }
        }

        [Test]
        public void EOFException()
        {
            using (var stream = new MemoryStream())
            {
                Assert.Throws<EndOfStreamException>(() => accessor.Read(stream));
            }
        }
    }
}
