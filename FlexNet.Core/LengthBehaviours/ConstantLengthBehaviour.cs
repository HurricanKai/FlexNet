using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexNet.Core.LengthBehaviours
{
    public class ConstantLengthBehaviour : ILengthHeader
    {
        public ProtocolDefinition Protocol
        { set { /* We dont even care */ } }
        public LengthBehaviour Behaviour => LengthBehaviour.Constant;
        public Int32 Read(Stream stream) => _length;
        public void Write(Stream stream, Int32 value) { /* Nope (?) */}

        private readonly int _length;

        public ConstantLengthBehaviour(int length)
        {
            _length = length;
        }
    }
}
