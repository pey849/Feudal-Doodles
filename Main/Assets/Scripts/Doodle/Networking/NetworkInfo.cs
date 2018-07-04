using System;
using System.Net;

namespace Doodle.Networking
{
    /// <summary>
    /// Information about a specific network address and port pair. 
    /// </summary>
    public struct NetworkInfo : IEquatable<NetworkInfo>
    {
        /// <summary>
        /// The IP address.
        /// </summary>
        public readonly string Address;

        /// <summary>
        /// The port number.
        /// </summary>
        public readonly int Port;

        public NetworkInfo( string address, int port )
        {
            Address = address;
            Port = port;
        }

        public NetworkInfo( IPAddress address, int port )
        {
            Address = address.ToString();
            Port = port;
        }

        public override string ToString()
        {
            return string.Format( "{0}:{1}", Address, Port );
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode() ^ Port;
        }

        public override bool Equals( object obj )
        {
            if( obj is NetworkInfo )
                return Equals( (NetworkInfo) obj );

            return false;
        }

        public bool Equals( NetworkInfo other )
        {
            return other.Address == Address
                && other.Port == Port;
        }
    }
}
