using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.Game;
using Doodle.Networking.Packets;
using Doodle.InputSystem.Handlers;

public class Scoreboard : MonoBehaviour, IGameListener
{
	/// <summary>
	/// Holds infomation about the current score and player statistics (e.g. number of kills).
	/// 
	/// As server, you should update this directly and broadcast it to clients.
	/// 
	/// As client, you should only be overwriting this whenever receiving a new ScoreboardStatePacket from the 
	/// server.
	/// 
	/// Contains IsGameOver boolean to track whether or not the game has ended.
	/// </summary>
	public ScoreboardStatePacket mPacket;

	/// <summary>
	/// From server perspective, did I modify the scoreboard? If so, I should broadcast it.
	/// </summary>
	public bool mPacketDirtyBit;

	/// <summary>
	/// Shared network manager from the game engine.
	/// </summary>
	protected NetworkManager mNwMgr;

	/// <summary>
	/// Am I the host?
	/// </summary>
	protected bool mIsHost;

	/// <summary>
	/// Is the scoreboard running?
	/// </summary>
	protected bool mIsRunning;

	/// <summary>
	/// Which team am I in?
	/// </summary>
	protected Team mMyTeam;

	/// <summary>
	/// Which role am I in?
	/// </summary>
	protected Role mMyRole;


	void Start()
	{
		mPacketDirtyBit = false;
		mNwMgr = null;
		mIsHost = false;
		mIsRunning = false;

		// This obj will be able to listen for FlagCaptured events and more.
		EventManager.Instance.AddGameListener(EventType.FlagCaptured, this);
		EventManager.Instance.AddGameListener(EventType.TimeUp, this);
		EventManager.Instance.AddGameListener(EventType.PlayerDied, this);
		EventManager.Instance.AddGameListener(EventType.BlockPlaced, this);
	}

	/// <summary>
	/// Start the scoreboard to listen for game events (e.g. flag captured). Scores will be reset. If the host, 
	/// this obj will handle synchronizing the server's truth score with the clients.
	/// </summary>
	/// 
	/// <param name="playersInfo">Information about the current players</param>
	/// <param name="nwMgr">Network manager from the game engine</param>
	/// <param name="isHost">Am I the server?</param>
	public void Run(LobbyStatePacket playersInfo, NetworkManager nwMgr, bool isHost, TeamRole myTeamRole)
	{
		mNwMgr = nwMgr;
		mIsHost = isHost;
		mMyTeam = Common.GetTeam(myTeamRole);
		mMyRole = Common.GetRole(myTeamRole);

		mPacket = new ScoreboardStatePacket(true);
		mPacket.PurpleRunner.Username = playersInfo.Usernames[(int)TeamRole.PurpleRunner];
		mPacket.PurpleBuilder.Username = playersInfo.Usernames[(int)TeamRole.PurpleBuilder];
		mPacket.YellowRunner.Username = playersInfo.Usernames[(int)TeamRole.YellowRunner];
		mPacket.YellowBuilder.Username = playersInfo.Usernames[(int)TeamRole.YellowBuilder];

		mIsRunning = true;
	}
		

	void Update()
	{
		if ( ! mIsRunning )
			return;
		
		SendScoreboardState();
		ReceiveScoreboardState();
	}

	/// <summary>
	/// This function is called when we receive an event (e.g. flag captured). Handle the event
	/// appropiately such as incrementing the player's score.
	/// </summary>
	public void OnEvent(EventType eventType, Component sender, object arg=null)
	{
		if ( ! mIsRunning )
			return;

        Team teamArg = Team.None;
        if(arg != null)
        {
            teamArg = ((EventManagerArg)arg).ToTeam();
        }

        switch (eventType)
		{
			case EventType.FlagCaptured:
			if (teamArg == Team.Purple)
				mPacket.PurpleRunner.FlagsCaptured++;
			else
				mPacket.YellowRunner.FlagsCaptured++;
			break;

			case EventType.TimeUp:
			mPacket.IsGameOver = true;
			break;

			case EventType.PlayerDied:
			if (teamArg == Team.Purple)
				mPacket.PurpleRunner.Deaths++;
			else
				mPacket.YellowRunner.Deaths++;
			break;

			case EventType.BlockPlaced:
			if (teamArg == Team.Purple)
				mPacket.PurpleBuilder.BlocksPlaced++;
			else 
				mPacket.YellowBuilder.BlocksPlaced++;
			break;
		}

		if (IsGameOver())
			mPacket.IsGameOver = true;

		// If host, signal Update() to broadcast scoreboard to clients.
		mPacketDirtyBit = true;
	}


	/// <summary>
	/// Stop the scoreboard from listening for game events and synchronization. this.mPacket will still
	/// be available to query the scores directly.
	/// </summary>
	public void Stop()
	{
		Debug.Log("Scoreboard.Stop()");
		mIsRunning = false;
	}





	private bool IsGameOver()
	{
		return (mPacket.PurpleRunner.FlagsCaptured >= 3 || mPacket.YellowRunner.FlagsCaptured >= 3 || mPacket.IsGameOver);
	}

	private void SendScoreboardState()
	{
		// If client or score hasn't been changed, no need to broadcast score to clients.
		if ( ! mIsHost || ! mPacketDirtyBit )
			return;

		mNwMgr.SendPacket<ScoreboardStatePacket>("info", mPacket);
		mPacketDirtyBit = false;
	}

	private void ReceiveScoreboardState()
	{
		// Only client can receive a score update.
		if (mIsHost)
			return;

		while (mNwMgr.PacketQueue.HasPacket<ScoreboardStatePacket>())
			mPacket = mNwMgr.PacketQueue.GetNextPacket<ScoreboardStatePacket>();
	}
}
