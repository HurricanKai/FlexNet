﻿using FlexNet.Core;
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
            var @byte = stream.ReadByte();
            if (@byte == -1)
                throw new EndOfStreamException();
            return (byte)@byte;
        }

        public void Write(Stream stream, Object obj)
        {
            var color = (Color)obj;
            WriteByte(stream, color.R);
            WriteByte(stream, color.G);
            WriteByte(stream, color.B);
        }

        private void WriteByte(Stream stream, byte @byte)
        {
            stream.WriteByte(@byte);
        }
    }
}
