using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.Game;
using Doodle.Networking.Serialization;

namespace Doodle.Networking.Packets
{
	[Packet( PacketType.LobbyRequestPlayerId )]
	public struct LobbyRequestPlayerIdPacket
	{
		public PlayerId PlayerId;
	}
}
