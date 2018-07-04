using System;
using System.Collections.Generic;
using System.Net;

namespace Doodle.Networking
{
    public interface INetworkHost : IDisposable
    {
        /// <summary>
        /// Determines if this network host was disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Determines if this network host has any incoming packets.
        /// </summary>
        bool HasPackets { get; }

        /// <summary>
        /// Determines if this network host has any incoming broadcast packets.
        /// </summary>
        bool HasBroadcastPackets { get; }

        /// <summary>
        /// Event triggered when a connection is established.
        /// </summary>
        event ConnectionConnectedHandler Connected;

        /// <summary>
        /// Event triggered when a connection is disconnected.
        /// </summary>
        event ConnectionDisconnectedHandler Disconnected;

        /// <summary>
        /// The max number of connections this host will accept.
        /// </summary>
        int MaxConnections { get; }
         
        /// <summary>
        /// Information about the local host ( IP Address and Port Number )
        /// </summary>
        NetworkInfo NetworkInfo { get; }

        /// <summary>
        /// Number of known connections.
        /// </summary>
        int ConnectionCount { get; }

        /// <summary>
        /// Connections currently known by this host.
        /// </summary>
        IEnumerable<INetworkConnection> Connections { get; }

        /// <summary>
        /// Number of channels configured on this host.
        /// </summary>
        int ChannelCount { get; }

        /// <summary>
        /// Channels configured on this host.
        /// </summary>
        IEnumerable<IChannel> Channels { get; }

        /// <summary>
        /// Determines if the local host has been opened.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Opens the local host to accepting and creating connections.
        /// </summary>
		bool Open();

        /// <summary>
        /// Connects the local host to a remote host.
        /// </summary>
        INetworkConnection Connect( string address, int port );

        /// <summary>
        /// Disconnects the given remote connection from this host.
        /// </summary>
        void Disconnect( INetworkConnection connection );

        /// <summary>
        /// Gets the next available packet.
        /// </summary>
        /// <returns>The packet or null if there are no more packets in the queue.</returns>
        Packet Receive();

        /// <summary>
        /// Gets the next available packet.
        /// </summary>
        /// <param name="packet">Packet returned, or null if the queue is empty.</param>
        /// <returns>True if a packet was returned.</returns>
        bool Receive( out Packet packet );

        /// <summary>
        /// Gets the next available broadcast packet.
        /// </summary>
        /// <returns>The packet or null if there are no more broadcast packets in the queue.</returns>
        BroadcastPacket ReceiveBroadcast();

        /// <summary>
        /// Gets the next available broadcast packet.
        /// </summary>
        /// <param name="packet">broadcast packet returned, or null if the queue is empty.</param>
        /// <returns>True if a broadcast packet was returned.</returns>
        bool ReceiveBroadcast( out BroadcastPacket packet );

        /// <summary>
        /// Gets the channel with the given name.
        /// </summary>
        IChannel GetChannel( string name );

        /// <summary>
        /// Gets the channel with the given id.
        /// </summary>
        IChannel GetChannel( int id );

        /// <summary>
        /// Gets the name of the channel with the given id.
        /// </summary>
        string GetChannelName( int id );

        /// <summary>
        /// Get the network connection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        INetworkConnection GetConnection( int id );

        /// <summary>
        /// Process events accumlated since last process.
        /// This is called automatically by <see cref="Receive"/> or <see cref="INetworkConnection.Send(string, byte[])"/>.
        /// </summary>
        void ProcessEvents();
    }
}
