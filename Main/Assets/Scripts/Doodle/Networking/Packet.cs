namespace Doodle.Networking
{
    /// <summary>
    /// A packet received by a network host.
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// The data payload.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// The channel this packet was received on.
        /// </summary>
        public IChannel Channel { get; private set; }

        /// <summary>
        /// The connection this packet was received from.
        /// </summary>
        public INetworkConnection Connection { get; private set; }

        /// <summary>
        /// Do not construct manually.
        /// Managed by <see cref="NetworkHost"/>.
        /// </summary>
        internal Packet( INetworkConnection connection, IChannel channel, byte[] data )
        {
            Connection = connection;
            Channel = channel;
            Data = data;
        }
    }
}
