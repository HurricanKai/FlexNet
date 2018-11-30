using System;
using System.IO;
using System.Reflection;

namespace FlexNet.Core
{
    public struct PacketDefinition
    {
        /// <summary>
        /// Given Id. Type is whatever was used to initialize the ProtocolDefinition.
        /// </summary>
        public Object Id { get; internal set; }
        /// <summary>
        /// The Binding Type all <see cref="Bindings"/> refer to
        /// </summary>
        public Type Binding { get; internal set; }
        /// <summary>
        /// Bindings, refering to the <see cref="Binding"/>
        /// </summary>
        public MemberInfo[] Bindings { get; internal set; }
        /// <summary>
        /// Read Delegate Provided by some <see cref="IDelegateBuilder"/>
        /// </summary>
        public Func<Stream, object> ReadDelegate { get; internal set; }
        /// <summary>
        /// Write Delegate Provided by some <see cref="IDelegateBuilder"/>
        /// </summary>
        public Action<Stream, object> WriteDelegate { get; internal set; }
    }
}