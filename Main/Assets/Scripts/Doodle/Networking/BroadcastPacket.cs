namespace Doodle.Networking
{
    /// <summary>
    /// A packet received by a network host.
    /// </summary>
    public class BroadcastPacket
    {
        /// <summary>
        /// The data payload.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// The network info about the remote host.
        /// </summary>
        public NetworkInfo RemoteInfo { get; private set; }

        /// <summary>
        /// Do not construct manually.
        /// Managed by <see cref="NetworkHost"/>.
        /// </summary>
        internal BroadcastPacket( string address, int port, byte[] data )
        {
            RemoteInfo = new NetworkInfo( address, port );
            Data = data;
        }
    }
}
