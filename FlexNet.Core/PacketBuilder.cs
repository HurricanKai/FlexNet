using System;
using System.Collections.Generic;
using System.Reflection;

namespace FlexNet.Core
{
    /// <summary>
    /// Builder used to Create a Packet.
    /// Used by Protocol Builder. Cannot be Created Externally
    /// </summary>
    public class PacketBuilder
    {
        private Type _idType;
        private object _id;
        private Type _binding;
        private List<MemberInfo> _bindings;

        internal PacketBuilder(Type idType)
        {
            _idType = idType;
            _bindings = new List<MemberInfo>();
        }

        /// <summary>
        /// Set the ID of this Packet
        /// </summary>
        /// <param name="id">Needs to be of the before passed in Type</param>
        /// <returns>This Packet Builder</returns>
        public PacketBuilder Id(object id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            if (id.GetType() != _idType)
                throw new ArgumentException("Needs to be of passed in Id Type", nameof(id));

            _id = id;
            return this;
        }

        /// <summary>
        /// Set Binding Type
        /// </summary>
        /// <typeparam name="T">Generic Type of the Binding Type</typeparam>
        /// <returns>This Packet Builder</returns>
        public PacketBuilder BindingType<T>() => BindingType(typeof(T));

        /// <summary>
        /// Set Binding Type
        /// </summary>
        /// <param name="bindingType">The Binding Type</param>
        /// <returns>This Packet Builder</returns>
        public PacketBuilder BindingType(Type bindingType)
        {
            if (bindingType is null)
                throw new ArgumentNullException(nameof(bindingType));

            _binding = bindingType;
            return this;
        }

        /// <summary>
        /// Marks a *field* of the Binding Type to be Serialized.
        /// BindingType has to be set beforehand.
        /// </summary>
        /// <param name="fieldName">Name of the Field</param>
        /// <returns>This Packet Builder</returns>
        public PacketBuilder BindField(string fieldName)
        {
            if (_binding is null)
                throw new InvalidOperationException("Binding Type has to be set before calling " + nameof(BindField));

            var info = _binding.GetField(fieldName);
            if (info is null)
                throw new ArgumentException("could not find Field, are you looking for " + nameof(BindProperty), nameof(fieldName));

            _bindings.Add(info);
            return this;
        }
        /// <summary>
        /// Marks a *property* of the Binding Type to be Serialized.
        /// BindingType has to be set beforehand.
        /// </summary>
        /// <param name="propertyName">Name of the Field</param>
        /// <returns>This Packet Builder</returns>
        public PacketBuilder BindProperty(string propertyName)
        {
            if (_binding is null)
                throw new InvalidOperationException("Binding Type has to be set before calling " + nameof(BindProperty));

            var info = _binding.GetProperty(propertyName);
            if (info is null)
                throw new ArgumentException("could not find Property, are you looking for " + nameof(BindField), nameof(propertyName));

            _bindings.Add(info);
            return this;
        }

        public PacketDefinition Build()
        {
            return new PacketDefinition()
            {
                Id = _id,
                Binding = _binding,
                Bindings = _bindings.ToArray()
            };
        }
    }
}