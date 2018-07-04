using Doodle.Networking;
using Doodle.Networking.Serialization;
using UnityEngine;

namespace Doodle.Game
{
    /// <summary>
    /// A component that acts like a server or client networking endpoint.
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        /// <summary>
        /// The local host for sending and receiving packets with a remote host.
        /// </summary>
        public NetworkHost NetworkHost { get; private set; }

        /// <summary>
        /// The serializer used to destruct objects and reconstruct objects as byte streams.
        /// </summary>
        public PacketSerializer Serializer { get; private set; }

        /// <summary>
        /// The sorted queue of packets.
        /// </summary>
        public PacketQueue PacketQueue { get; private set; }

        /// <summary>
        /// The address of the remote server ( in client mode )
        /// or the address of the local server ( in server mode ).
        /// </summary>
        public string ServerAddress = "127.0.0.1";

        /// <summary>
        /// The port of the remote server ( in client mode )
        /// or the port of the local server ( in server mode ).
        /// </summary>
        public int ServerPort = 8888;

        /// <summary>
        /// Determines if this network host acts like a server or client.
        /// </summary>
        protected NetworkMode Mode;

		/// <summary>
		/// Has the network manager started? Call Run() to start it.
		/// </summary>
		public bool IsRunning = false;


		/// <summary>
		/// Start up the network manager. This should be called by Lobby.cs
		/// when the server host IP has been discovered and is connectable.
		/// </summary>
		/// 
		/// <param name="serverAddr">
		/// As server, local IP address to host on.
		/// As client, remote server IP address to connect to.
		/// </param>
		/// 
		/// <param name="nwMode">
		/// Are you the server or client?
		/// </param>
		/// 
		/// <param name="useDNS">
		/// If serverAddr is a local IP, convert it via DNS? Set to false if using wifi-direct IP address.
		/// </param>
		/// 
		/// <returns>
		/// True if successful.
		/// If client, use this.NetworkHost.ConnectionCount == 1 afterwards to confirm connection to server.
		/// </returns>
		public bool Run(string serverAddr, NetworkMode nwMode, bool useDNS=true)
		{
			ServerAddress = serverAddr;
			Mode = nwMode;

			// Construct serializer and packet queue
			Serializer = new XMLPacketSerializer();
			PacketQueue = new PacketQueue( Serializer );

			// Create and configure host 
			if( Mode == NetworkMode.Server )
			{
				// Convert any "local address" to the "dns local address"
				if( useDNS && NetworkHost.IsLocalAddress( ServerAddress ) )
					ServerAddress = NetworkHost.GetLocalAddress();

				// Server, configures socket to be "known" at this address
				Debug.Log("NetworkManager: As a server, I am using server IP: " + ServerAddress);
				NetworkHost = new NetworkHost( ServerAddress, ServerPort, 4 );
			}
			else
			{
				// Client, connect to server
				NetworkHost = new NetworkHost( 4 );
			}

			// Configure channels and open/allow connections.
			NetworkHost.ConfigureChannel( "info", ChannelType.ReliableSequenced );
			NetworkHost.ConfigureChannel( "state", ChannelType.UnreliableStateUpdate );
			NetworkHost.ConfigureChannel( "input", ChannelType.ReliableStateUpdate );
			bool rv = NetworkHost.Open();
			if (!rv)
			{
				NetworkHost.Dispose();
				NetworkHost = null;
				return false;
			}

			// Connect client to server
			if( Mode == NetworkMode.Client )
			{
				Debug.Log("NetworkManager: As a client, I am using server IP: " + ServerAddress);
				NetworkHost.Connect( ServerAddress, ServerPort );
			}
				
			// Allow FixedUpdate() to run.
			IsRunning = true;

			return true;
		}
			
		/// <summary>
		/// Stop the network manager. Run() will need to be called to start again.
		/// </summary>
		public void Stop()
		{
			if (IsRunning)
			{
				IsRunning = false;
				NetworkHost.Dispose();
			}
		}

        private void FixedUpdate()
        {
			// Run() needs to be called first for this body to execute.
			if (IsRunning)
			{
	            // Process network events ( send, receive, broadcast, etc )
	            NetworkHost.ProcessEvents();

	            // Process broadcast packets.
	            BroadcastPacket bPacket;
	            while( NetworkHost.ReceiveBroadcast( out bPacket ) )
	            {
	                Debug.LogFormat( "Received Broadcast: {0} bytes from {1}", bPacket.Data.Length, bPacket.RemoteInfo );
	            }
                
	            // Process incoming packets from remote connections.
	            if( NetworkHost.ConnectionCount > 0 )
	                PacketQueue.GatherPackets( NetworkHost );
			}
        }

        #region Send Functions

        /// <summary>
        /// Send out packet to all available connections.
        /// </summary>
        public bool SendPacket<T>( string channel, T packet )
        {
			byte[] data = Serializer.Serialize( packet );
			return NetworkHost.SendPacket<T>(data, channel, packet);
        }

        /// <summary>
        /// Send out packet to the connection with the specified Id.
        /// </summary>
        public bool SendPacket<T>( string channel, int connectionId, T packet )
        {
            var conn = NetworkHost.GetConnection( connectionId );
            var data = Serializer.Serialize( packet );
            bool isSuccess = conn.Send( channel, data );
			return isSuccess;
        }

        #endregion

        private void OnDestroy()
        {
			if (IsRunning)
           		NetworkHost.Dispose();
        }


    }
}
