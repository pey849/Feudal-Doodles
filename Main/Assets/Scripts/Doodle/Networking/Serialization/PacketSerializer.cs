using System;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.GZip;
using System.Linq;
using System.IO;

namespace Doodle.Networking.Serialization
{
    /// <summary>
    /// A packet serializer is used to deconstruct packets into bytes, or reconstruct packets from bytes.
    /// </summary>
    public abstract class PacketSerializer
    {
        private Dictionary<Type, PacketType> TypeToPacket;
        private Dictionary<PacketType, Type> PacketToType;

        /// <summary>
        /// Deconstructs an object into a byte representation.
        /// </summary>
        public abstract byte[] Serialize( object data );

        /// <summary>
        /// Reconstructs an object from byte representation.
        /// </summary>
        public abstract object Deserialize( byte[] data );

        protected static byte[] Compress( byte[] data )
        {
            // return data;
            using( var ms = new MemoryStream() )
            using( var bin = new BinaryWriter( ms ) )
            using( var gz = new GZipOutputStream( ms ) )
            {
                // Store uncompressed size
                bin.Write( (ushort) data.Length );

                // Write compressed data
                gz.Write( data, 0, data.Length );
                gz.Finish();

                // Return compressed blob
                return ms.ToArray();
            }
        }

        protected static byte[] Decompress( byte[] data )
        {
            // return data;
            using( var ms = new MemoryStream( data ) )
            using( var bin = new BinaryReader( ms ) )
            using( var gz = new GZipInputStream( ms ) )
            {
                // Get uncompressed size
                var size = bin.ReadUInt16();

                // Read from compressed stream 
                var buffer = new byte[size];
                gz.Read( buffer, 0, size );

                // Return uncomprsesed blob
                return buffer;
            }
        }

        public PacketSerializer()
        {
            // 
            PacketToType = new Dictionary<PacketType, Type>();
            TypeToPacket = new Dictionary<Type, PacketType>();

            // Find all types
            foreach( var type in GetType().Assembly.GetTypes() )
            {
                var attr = (PacketAttribute) type.GetCustomAttributes( typeof( PacketAttribute ), false ).FirstOrDefault();
                if( attr == null ) continue;
                else
                {
                    // Not a value type
                    if( type.IsValueType == false )
                        throw new InvalidOperationException(
                            string.Format( "Currently, only value types are supported by this serializer. Type '{0}' is invalid.", type.Name ) );

                    // Duplicate
                    if( PacketToType.ContainsKey( attr.PacketType ) )
                        throw new InvalidOperationException(
                             string.Format( "Duplicate packet types! {0} was already defined with {1} conflicting with {2}.",
                             attr.PacketType, PacketToType[attr.PacketType].Name, type.Name ) );

                    // Store
                    TypeToPacket[type] = attr.PacketType;
                    PacketToType[attr.PacketType] = type;
                }
            }
        }

        /// <summary>
        /// Gets the type associated with the packet type.
        /// </summary> 
        protected Type GetType( PacketType type )
        {
            if( PacketToType.ContainsKey( type ) ) return PacketToType[type];
            else throw new ArgumentException( string.Format( "Unable to get the type for packet '{0}'", type ), "type" );
        }

        /// <summary>
        /// Gets the packet type associated with the type.
        /// </summary> 
        protected PacketType GetPacketType( Type type )
        {
            if( TypeToPacket.ContainsKey( type ) ) return TypeToPacket[type];
            else throw new ArgumentException( string.Format( "Unable to get the packet type for '{0}'", type.Name ), "type" );
        }
    }
}
