using System;

namespace Doodle.Networking.Serialization
{
    /// <summary>
    /// Used to mark structs as packets.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false )]
    public class PacketAttribute : Attribute
    {
        /// <summary>
        /// The kind of packet this attribute represents.
        /// </summary>
        public readonly PacketType PacketType;

        /// <summary>
        /// Creates a new packet attribute.
        /// </summary>
        public PacketAttribute( PacketType type )
        {
            PacketType = type;
        }
    }
}
