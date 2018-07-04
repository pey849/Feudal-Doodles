using Doodle.Game;
using Doodle.InputSystem;
using Doodle.Networking.Serialization;
using System;
using UnityEngine;

namespace Doodle.Networking.Packets
{
	[Packet( PacketType.LobbyUpdateFromClient )]
    public struct LobbyUpdateFromClientPacket
    {
		public TeamRole TeamRole;
		public bool IsOccupied;
		public string Username;
//		public bool IsReady;
    }
}
