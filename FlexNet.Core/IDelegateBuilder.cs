using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlexNet.Core
{
    public interface IDelegateBuilder
    {
        /// <summary>
        /// Builds a Read Delegate for a given Packet Definition.
        /// </summary>
        /// <param name="definition">The Definition</param>
        /// <returns>The Delegate</returns>
        Func<Stream, Object> BuildReadDelegate(PacketDefinition definition, Dictionary<Type, INetworkAccessor> accessors);
        /// <summary>
        /// Builds a Write Delegate for a given Packet Definition.
        /// </summary>
        /// <param name="definition">The Definition</param>
        /// <returns>The Delegate</returns>
        Action<Stream, Object> BuildWriteDelegate(PacketDefinition definition, Dictionary<Type, INetworkAccessor> accessors);

    }
}
