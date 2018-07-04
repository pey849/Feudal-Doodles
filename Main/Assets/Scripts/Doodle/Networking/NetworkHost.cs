using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System;
using System.Text;
using System.Linq;
using System.Net;

namespace Doodle.Networking
{
    public delegate void ConnectionConnectedHandler( INetworkConnection connection );

    public delegate void ConnectionDisconnectedHandler( INetworkConnection connection );

    /// <summary>
    /// Network Host. Represents the local endpoint in a network.
    /// </summary>
    public partial class NetworkHost : INetworkHost
    {
        /// <summary>
        /// Unique id representing this host.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Number of connections available on this host.
        /// </summary>
		public int ConnectionCount 
		{ 
			get
			{
				return _Connections.Count( c => c.IsConnected );
			}
		}

        /// <summary>
        /// Number of channels available on this host.
        /// </summary>
        public int ChannelCount { get { return _Channels.Count; } }

        /// <summary>
        /// The set of connections available on this host.
        /// </summary>
        public IEnumerable<INetworkConnection> Connections
        {
            get
            {
                if( !IsOpen ) throw new InvalidOperationException( "Unable to access connection info, host not open." );

				lock(_Connections)
				{
	                foreach( var connection in _Connections.Where( c => c.IsConnected ) )
	                    yield return connection;
				}
            }
        }

        /// <summary>
        /// The set of channels available on this host.
        /// </summary>
        public IEnumerable<IChannel> Channels
        {
            get
            {
                if( !IsOpen ) throw new InvalidOperationException( "Unable to access channel info, host not open." );

                // 
                foreach( var channel in _Channels.Values )
                    yield return channel;
            }
        }

        /// <summary>
        /// Event called when a connection is established.
        /// </summary>
        public event ConnectionConnectedHandler Connected;

        /// <summary>
        /// Event called when an established connection is lost.
        /// </summary>
        public event ConnectionDisconnectedHandler Disconnected;

        /// <summary>
        /// Max number of supported connections on this host.
        /// </summary>
        public int MaxConnections { get; private set; }

        /// <summary>
        /// Information about the local host ( IP Address and Port Number )
        /// </summary>
        public NetworkInfo NetworkInfo { get; private set; }

        /// <summary>
        /// Determines if the host has been opened to connections.
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// Determines if this network host was disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Determines if this network host has any incoming packets.
        /// </summary>
        public bool HasPackets { get { return _Packets.Count > 0; } }

        /// <summary>
        /// Determines if this network host has any incoming broadcast packets.
        /// </summary>
        public bool HasBroadcastPackets { get { return _BroadcastPackets.Count > 0; } }

        //

        private readonly int MAX_PACKET_QUEUE_SIZE = byte.MaxValue;

        private HostTopology HostTopology;
        private ConnectionConfig ConnectionConfig;

        private Queue<Packet> _Packets;
        private Queue<BroadcastPacket> _BroadcastPackets;
		private List<NetworkConnection> _Connections;		// Setting to public as hotfix to lock _Connections during SendPacket() and Dispose() which can conflict without locking.
        private Dictionary<string, Channel> _Channels;

        private List<ChannelDesc> _ChannelBuildList;

        private int _BroadcastKey = -1;
        private int _BroadcastVersion;

        private static NetworkHost _BroadcastHost = null;

        private static byte[] Buffer = new byte[8192];

        private static bool NetworkAPIInitialized;
        private static int ConnetionCount = 0;

        //

        /// <summary>
        /// Create a network connection end-point with the given address.
        /// </summary>
        /// <param name="channels">Channel topology.</param>
        public NetworkHost( int maxConnections )
            : this( IPAddress.Any, 0, maxConnections )
        { }

        /// <summary>
        /// Create a network connection end-point with the given address.
        /// </summary>
        /// <param name="channels">Channel topology.</param> 
        /// <param name="port">The port to bind this connection to.</param>
        /// <param name="maxConnections">Maximum number of supported connections.</param>
        public NetworkHost( int port, int maxConnections )
            : this( IPAddress.Any, port, maxConnections )
        { }

        /// <summary>
        /// Create a network connection end-point with the given address.
        /// </summary>
        /// <param name="channels">Channel topology.</param>
        /// <param name="address">The IPv4 address to bind this connection to.</param>
        /// <param name="port">The port to bind this connection to.</param>
        /// <param name="maxConnections">Maximum number of supported connections.</param>
        public NetworkHost( string address, int port, int maxConnections )
            : this( IPAddress.Parse( address ), port, maxConnections )
        { }

        /// <summary>
        /// Create a network connection end-point with the given address.
        /// </summary>
        /// <param name="channels">Channel topology.</param>
        /// <param name="address">The IPv4 address to bind this connection to.</param>
        /// <param name="port">The port to bind this connection to.</param>
        /// <param name="maxConnections">Maximum number of supported connections.</param>
        public NetworkHost( IPAddress address, int port, int maxConnections )
        {
            // 
            if( maxConnections <= 0 ) throw new ArgumentOutOfRangeException( "maxConnections" );
            if( address == null ) throw new ArgumentNullException( "address" );
            if( port < 0 ) throw new ArgumentOutOfRangeException( "port" );

            // 
            InitiateNetworkAPI();

            // 
            ConnectionConfig = new ConnectionConfig();
			ConnectionConfig.DisconnectTimeout = 5000;

            // 
            MaxConnections = maxConnections;
            NetworkInfo = new NetworkInfo( address, port );

            // 
            _ChannelBuildList = new List<ChannelDesc>();
            _Channels = new Dictionary<string, Channel>();
            _Connections = new List<NetworkConnection>();
            _BroadcastPackets = new Queue<BroadcastPacket>();
            _Packets = new Queue<Packet>();
        }

        /// <summary>
        /// Add a new channel.
        /// </summary>
        public void ConfigureChannel( string name, ChannelType type )
        {
            if( IsOpen ) throw new InvalidOperationException( "Network host already open, unable to modify channel configuration." );
            _ChannelBuildList.Add( new ChannelDesc { Name = name, Type = type } );
        }

        /// <summary>
        /// Opens the host to accepting or creating connections.
        /// </summary>
		/// 
		/// <returns>
		/// True if successful.
		/// False if failure. this.ConnectionConfig and this._Channels will be discarded.
		/// </returns>
        public bool Open()
        {
            if( IsOpen ) throw new InvalidOperationException( "Network host already open" );

            // Sort by name ( to ensure same id )
            _ChannelBuildList.Sort( ( a, b ) => a.Name.CompareTo( b.Name ) );

            // Construct channels
            foreach( var build in _ChannelBuildList )
            {
                var id = ConnectionConfig.AddChannel( (QosType) build.Type );
                _Channels[build.Name] = new Channel( this, build.Name, id, build.Type );
            }

            // Define Host
            HostTopology = new HostTopology( ConnectionConfig, MaxConnections );
            Id = NetworkTransport.AddHost( HostTopology, NetworkInfo.Port, NetworkInfo.Address.ToString() );
			if (Id == -1)
			{
				// Undo changes made by Open().
				ConnectionConfig = new ConnectionConfig();
				_Channels = new Dictionary<string, Channel>();
				return false;
			}

            _ChannelBuildList = null;

            // Is now open
            IsOpen = true;

			return true;
        }

        /// <summary>
        /// Connect this connection to remote host.
        /// </summary> 
        public INetworkConnection Connect( string address, int port )
        {
            return Connect( IPAddress.Parse( address ), port );
        }

        /// <summary>
        /// Connect this connection to remote host.
        /// </summary> 
        public INetworkConnection Connect( IPAddress address, int port )
        {
            // Host must already open
            if( !IsOpen ) throw new InvalidOperationException( "Unable to connect to remote host, local host not open." );

            // Validate address and port
            if( address == null ) throw new ArgumentNullException( "address" );
            if( port < 0 ) throw new ArgumentOutOfRangeException( "port" );

            // Connect To Remote Host
            byte error;
            var id = NetworkTransport.Connect( Id, address.ToString(), port, 0, out error );
            if( error > 0 ) throw new NetworkException( "Connect", error );

            // Create new connection and store in connections list
            var connection = new NetworkConnection( this, id );
            _Connections.Add( connection );

            return connection;
        }

        /// <summary>
        /// Disconnect this connection from a remote host.
        /// </summary>
        public void Disconnect( INetworkConnection connection )
        {
            // 
            if( !IsOpen ) throw new InvalidOperationException( "Unable to disconnect from remote host, local host not open." );
            connection.Dispose();
        }

        /// <summary>
        /// Gets a known network connection by the given id.
        /// </summary>
        /// <exception cref="InvalidOperationException">Local host not configured.</exception>
        /// <exception cref="InvalidOperationException">Connection id not valid.</exception>
        public INetworkConnection GetConnection( int id )
        {
            // 
			if( !IsOpen ) return null; //throw new InvalidOperationException( "Unable to get channel information, local host not open." );

            // 
            var conn = _Connections.Find( c => c.Id == id );
			if( conn == null ) return null; //throw new InvalidOperationException( string.Format( "Unable to find connection {0}.", id ) );
            return conn;
        }

        /// <summary>
        /// Receive any packets sent to the local host.
        /// </summary>
        /// <returns></returns>
        public Packet Receive()
        {
            ProcessEvents(); // Process any events

            if( HasPackets ) return _Packets.Dequeue();
            else return null;
        }

        /// <summary>
        /// Receive any packets sent to the local host.
        /// </summary>
        /// <returns></returns>
        public bool Receive( out Packet packet )
        {
            packet = Receive();
            return packet != null;
        }

        /// <summary>
        /// Receive any broadcast packets received by the local host.
        /// </summary>
        /// <returns></returns>
        public BroadcastPacket ReceiveBroadcast()
        {
            if( HasBroadcastPackets ) return _BroadcastPackets.Dequeue();
            else return null;
        }

        /// <summary>
        /// Receive any broadcast packets received by the local host.
        /// </summary>
        /// <returns></returns>
        public bool ReceiveBroadcast( out BroadcastPacket packet )
        {
            packet = ReceiveBroadcast();
            return packet != null;
        }

        /// <summary>
        /// Gets channel information with the given name.
        /// </summary> 
        public IChannel GetChannel( string name )
        {
            // 
            if( !IsOpen ) throw new InvalidOperationException( "Unable to get channel information, local host not open." );

            // 
            if( !_Channels.ContainsKey( name ) )
                throw new InvalidOperationException( string.Format( "Unable to get channel by name. ( '{0}' )", name ) );
            return _Channels[name];
        }

        /// <summary>
        /// Gets a channel with the specific id.
        /// </summary>
        public IChannel GetChannel( int id )
        {
            // 
            if( !IsOpen ) throw new InvalidOperationException( "Unable to get channel information, local host not open." );

            // 
            var channel = _Channels.Values.Where( c => c.Id == id ).FirstOrDefault();
            if( channel == null ) throw new InvalidOperationException( string.Format( "Unable to get channel by id. ( {0} ).", id ) );
            else return channel;
        }

        /// <summary>
        /// Gets the name of the channel with the given id.
        /// </summary>
        public string GetChannelName( int id )
        {
            var c = GetChannel( id );
            return _Channels.Where( kv => kv.Value.Id == c.Id ).First().Key;
        }

        protected virtual void OnConnect( INetworkConnection connection )
        {
            if( Connected != null )
                Connected.Invoke( connection );
        }

        protected virtual void OnDisconnect( INetworkConnection connection )
        {
            if( Disconnected != null )
                Disconnected.Invoke( connection );
        }

        /// <summary>
        /// Process network events on this host. 
        /// This will send and receive packets as well as handle connections and disconnects.
        /// </summary>
        public void ProcessEvents()
        {
            // 
            if( !IsOpen ) throw new InvalidOperationException( "Unable to process network events, local host not open." );

            while( true )
            {
                byte error;
                int connectionId, channelId, dataSize;
                var type = NetworkTransport.ReceiveFromHost( Id, out connectionId, out channelId, Buffer, Buffer.Length, out dataSize, out error );

				// Danny: In rare instances, this will be encountered instead of NetworkEventType.Disconnect. 
				if ((NetworkError)error == NetworkError.Timeout)
				{
					Debug.LogError("NetworkHost.ProcessEvents() : Detected Timeout.");
					type = NetworkEventType.DisconnectEvent;
				}
                else if( error > 0 ) 
				{
					throw new NetworkException( string.Format( "Receive - Host {0}", Id ), error );
				}

                if( type == NetworkEventType.Nothing ) return; // Exit function

//				if( error > 0 )
//                    Debug.LogFormat( "{4} Event: Host {0} Connection {1} Channel {2} Size {3}", Id, connectionId, channelId, dataSize, type );

                var channel = _Channels.Values.FirstOrDefault( c => c.Id == channelId );
                var connection = _Connections.Find( c => c.Id == connectionId );

                switch( type )
                {
                    case NetworkEventType.BroadcastEvent:

                        // Get remote info
                        string remoteAddress;
                        int messageSize, remotePort;
                        NetworkTransport.GetBroadcastConnectionInfo( Id, out remoteAddress, out remotePort, out error );
                        if( error > 0 ) throw new NetworkException( string.Format( "Broadcast - Get Info {0}", Id ), error );

                        // Avoid messages from the local host
                        if( !IsLocalAddress( remoteAddress ) )
                        {
                            // Get remote message
                            NetworkTransport.GetBroadcastConnectionMessage( Id, Buffer, Buffer.Length, out messageSize, out error );
                            if( error > 0 ) throw new NetworkException( string.Format( "Broadcast - Get Message {0}", Id ), error );

                            // Extract message
                            var message = new byte[messageSize];
                            Array.Copy( Buffer, message, messageSize );

                            // Create packet object
                            var bPacket = new BroadcastPacket( remoteAddress, remotePort, message );
                            _BroadcastPackets.Enqueue( bPacket );

                            // Too many packets, start clearing them...!
                            if( _BroadcastPackets.Count > MAX_PACKET_QUEUE_SIZE )
                            {
                                Debug.LogWarningFormat( "Too many broadcast packets in queue. Discarding oldest packet." );
                                _BroadcastPackets.Dequeue();
                            }
                        }

                        break;

                    case NetworkEventType.ConnectEvent:

                        // Create new remote 
                        // Will be null if a remote is connecting to the local host
                        // Won't be null if the local host is connecting to a remote host
                        if( connection == null )
                        {
                            connection = new NetworkConnection( this, connectionId );
                            _Connections.Add( connection );
                        }

                        // 
                        if( !connection.IsConnected )
                            connection.OnConnected();

                        // Trigger Connection Event
                        OnConnect( connection );
                        break;

                    case NetworkEventType.DisconnectEvent:
						
						Debug.Log("Got disconnect event.");

                        // Remove remote connection
                        if( connection != null )
                        {
                            _Connections.Remove( connection );
                        }
                        else
                        {
                            Debug.LogWarningFormat( "Disconnect: {0} wasn't in connections?", connectionId );
                        }

                        // Trigger Disconnection Event
                        OnDisconnect( connection );
                        break;

                    case NetworkEventType.DataEvent:

                        // Copy packet from buffer
                        var data = new byte[dataSize];
                        Array.Copy( Buffer, data, dataSize );

                        // Debug.LogFormat( "-- Received a packet ( {0} bytes )", data.Length );

                        // Create packet object
                        var dPacket = new Packet( connection, channel, data );
                        _Packets.Enqueue( dPacket );

                        // Too many packets, start clearing them...!
                        if( _Packets.Count > MAX_PACKET_QUEUE_SIZE )
                        {
                            Debug.LogWarningFormat( "Too many packets in queue. Discarding oldest packet." );
                            _Packets.Dequeue();
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Set the broadcast credentials for this host.
        /// </summary>
        /// <param name="key">Some unique key that represents the game.</param>
        /// <param name="version">Some versioning number to differentiate between builds.</param>
        public void SetBroadcastCredentials( int key, int version )
        {
            if( !IsOpen ) throw new InvalidOperationException( "Unable to set broadcast credentials, local host not open." );
            if( version < 0 ) throw new ArgumentException( "version" );
            if( key < 0 ) throw new ArgumentException( "key" );

            // 
            _BroadcastKey = key;
            _BroadcastVersion = version;

            // 
            byte error;
            NetworkTransport.SetBroadcastCredentials( Id, key, version, 0, out error );
            if( error > 0 ) throw new NetworkException( "Error setting broadcast credentials", error );
        }

        /// <summary>
        /// Begins broadcasting the given message on the local subnet using the given host.
        /// </summary>
        public static void StartBroadcast( NetworkHost host, byte[] message )
        {
            if( !host.IsOpen ) throw new InvalidOperationException( "Unable to start broadcast, local host not open." );
            if( host._BroadcastKey == -1 ) throw new InvalidOperationException( "Unable to start broadcast, host credentials not set" );

            // 
            byte error;
            NetworkTransport.StartBroadcastDiscovery( host.Id, host.NetworkInfo.Port, host._BroadcastKey, host._BroadcastVersion, 0, message, message.Length, 500, out error );
            if( error > 0 ) throw new NetworkException( "Error setting broadcast credentials", error );
        }

        /// <summary>
        /// Stops broadcasting on the local subnet.
        /// </summary>
        public static void StopBroadcast()
        {
            if( _BroadcastHost == null ) throw new InvalidOperationException( "Unable to stop broadcast, no broadcast enabled." );
            NetworkTransport.StopBroadcastDiscovery();
        }

        /// <summary>
        /// Determines if a message is being broadcasted on the local subnet.
        /// </summary>
        public static bool IsBroadcasting()
        {
            return _BroadcastHost != null;
        }

        /// <summary>
        /// Gets the host ( if any ) that was used to configure the broadcast credentials.
        /// </summary>
        public static NetworkHost GetBroadcastHost()
        {
            return _BroadcastHost;
        }

        /// <summary>
        /// Dispose this host, this will disconnect any active connections.
        /// </summary>
        public void Dispose()
        {
            if( IsDisposed == false )
            {
				lock (_Connections)
				{
	                // Disconnect
	                foreach( var conn in _Connections.ToArray() )
	                    Disconnect( conn );

	                DisposeNetworkAPI();
	                IsDisposed = true;
				}
            }
        }

        /// <summary>
        /// Primes the network API and increments the ref-count of the connections wanting it.
        /// </summary>
        private static void InitiateNetworkAPI()
        {
            ConnetionCount++;

            if( !NetworkAPIInitialized )
            {
                NetworkAPIInitialized = true;
                NetworkTransport.Init();
            }
        }

        /// <summary>
        /// Discharges the network API and decrements the ref-count, eventually stopping the network API.
        /// </summary>
        private static void DisposeNetworkAPI()
        {
            if( ConnetionCount > 0 ) ConnetionCount--;
            
			if( ConnetionCount == 0 && NetworkAPIInitialized )
            {
                NetworkAPIInitialized = false;
                NetworkTransport.Shutdown();
            }
        }

        /// <summary>
        /// Used to build the channel configuration.
        /// </summary>
        struct ChannelDesc
        {
            public string Name;
            public ChannelType Type;
        }



		// Danny: Moved from NetworkManager to enable locking.

		public bool SendPacket<T>( byte[] data, string channel, T packet )
		{
			// Debug.LogFormat( "Sending {0} ( {1} bytes )", typeof( T ).Name, data.Length );

			bool isAllSuccess = true;

			lock (_Connections)
			{
				foreach( var conn in _Connections )
				{
					if ( ! conn.IsConnected )
						continue;

					bool isSuccess = conn.Send( channel, data );
					if ( ! isSuccess )
					{
						Debug.LogError("NetworkHost.SendPacket() failed to connection id: " + conn.Id);
						isAllSuccess = false;
					}
				}
			}

			return isAllSuccess;
		}
    }
}