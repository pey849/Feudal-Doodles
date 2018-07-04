namespace Doodle.Networking.Serialization
{
    /// <summary>
    /// Used to mark various types as a certain kind of packet.
    /// </summary>
    public enum PacketType : byte
    {
        /// <summary>
        /// An input packet.
        /// </summary>
        Input,

        /// <summary>
        /// A transform packet.
        /// </summary>
        Transform,

        /// <summary>
        /// 
        /// </summary>
        Broadcast,

        /// <summary>
        /// Send to a player from the host to signal acceptance.
        /// </summary>
        PlayerAccept,

        /// <summary>
        /// Sent to player to inform about the loss of a player.
        /// </summary>
        PlayerLoss,


		/* Lobby-specific packets */

		/// <summary>
		/// True state of the lobby from the server. 
		/// </summary>
		LobbyState,

		/// <summary>
		/// Request from the client that is in the lobby (e.g. changed role).
		/// </summary>
		LobbyUpdateFromClient,

		/// <summary>
		/// Request from the client to retrieve its unique player id.
		/// </summary>
		LobbyRequestPlayerId,

		/// <summary>
		/// Response from server to give a client its unique player id.
		/// </summary>
		LobbyGivenPlayerId,

		/// <summary>
		/// Event that a player (client or server) disconnects.
		/// </summary>
		LobbyPlayerDisconnect,

		/// <summary>
		/// Event that the server starts the game.
		/// </summary>
		LobbyStartGame,

		/// <summary>
		/// Signal from server for client to return to the lobby menu.
		/// </summary>
		LobbyReturn,



		/* Renderer and game engine packets*/

        /// <summary>
		/// packet for current state of gameobjects like blocks/traps
		/// </summary>
		GameState,

        /// <summary>
		/// packet for when a single block is updated (added/removed)
		/// </summary>
		BlockUpdatePacket,

        /// <summary>
		/// packet for current state the runners.
		/// </summary>
		RunnerState,

        /// <summary>
		/// packet for current state the runners.
		/// </summary>
		ProjectilePacket,

        /// <summary>
		/// packet for player push
		/// </summary>
		pushPacket,

        /// <summary>
		/// packet for sending clock time and to notify economy to raise gold.
		/// </summary>
        GameClockPacket,



		/* Scoreboard packets */

		/// <summary>
		/// Team and player scores.
		/// </summary>
		ScoreboardState,

		/// <summary>
		/// Notification from client that he died.
		/// </summary>
		PlayerDied,



		/* Event Manager Networked Packets */

		ClientToHostBroadcastEvent,

		HostToClientsBroadcastEvent,


		/// <summary>
		/// Signal that sender of this packet is still alive.
		/// </summary>
		Alive,

    }
}
