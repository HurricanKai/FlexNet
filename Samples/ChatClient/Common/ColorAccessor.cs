using FlexNet.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace FlexNet.Samples.ChatClient.Common
{
    public class ColorAccessor : INetworkAccessor
    {
        public Type[] ProcessableTypes { get; } = new Type[] { typeof(Color) };

        public Object Read(Stream stream)
        {
            return Color.FromArgb(ReadByte(stream), ReadByte(stream), ReadByte(stream));
        }

        private byte ReadByte(Stream stream)
        {
            var v = stream.ReadByte();
            if (v == -1)
                throw new EndOfStreamException();
            return (byte)v;
        }

        public void Write(Stream stream, Object obj)
        {
            var v = (Color)obj;
            WriteByte(stream, v.R);
            WriteByte(stream, v.G);
            WriteByte(stream, v.B);
        }

        private void WriteByte(Stream stream, byte b)
        {
            stream.WriteByte(b);
        }
    }
}
