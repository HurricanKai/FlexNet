using System;
using System.IO;
using FlexNet.Core;

namespace FlexNet.Samples.ChatClient.Common
{
    public class DateTimeAccessor : INetworkAccessor
    {
        public Type[] ProcessableTypes { get; } = new Type[] { typeof(DateTime) };

        public Object Read(Stream stream)
        {
            var bytes = new byte[8];
            stream.Read(bytes, 0, 8);
            return DateTime.FromBinary(BitConverter.ToInt64(bytes, 0));
        }

        public void Write(Stream stream, Object obj)
        {
            var data = (DateTime)obj;
            var bytes = BitConverter.GetBytes(data.ToBinary());
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}