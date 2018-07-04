using Doodle.Game;
using Doodle.InputSystem;
using Doodle.Networking.Serialization;
using System;
using UnityEngine;

namespace Doodle.Networking.Packets
{
	[Packet( PacketType.HostToClientsBroadcastEvent )]
	public struct HostToClientsBroadcastEventPacket
    {
		public PlayerId senderPlayerId;
		public EventType eventType;
		public string arg;

		public HostToClientsBroadcastEventPacket(PlayerId senderPlayerId, EventType eventType, string arg)
		{
			this.senderPlayerId = senderPlayerId;
			this.eventType = eventType;
			this.arg = arg;
		}
	}
}
