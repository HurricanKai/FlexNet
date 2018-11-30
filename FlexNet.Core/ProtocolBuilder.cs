using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlexNet.Core
{
    public class ProtocolBuilder
    {
        /// <summary>
        /// Create a new, empty Protocol Builder using a Id Type
        /// </summary>
        /// <typeparam name="TId">The Type used for Ids</typeparam>
        /// <returns>a new, empty Protocol Builder</returns>
        public static ProtocolBuilder Create<TId>()
        {
            return new ProtocolBuilder(typeof(TId));
        }
        /// <summary>
        /// Create a new, empty Protocol Builder using <paramref name="idType"/> as Id Type
        /// </summary>
        /// <param name="idType">Used as Id Type</param>
        /// <returns>a new, empty Protocol Builder</returns>
        public static ProtocolBuilder Create(Type idType)
        {
            return new ProtocolBuilder(idType);
        }

        private readonly Type _idType;
        private ILengthHeader _lengthBehaviour;
        private List<PacketDefinition> _packets;
        private Dictionary<Type, INetworkAccessor> _accessors;
        private IIdHeader _idHeader;
        private IIdResolver _idResolver;

        internal ProtocolBuilder(Type idType)
        {
            if (idType is null)
                throw new ArgumentNullException(nameof(idType));

            _idType = idType;
            _packets = new List<PacketDefinition>();
            _accessors = new Dictionary<Type, INetworkAccessor>();
        }

        /// <summary>
        /// Set the Length Behaviour used
        /// </summary>
        /// <param name="behaviour">The Behaviour to use</param>
        /// <returns>This Protocol Builder</returns>
        public ProtocolBuilder LengthBehaviour(ILengthHeader behaviour)
        {
            _lengthBehaviour = behaviour;
            return this;
        }

        public ProtocolBuilder UseIds(IIdHeader header, IIdResolver resolver)
        {
            _idHeader = header;
            _idResolver = resolver;
            return this;
        }

        /// <summary>
        /// Register a new Packet using a PacketBuilder
        /// </summary>
        /// <param name="action">the Action to Perform on the PacketBuilder</param>
        /// <returns>This Protocol Builder</returns>
        public ProtocolBuilder RegisterPacket(Action<PacketBuilder> action)
        {
            var builder = new PacketBuilder(_idType);
            action(builder);
            _packets.Add(builder.Build());

            return this;
        }

        public ProtocolBuilder RegisterAccessor<T, TAccessor>() where TAccessor : INetworkAccessor, new() 
            => RegisterAccessor(typeof(T), typeof(TAccessor));

        public ProtocolBuilder RegisterAccessor(Type type, Type accessorType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (accessorType is null)
                throw new ArgumentNullException(nameof(accessorType));

            if (!typeof(INetworkAccessor).IsAssignableFrom(accessorType))
                throw new ArgumentException($"Cannot Convert to INetworkAccessor. Make shure to implement INetworkAccessor on {accessorType.FullName}.", nameof(accessorType));

            var a = (INetworkAccessor)Activator.CreateInstance(accessorType);
            if (!a.ProcessableTypes.Contains(type))
                throw new ArgumentException($"{accessorType.FullName} can not Process {type.FullName}. Please Register an Appropiate Accessor", nameof(accessorType));

            _accessors.Add(type, a);
            return this;
        }

        /// <summary>
        /// Build the Protocol
        /// </summary>
        /// <returns>a Protocol Definition</returns>
        public ProtocolDefinition Build(IDelegateBuilder builder)
        {
            var v = new ProtocolDefinition();
            v.Accessors = _accessors;
            v.Packets = _packets.Select((def) =>
            {
                def.WriteDelegate = builder.BuildWriteDelegate(def, _accessors);
                def.ReadDelegate = builder.BuildReadDelegate(def, _accessors);
                return def;
            }).ToArray();
            v.IdType = _idType;
            v.LengthHeader = _lengthBehaviour;
            v.IdHeader = _idHeader;
            v.IdResolver = _idResolver;
            if (v.LengthHeader != null)
                v.LengthHeader.Protocol = v;
            if (v.IdHeader != null)
                v.IdHeader.Protocol = v;
            if (v.IdResolver != null)
                v.IdResolver.Protocol = v;
            return v;
        }
    }
}
