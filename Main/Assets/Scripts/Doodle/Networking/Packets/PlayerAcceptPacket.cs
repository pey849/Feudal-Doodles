using Doodle.Game;
using Doodle.InputSystem;
using Doodle.Networking.Serialization;

namespace Doodle.Networking.Packets
{
    [Packet( PacketType.PlayerAccept )]
    public struct PlayerAcceptPacket
    {
        public PlayerId PlayerId;

        public PlayerAcceptPacket( PlayerId id )
        {
            PlayerId = id;
        }
    }
}
