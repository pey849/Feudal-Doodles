using Doodle.Game;
using Doodle.Networking.Serialization;

namespace Doodle.Networking.Packets
{
    [Packet( PacketType.PlayerLoss )]
    public struct PlayerLossPacket
    {
        public PlayerId PlayerId;

        public PlayerLossPacket( PlayerId id )
        {
            PlayerId = id;
        }
    }
}
