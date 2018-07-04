namespace Doodle.Networking
{
    /// <summary>
    /// Contains information about a data channel configured on a host.
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// The host-unique id for the channel.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of this channel.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The quality-of-service type of this channel.
        /// </summary>
        ChannelType ChannelType { get; }

        /// <summary>
        /// The host this channel is associated with.
        /// </summary>
        NetworkHost Host { get; }
    }
}
