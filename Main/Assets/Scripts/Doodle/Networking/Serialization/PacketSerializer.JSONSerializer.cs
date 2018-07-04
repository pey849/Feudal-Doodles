using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using UnityEngine;

namespace Doodle.Networking.Serialization
{
    /// <summary>
    /// The default packet serializer.
    /// </summary>
    /// <remarks> Compression Lib: http://www.icsharpcode.net/OpenSource/SharpZipLib </remarks>
    public class JSONPacketSerializer : PacketSerializer
    {
        private Dictionary<Type, XmlSerializer> Serializers;

        /// <summary>
        /// Creates a new default packet serializer.
        /// </summary>
        public JSONPacketSerializer()
        {
            Serializers = new Dictionary<Type, XmlSerializer>();
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
                ms.Write( Encoding.ASCII.GetBytes( JsonUtility.ToJson( data ) ) );

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

            // Read Packet Type
            var pType = (PacketType) data[0];

            // Reconstruct object
            var type = GetType( pType );
            var json = Encoding.ASCII.GetString( data, 1, data.Length - 1 );
            return JsonUtility.FromJson( json, type );
        }
    }
}
