namespace Doodle.Networking
{
    public partial class NetworkHost
    {
        /// <summary>
        /// Contains information about a data channel configured on a host.
        /// </summary>
        class Channel : IChannel
        {
            /// <summary>
            /// The host-unique id for the channel.
            /// </summary>
            public int Id { get; private set; }

            /// <summary>
            /// The name of this channel.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// The quality-of-service type of this channel.
            /// </summary>
            public ChannelType ChannelType { get; private set; }

            /// <summary>
            /// The host this channel is associated with.
            /// </summary>
            public NetworkHost Host { get; private set; }

            /// <summary>
            /// Don't construct directly. Managed by <see cref="NetworkHost"/>
            /// </summary> 
            internal Channel( NetworkHost host, string name, byte id, ChannelType type )
            {
                Host = host;
                Name = name;
                ChannelType = type;
                Id = id;
            }
        }
    }
}
