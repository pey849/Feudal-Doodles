namespace Doodle.Networking
{
    /// <summary>
    /// A packet received by a network host.
    /// </summary>
    public class TypedPacket<T>
    {
        /// <summary>
        /// The deserialized data payload.
        /// </summary>
		public T Data;

        /// <summary>
        /// The channel this packet was received on.
        /// </summary>
		public IChannel Channel;

        /// <summary>
        /// The connection this packet was received from.
        /// </summary>
		public INetworkConnection Connection;
    }
}
