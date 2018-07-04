using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Doodle.Networking
{
    public partial class NetworkHost
    {
        /// <summary>
        /// Returns a string form this devices IP Address.
        /// </summary>
        public static string GetLocalAddress()
        {
            try
            {
                // Try getting out-bound address of connecting to the internet
                using( Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, 0 ) )
                {
                    socket.Connect( "8.8.8.8", 65530 );
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    return endPoint.Address.ToString();
                }
            }
            catch( Exception )
            {
                // Failed, so just get the first address from local address
                var ipEntry = Dns.GetHostEntry( Dns.GetHostName() );
                var addr = ipEntry.AddressList.FirstOrDefault( a => a.AddressFamily == AddressFamily.InterNetwork );
                return addr.ToString();
            }
        }

        /// <summary>
        /// Determines if the given address is part of the local device's address list.
        /// </summary>
        public static bool IsLocalAddress( string address )
        {
            var ipaddr = IPAddress.Parse( address );
            return IsLocalAddress( ipaddr );
        }

        /// <summary>
        /// Determines if the given address is part of the local device's address list.
        /// </summary>
        public static bool IsLocalAddress( IPAddress address )
        {
            if( IPAddress.Loopback.Equals( address ) ) return true;
            var ipEntry = Dns.GetHostEntry( Dns.GetHostName() );
            return ipEntry.AddressList.Contains( address );
        }
    }
}
