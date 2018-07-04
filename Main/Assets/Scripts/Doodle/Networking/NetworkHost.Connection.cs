using System;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

namespace Doodle.Networking
{
    public partial class NetworkHost
    {
        class NetworkConnection : INetworkConnection, IEquatable<NetworkConnection>
        {
            public int Id { get; private set; }

            public NetworkHost Host { get; private set; }

            public bool IsDisposed { get; private set; }

            public NetworkInfo NetworkInfo { get; private set; }

            public bool IsConnected { get; internal set; }

            internal NetworkConnection( NetworkHost host, int id )
            {
                Id = id;
                Host = host;
            }

            internal void OnConnected()
            {
                int port;
                string address;
                NetworkID networkID;
                NodeID nodeID;
                byte error;

                // Validates connection and gets info about the connection
                NetworkTransport.GetConnectionInfo( Host.Id, Id, out address, out port, out networkID, out nodeID, out error );
                if( error > 0 ) throw new NetworkException( "Unable to get connection info, invalid connection id?", error );

                // 
                NetworkInfo = new NetworkInfo( address, port );
                IsConnected = true;
            }

            /// <summary>
            /// Sends a packet to the remote host on this connection.
            /// </summary>
            public bool Send( string channel, byte[] packet )
            {
                if( !IsConnected ) throw new InvalidOperationException( string.Format( "Unable to send packet, connection {0} not established with remote host.", Id ) );
                Host.ProcessEvents(); // Process any events

                byte error;
                var chan = Host.GetChannel( channel );
                NetworkTransport.Send( Host.Id, Id, chan.Id, packet, packet.Length, out error );
                if( error > 0 ) 
				{
					// Danny: Disable exception because it was causing rest of game to freeze.
					// throw new NetworkException( string.Format( "Unable to send packet to connection {0} via channel {1}", Id, chan.Id ), error );
					return false;
				}

				return true;
            }

            public override int GetHashCode()
            {
                return Id ^ Host.GetHashCode();
            }

            public override bool Equals( object obj )
            {
                if( obj is NetworkConnection )
                    return Equals( obj as NetworkConnection );

                return false;
            }

            public bool Equals( NetworkConnection other )
            {
                return other.Id == Id;
            }

            public void Dispose()
            {
                if( IsDisposed == false )
                {
                    IsDisposed = true;

                    if( IsConnected )
                    {
                        // Disconnect
                        byte error;
                        NetworkTransport.Disconnect( Host.Id, Id, out error );
						if( error > 0 ) Debug.LogError("NetworkHost.Connection.Dispose(): Failed.");//throw new NetworkException( "Disconnect", error );
                    }

                    // Clear data ( makes connection useless )
                    IsConnected = false;
                    Host = null;
                    Id = -1;
                }
            }
        }
    }
}
