using System;
using System.Net;

namespace Doodle.Networking
{
    /// <summary>
    /// Represents a connecton to a remote host.
    /// </summary>
    public interface INetworkConnection : IDisposable
    {
        /// <summary>
        /// The host-unique id for this connection.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The local host associated with this connection.
        /// </summary>
        NetworkHost Host { get; }

        /// <summary>
        /// Information about the remote connection ( IP Address and Port Number )
        /// </summary>
        NetworkInfo NetworkInfo { get; }

        /// <summary>
        /// Determines if this connection has actually be established.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Determines if this connection has been disposed of.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Send a packet along this connection via the given channel.
        /// </summary>
        bool Send( string channel, byte[] packet );
    }
}
