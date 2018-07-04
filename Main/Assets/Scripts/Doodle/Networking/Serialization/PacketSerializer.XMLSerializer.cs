using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace Doodle.Networking.Serialization
{
    /// <summary>
    /// The default packet serializer.
    /// </summary>
    /// <remarks> Compression Lib: http://www.icsharpcode.net/OpenSource/SharpZipLib </remarks>
    public class XMLPacketSerializer : PacketSerializer
    {
        private Dictionary<Type, XmlSerializer> Serializers;

        /// <summary>
        /// Creates a new default packet serializer.
        /// </summary>
        public XMLPacketSerializer()
        {
            Serializers = new Dictionary<Type, XmlSerializer>();
        }

        private XmlSerializer GetSerializer( Type type )
        {
            // Create serializer if not encountered yet.
            if( Serializers.ContainsKey( type ) == false )
                Serializers[type] = new XmlSerializer( type );

            return Serializers[type];
        }

        /// <summary>
        /// Deconstructs an object into a byte representation.
        /// </summary>
        public override byte[] Serialize( object data )
        {
            // 
            if( data == null ) throw new ArgumentNullException( "data" );

            var type = data.GetType();

            // Validate type
            var pType = GetPacketType( type );
            using( var ms = new MemoryStream() )
            {
                // Build Packet
                ms.WriteByte( (byte) pType );

                // 
                var serializer = GetSerializer( type );
                using( var tw = new StreamWriter( ms, Encoding.UTF8 ) )
                    serializer.Serialize( tw, data );

                // 
                return Compress( ms.ToArray() );
            }
        }

        /// <summary>
        /// Reconstructs an object from byte representation.
        /// </summary>
        public override object Deserialize( byte[] data )
        {
            // 
            if( data == null ) throw new ArgumentNullException( "data" );

            // 
            data = Decompress( data );

            // 
            using( var ms = new MemoryStream( data ) )
            {
                // Read Packet Type
                var pType = (PacketType) ms.ReadByte();

                // Reconstruct object
                var type = GetType( pType );

                // 
                var serializer = GetSerializer( type );
                return serializer.Deserialize( ms );
            }
        }
    }
}