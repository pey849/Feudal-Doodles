using QOS = UnityEngine.Networking.QosType;

namespace Doodle.Networking
{
    /// <summary>
    /// The quality-of-service type of network host channels.
    /// </summary>
    public enum ChannelType
    {
        /// <summary> 
        /// Each message is guaranteed to be delivered but not guaranteed to be in order.
        /// </summary>
        ReliableUnordered = QOS.Reliable,

        /// <summary> 
        /// There is no guarantee of delivery or ordering.
        /// </summary>
        UnreliableUnordered = QOS.Unreliable,

        /// <summary> 
        /// Each message is guaranteed to be delivered and in order.
        /// </summary>
        ReliableSequenced = QOS.ReliableSequenced,

        /// <summary>
        /// There is no guarantee of delivery and all unordered messages will be dropped. 
        /// Example: VoIP.
        /// </summary>
        UnreliableSequenced = QOS.UnreliableSequenced,

        /// <summary> 
        /// A reliable message. Note: Only the last message in the send buffer is sent. Only
        /// the most recent message in the receive buffer will be delivered.
        /// </summary>
        ReliableStateUpdate = QOS.ReliableStateUpdate,

        /// <summary>
        /// An unreliable message. Only the last message in the send buffer is sent. Only
        /// the most recent message in the receive buffer will be delivered.
        /// </summary>
        UnreliableStateUpdate = QOS.StateUpdate,
    }
}
