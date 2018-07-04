using Doodle.Game;
using Doodle.InputSystem;
using Doodle.Networking.Serialization;
using System;
using UnityEngine;

namespace Doodle.Networking.Packets
{
	[Packet( PacketType.ScoreboardState )]
	public struct ScoreboardStatePacket
    {
		public bool IsGameOver;

		public Runner PurpleRunner;
		public Builder PurpleBuilder;

		public Runner YellowRunner;
		public Builder YellowBuilder;

		public ScoreboardStatePacket(bool unused)
		{
			IsGameOver = false;

			PurpleRunner = new Runner(true);
			PurpleBuilder = new Builder(true);

			YellowRunner = new Runner(true);
			YellowBuilder = new Builder(true);
		}




		[System.Serializable]
		public struct Runner
		{
			public string Username;
			public int FlagsCaptured;
			public int Deaths;

			public Runner(bool unused)
			{
				Username = "<Unassigned Username>";
				FlagsCaptured = 0;
				Deaths = 0;
			}
		}

		[System.Serializable]
		public struct Builder
		{
			public string Username;
			public int BlocksPlaced;

			public Builder(bool unused)
			{
				Username = "<Unassigned Username>";
				BlocksPlaced = 0;
			}
		}
    }
}
