using Doodle.Game;
using Doodle.InputSystem;
using Doodle.Networking.Serialization;
using System;
using UnityEngine;

namespace Doodle.Networking.Packets
{
	[Packet( PacketType.LobbyState )]
    public struct LobbyStatePacket
    {
		// If true, host should broadcast new lobby state and refresh it own GUI.
		public bool IsDirty;

		// Based on the number of connected clients, not number of occupied lobby slots.
		public int NumActivePlayers;

		// Has in-game started?
		public bool IsStartGame;

		// Each array element corresponds to a team+role. 
		// For example, isOccupied[TeamRole.PurpleBuilder] == true.

		public bool[] IsOccupieds;
		public PlayerId[] PlayerIds;
		public string[] Usernames;
		public bool[] IsHosts;
//		public bool[] IsReadys;

		public LobbyStatePacket(bool unused)
		{
			IsDirty = false;
			NumActivePlayers = 0;
			IsStartGame = false;
			IsOccupieds = new bool[4];
			PlayerIds = new PlayerId[4];
			Usernames = new string[4];
			IsHosts = new bool[4];
//			IsReadys = new bool[4];

			for (int i = 0; i < 4; i++)
				IsOccupieds[i] = false;
		}

		public void MergeUpdateFromClient(LobbyUpdateFromClientPacket client, PlayerId clientId)
		{
			int s = (int) client.TeamRole; 

			// If TeamRole is already occupied by a different client
			if (this.IsOccupieds[s] && this.PlayerIds[s] != clientId)
			{
				// Don't merge client's update.
			}
			else 
			{
				// First, remove client's current occupied slot if exists.
				for (int i = 0; i < 4; i++)
				{
					if (this.IsOccupieds[i] == true && this.PlayerIds[i] == clientId)
						this.IsOccupieds[i] = false;
				}

				// Second, put client into its requested team role slot if requested for it.
				this.IsOccupieds[s] = client.IsOccupied;
				this.PlayerIds[s] = clientId;
				this.Usernames[s] = client.Username;
				this.IsHosts[s] = false;
//				this.IsReadys[s] = client.IsReady;
			}
		}

		public void RemoveClient(PlayerId id)
		{
			for (int i = 0; i < 4; i++)
				if (this.IsOccupieds[i] == true && this.PlayerIds[i] == id)
					this.IsOccupieds[i] = false;
		}
    }
}
