using Doodle.Game;
using Doodle.Networking.Serialization;

namespace Doodle.Networking.Packets
{
	[Packet( PacketType.LobbyPlayerDisconnect )]
	public struct LobbyPlayerDisconnectPacket
	{
		public bool IsServer;

		public LobbyPlayerDisconnectPacket(bool isServer)
		{
			this.IsServer = isServer;
		}
	}
}
