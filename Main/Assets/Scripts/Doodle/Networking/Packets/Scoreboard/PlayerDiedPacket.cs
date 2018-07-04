using Doodle.Game;
using Doodle.InputSystem;
using Doodle.Networking.Serialization;
using System;
using UnityEngine;

namespace Doodle.Networking.Packets
{
	[Packet( PacketType.PlayerDied )]
	public struct PlayerDiedPacket
    {
		public Team team;

		public PlayerDiedPacket(Team team)
		{
			this.team = team;
		}
	}
}
