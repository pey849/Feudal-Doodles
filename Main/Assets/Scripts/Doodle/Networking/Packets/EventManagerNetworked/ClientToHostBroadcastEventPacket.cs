using Doodle.Game;
using Doodle.InputSystem;
using Doodle.Networking.Serialization;
using System;
using UnityEngine;

namespace Doodle.Networking.Packets
{
	[Packet( PacketType.ClientToHostBroadcastEvent )]
	public struct ClientToHostBroadcastEventPacket
    {
		public PlayerId senderPlayerId;
		public EventType eventType;
		public string arg;

		public ClientToHostBroadcastEventPacket(PlayerId senderPlayerId, EventType eventType, string arg)
		{
			this.senderPlayerId = senderPlayerId;
			this.eventType = eventType;
			this.arg = arg;
		}
	}
}
