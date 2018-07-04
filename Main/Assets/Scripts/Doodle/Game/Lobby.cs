using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.Game;
using Doodle.Networking;
using Doodle.Networking.Packets;
using Doodle.Networking.Serialization;

public class Lobby : MonoBehaviour 
{
	public bool IsDebug = true;

	// Can interact with menu if needed (e.g. hide it).
	public MenusHandler Menus;

	// How client and server will discover and connect to each other before UDP/TCP connections are created: 
	// {InternetIP, InternetRoomCode, LocalUDPBroadcast, LocalWifiDirect}
	protected NetworkType nwType;

	// Module to allow host to create and advertise its public network (e.g. wifi direct group), and allows
	// client to discover and hop onto host's network to begin establishing UDP/TCP connections. 
	// Implementation to be used is dependent on this.nwType.
	protected IServerLookup serverLookup = null;

	// Lobby code (room code or IP address) to try when joining a lobby.
	protected string lobbyCode = null;

	// Handles UDP/TCP connections between server and clients.
	public NetworkManager NwMgr;

	// After successfully entering a lobby, toggle one of these for game engine to handle the lobby logic
	// such as broadcasting usernames, alerting server about changing roles, and etc.
	protected bool toggle_InLobbyHostLogic = false;
	protected bool toggle_InLobbyClientLogic = false;

	// True lobby state from the server.
	public LobbyStatePacket TrueLobbyState;

	public PlayerId myId;
	protected enum PlayerIdStatus {None, Requested, Received};
	protected PlayerIdStatus myIdStatus = PlayerIdStatus.None;

	// Need to delay packet sending a bit until NwMgr is fully initialized. NwMgr needs to establish connection event
	// after calling Run() before sending packets.
	protected enum inLobbyClientLogicStatus {NotStarted, DelayingStart, Started};
	protected inLobbyClientLogicStatus inLobbyClientLogicCurStatus = inLobbyClientLogicStatus.NotStarted;
	protected float inLobbyClientLogicStartDelayTime = 2;
	protected float inLobbyClientLogicStartDelayFinish;

	// Need to delay NwMgr shutdown after sending final packet that we are disconnecting from the lobby.
	protected enum nwMgrShutdownStatus {NotTriggered, Triggered, ShuttingDown, Done};
	protected nwMgrShutdownStatus nwMgrShutdownCurStatus = nwMgrShutdownStatus.NotTriggered;
	protected float nwMgrShutdownDelayTime = 2;
	protected float nwMgrShutdownDelayTimeFinish;

	// If don't receive alive packet within this time limit, disconnect.
	public float recvAlivePacketTimeout = 5f;
	protected float[] timeSinceAlivePacketRecv = new float[]{0f,0f,0f,0f};

	// Sends alive packet to clients every second.
	public float sentAlivePacketInterval = 1f;
	protected float timeSinceAlivePacketSent = 0f;

	protected bool isServer = false;


	/// <summary>
	/// Triggered event when network manager encounters a disconnect event.
	/// </summary>
	protected void ClientDisconnectEvent(INetworkConnection conn)
	{
		Debug.Log("Lobby.ClientDisconnectEvent()");

		// If hosting...
		if (toggle_InLobbyHostLogic)
		{
			// Remove disconnected player from lobby slot.
			PlayerId gonePlayer = DetermineClientPlayerId(conn);
			for (int i = 0; i < 4; i++)
			{
				if (TrueLobbyState.PlayerIds[i] == gonePlayer)
					TrueLobbyState.IsOccupieds[i] = false;
			}

			// Update() will broadcast and refresh GUI lobby changes, and update number of active players.
		}
		// If client that's in the lobby or in-game
		else if (toggle_InLobbyClientLogic)
		{
			Menus.OnClick_Disconnect("Lost connection to host.");
		}
	}



	void Start () 
	{
		
	}

	void Update ()
	{
		if (toggle_InLobbyHostLogic)
			InLobbyHostLogic();

		if (toggle_InLobbyClientLogic)
			InLobbyClientLogic();

		//if (toggle_InLobbyHostLogic || toggle_InLobbyClientLogic)
		//	AliveCheck();

		Update_Disconnect();
	}








	/* Functions that the menu will be invoking upon button presses */




	/// <summary>
	/// Step 1/3 of hosting a lobby.
	/// Create a network. Clients will be able to discover your network such that 
	/// your IP address will be visible and connectable by the clients.
	/// 
	/// Call HostGame_CreateNetworkResult() afterwards.
	/// </summary>
	/// 
	/// <param name="nwType">
	/// Network type to use: {InternetIP, InternetRoomCode, LocalUDPBroadcast, LocalWifiDirect}
	/// </param>
	public void HostLobby_CreateNetwork(NetworkType nwType)
	{
		if (IsDebug) Debug.Log("HostGame_CreateNetwork: Network Type - " + nwType);
		SetNetworkType(nwType);

		if (nwType == NetworkType.InternetIP)
		{
			// Nothing special needed. Clients will be able to connect to you (host) through 
			// the WWW network.
		}
		else 
		{
			// Along with advertising the network, this will also handle creating the network as necessary.
			this.serverLookup.AdvertiseServer();
		}
	}

	/// <summary>
	/// Step 2/3 of hosting a lobby.
	/// Get the result of HostGame_CreateNetwork().
	/// Menu should display a loading screen while repeatedly calling this function until success.
	/// Upon success, network manager will have been started to listen and accept client connections.
	/// </summary>
	/// 
	/// <returns>
	/// If success, returns lobby code that is either an IP address or 4-digit room code. Menu should
	/// make this visible. Lobby code will an IP address if InternetIP was passed to HostGame_CreateNetwork().
	/// If failure, returns "failure".
	/// If not ready yet, returns null.
	/// </returns>
	public string HostLobby_CreateNetworkResult()
	{
		if (this.nwType == NetworkType.InternetIP)
		{
			// Use "127.0.0.1" as our server IP.
			bool isSuccess = NwMgr.Run(NwMgr.ServerAddress, NetworkMode.Server);
			if ( ! isSuccess )
				return "failure";

			// But display public IP to share with clients to connect.
			string hostIP = NetworkHost.GetLocalAddress();
			if (IsDebug) Debug.Log("HostGame_CreateNetworkResult: Public IP - " + hostIP);
			return hostIP;
		}
		else if (this.nwType == NetworkType.InternetRoomCode)
		{
			string roomCode = this.serverLookup.AdvertiseServerResult();
			if (roomCode == null || roomCode == "failure")
			{
				return roomCode;
			}
			else
			{
				// Use "127.0.0.1" as our server IP.
				bool isSuccess = NwMgr.Run(NwMgr.ServerAddress, NetworkMode.Server);
				if ( ! isSuccess )
					return "failure";

				// But display 4-digit room code to share with clients to connect.
				return roomCode;
			}
		}
		else
		{
			string roomCode = this.serverLookup.AdvertiseServerResult();
			if (roomCode == null || roomCode == "failure")
			{
				return roomCode;
			}
			else
			{
				this.serverLookup.GetServerIP(null);
		
				string hostIP =  this.serverLookup.GetServerIPResult();
				bool useDnsIP = this.nwType != NetworkType.LocalWifiDirect;
				bool isSuccess = NwMgr.Run(hostIP, NetworkMode.Server, useDnsIP);
				if ( ! isSuccess )
					return "failure";

				return roomCode;
			}
		}
	}

	/// <summary>
	/// Step 3/3 of hosting a lobby.
	/// 
	/// This will cause this script to handle the logic involving hosting a lobby like 
	/// broadcasting the true lobby state to clients and receiving updates from clients.
	/// </summary>
	public void HostLobby_ToggleLobbyHostLogic(bool isOn)
	{
		if (isOn)
		{
			Debug.Log(NwMgr.NetworkHost);
			NwMgr.NetworkHost.Disconnected -= new ConnectionDisconnectedHandler(ClientDisconnectEvent);
			NwMgr.NetworkHost.Disconnected += new ConnectionDisconnectedHandler(ClientDisconnectEvent);
		}
		else 
		{
			if (NwMgr.NetworkHost != null)
				NwMgr.NetworkHost.Disconnected -= new ConnectionDisconnectedHandler(ClientDisconnectEvent);
		}

		this.TrueLobbyState = new LobbyStatePacket(true);
		this.toggle_InLobbyHostLogic = isOn;
	}





	/// <summary>
	/// Step 1/4 of joining a lobby.
	/// Join a hosted network such that you (client) will be able to establish UDP/TCP connections to the 
	/// host IP. All 5 functions must be called in order for this to happen.
	/// 
	/// Call JoinLobby_StartNetworkManager() afterwards.
	/// </summary>
	/// 
	/// <param name="nwType">
	/// Network type of the host: {InternetIP, InternetRoomCode, LocalUDPBroadcast, LocalWifiDirect}
	/// </param>
	/// 
	/// <param name="lobbyCode">
	/// Lobby code identifier of the host's network. If nwType is set to InternetIP, you will need to provide 
	/// the host's IP address. Otherwise, provide the 4-digit room code. 
	/// </param>
	public void JoinLobby_DiscoverIP(NetworkType nwType, string lobbyCode)
	{
		SetNetworkType(nwType);
		this.lobbyCode = lobbyCode;

		if (nwType == NetworkType.InternetIP)
		{
			// Nothing special needed. Client can already connect to server directly via public IP.
		}
		else 
		{
			// Along with retrieving the server's IP, this will also handle joining the host's network space
			// to retrieve the server's IP.
			this.serverLookup.GetServerIP(lobbyCode);
		}
	}

	/// <summary>
	/// Step 2/4 of Join Game.
	/// Start the network manager.
	/// Menu should display a loading screen while repeatedly calling this function until success.
	/// 
	/// Call JoinLobby_ConnectResult() afterwards if success.
	/// </summary>
	/// 
	/// <returns>
	/// If success, returns true.
	/// If failure, returns false.
	/// If not ready yet, returns null.
	/// </returns>
	public bool? JoinLobby_StartNetworkManager()
	{
		if (this.nwType == NetworkType.InternetIP)
		{
			string serverIP = this.lobbyCode;
			bool isSuccess = NwMgr.Run(serverIP, NetworkMode.Client);
			return isSuccess;
		}
		else 
		{
			string serverIP = this.serverLookup.GetServerIPResult();

			if (serverIP == null)
				return null;
			else if (serverIP == "failure")
				return false;
			else 
			{
				// At this point, we are within the host's network to establish UDP/TCP connections.
				bool useDnsIP = this.nwType != NetworkType.LocalWifiDirect;
				bool isSuccess = NwMgr.Run(serverIP, NetworkMode.Client, useDnsIP);
				return isSuccess;
			}
		}
	}

	/// <summary>
	/// Step 3/4 of Join Game.
	/// Inquire whether or not we have connected to the lobby.
	/// Menu should display a loading screen while repeatedly calling this function until success.
	/// 
	/// Call JoinLobby_ToggleLobbyClientLogic() afterwards if success.
	/// Call JoinLobby_DisposeNetworkManager() afterwards if give up or failure.
	/// </summary>
	/// 
	/// <returns>
	/// If connected, returns true.
	/// If not connected, returns null.
	/// </returns>
	public bool? JoinLobby_ConnectResult()
	{
		if (this.NwMgr.NetworkHost.ConnectionCount == 0)
			return null;
		else 
			return true;
	}

	/// <summary>
	/// Dispose the active network manager. Call this if JoinLobby_DisposeNetworkManager() fails or was gave up.
	/// </summary>
	public void JoinLobby_DisposeNetworkManager()
	{
		this.NwMgr.Stop();
	}


	/// <summary>
	/// Step 4/4 of joining a lobby.
	/// 
	/// This will cause this script to handle the logic of being in a lobby such as fetching 
	/// the true lobby state from the server, and sending your lobby changes.
	/// </summary>
	public void JoinLobby_ToggleLobbyClientLogic(bool isOn)
	{
		this.TrueLobbyState = new LobbyStatePacket(true);
		this.toggle_InLobbyClientLogic = isOn;

		if ( ! isOn )
		{
			if (NwMgr.NetworkHost != null)
				NwMgr.NetworkHost.Disconnected -= new ConnectionDisconnectedHandler(ClientDisconnectEvent);
			this.inLobbyClientLogicCurStatus = inLobbyClientLogicStatus.NotStarted;
		}
		else 
		{
			NwMgr.NetworkHost.Disconnected -= new ConnectionDisconnectedHandler(ClientDisconnectEvent);
			NwMgr.NetworkHost.Disconnected += new ConnectionDisconnectedHandler(ClientDisconnectEvent);
			this.inLobbyClientLogicCurStatus = inLobbyClientLogicStatus.DelayingStart;
		}
	}









	/// <summary>
	/// Disconnect from either the lobby or in-game. 
	/// </summary>
	public void Disconnect(bool isServer, NetworkType nwType, bool isHostTimeout=false)
	{
		if (NwMgr.IsRunning)
			NwMgr.NetworkHost.Disconnected -= new ConnectionDisconnectedHandler(ClientDisconnectEvent);

		this.isServer = isServer;
		SetNetworkType(nwType);

		// If in the lobby or in-game, and still connected to host
		if ((toggle_InLobbyClientLogic || toggle_InLobbyHostLogic) && ! isHostTimeout)
		{
			// Before sending disconnect signal, ensure another player is present to receive it.
			if (TrueLobbyState.NumActivePlayers > 1)
			{
				LobbyPlayerDisconnectPacket sendPkt = new LobbyPlayerDisconnectPacket(isServer);
				NwMgr.SendPacket<LobbyPlayerDisconnectPacket>("info", sendPkt);
			}

			HostLobby_ToggleLobbyHostLogic(false);
			JoinLobby_ToggleLobbyClientLogic(false);

			myIdStatus = PlayerIdStatus.None;
		}
			
		// Empty lobby state.
		TrueLobbyState = new LobbyStatePacket(true);

		// Trigger nwMgr.Stop() and this.serverLookup.Cancel() to be called in 2 seconds.
		nwMgrShutdownCurStatus = nwMgrShutdownStatus.Triggered;
	}

	protected void Update_Disconnect()
	{
		if (nwMgrShutdownCurStatus == nwMgrShutdownStatus.Triggered)
		{
			if (IsDebug) Debug.Log("Lobby.cs :: NwMgr shutdown has commenced.");
			nwMgrShutdownDelayTimeFinish = Time.time + nwMgrShutdownDelayTime;
			nwMgrShutdownCurStatus = nwMgrShutdownStatus.ShuttingDown;
		}
		else if (nwMgrShutdownCurStatus == nwMgrShutdownStatus.ShuttingDown)
		{
			if (Time.time > nwMgrShutdownDelayTimeFinish)
			{
				if (IsDebug) Debug.Log("Lobby.cs :: NwMgr shutdown is invoked.");
				NwMgr.Stop();

				SetNetworkType(nwType);
				if (nwType == NetworkType.InternetIP || (nwType == NetworkType.InternetRoomCode && !this.isServer))
					;
				else 
					this.serverLookup.Cancel();

				nwMgrShutdownCurStatus = nwMgrShutdownStatus.Done;

				Debug.Log("Update_Disconnect() invoked.");
			}
		}
	}

	public string DisconnectResult()
	{
		// If network manager is still stopping or Cancel() pending, wait.
		if (nwMgrShutdownCurStatus == nwMgrShutdownStatus.Triggered ||
			nwMgrShutdownCurStatus == nwMgrShutdownStatus.ShuttingDown)
			return null;

		if (nwType == NetworkType.InternetIP || (nwType == NetworkType.InternetRoomCode && !this.isServer))
			return "success";
		else 
			return this.serverLookup.CancelResult();
	}






	/* Helper functions for the above menu functions */

	/// <summary>
	/// Set the network type to use.
	/// </summary>
	/// 
	/// <param name="nwType">
	/// Network type to use:  {InternetRoomCode, LocalUDPBroadcast, LocalWifiDirect}
	/// </param>
	protected void SetNetworkType(NetworkType nwType)
	{
		this.nwType = nwType;

		if (nwType == NetworkType.InternetIP)
			this.serverLookup = null;
		else if (nwType == NetworkType.InternetRoomCode)
			this.serverLookup = this.GetComponent<ServerLookupHTTP>();
		else if (nwType == NetworkType.LocalUDPBroadcast)
			this.serverLookup = this.GetComponent<ServerLookupUDPBroadcast>();
		else if (nwType == NetworkType.LocalWifiDirect)
			this.serverLookup = this.GetComponent<ServerLookupWifiDirect>();
	}






	/* These functions below are called by Update(). They handle packet-received events relating to the 
	   lobby state. */




	/// <summary>
	/// As the host of the lobby, handle any received packets from my clients.
	/// </summary>
	protected void InLobbyHostLogic()
	{
		int prevNumActivePlayers = TrueLobbyState.NumActivePlayers;
		TrueLobbyState.NumActivePlayers = 1 + NwMgr.NetworkHost.ConnectionCount;

		// If number of active players have changed...
		if (prevNumActivePlayers != TrueLobbyState.NumActivePlayers)
			TrueLobbyState.IsDirty = true;

		// At the start, assign myself the first player Id.
		if (myIdStatus == PlayerIdStatus.None)
		{
			if (IsDebug) Debug.Log("InLobbyHostLogic: Assigning myself a player id.");

			myId = PlayerId.One;
			myIdStatus = PlayerIdStatus.Received;
		}

		// A new client has joined the lobby and is requesting a unique player ID. 
		if (NwMgr.PacketQueue.HasPacket<LobbyRequestPlayerIdPacket>())
		{
			if (IsDebug) Debug.Log("InLobbyHostLogic: Got new player event.");

			INetworkConnection recvNwConn;
			NwMgr.PacketQueue.GetNextPacket<LobbyRequestPlayerIdPacket>(out recvNwConn);

			// Give client an ID relative to its incoming channel ID.

			LobbyGivenPlayerIdPacket sendPkt = new LobbyGivenPlayerIdPacket();
			sendPkt.PlayerId = DetermineClientPlayerId(recvNwConn);
			NwMgr.SendPacket<LobbyGivenPlayerIdPacket>("info", recvNwConn.Id, sendPkt);

			// NumActivePlayers will be handled at the top of this function.
		}

		// Received update from client (e.g. changed role).
		if (NwMgr.PacketQueue.HasPacket<LobbyUpdateFromClientPacket>())
		{
			if (IsDebug) Debug.Log("InLobbyHostLogic: Got lobby update from client.");

			INetworkConnection recvNwConn;
			LobbyUpdateFromClientPacket recvPkt = NwMgr.PacketQueue.GetNextPacket<LobbyUpdateFromClientPacket>(out recvNwConn);

			PlayerId clientId = DetermineClientPlayerId(recvNwConn);
			TrueLobbyState.MergeUpdateFromClient(recvPkt, clientId);

			TrueLobbyState.IsDirty = true;
		}

		// Received alert that a client is disconnecting from server.
		if (NwMgr.PacketQueue.HasPacket<LobbyPlayerDisconnectPacket>())
		{
			if (IsDebug) Debug.Log("InLobbyHostLogic: Client is disconnecting.");

			INetworkConnection recvNwConn;
			NwMgr.PacketQueue.GetNextPacket<LobbyPlayerDisconnectPacket>(out recvNwConn);

			PlayerId lossId = DetermineClientPlayerId(recvNwConn);
			TrueLobbyState.RemoveClient(lossId);

			TrueLobbyState.IsDirty = true;
		}
			
		if (TrueLobbyState.IsDirty)
		{
			// Broadcast new lobby state.
			NwMgr.SendPacket<LobbyStatePacket>("info", TrueLobbyState);

			// Refresh my lobby GUI.
			Menus.RefreshLobby(TrueLobbyState, myId);

			TrueLobbyState.IsDirty = false;
		}
	}

	/// <summary>
	/// As a client of the lobby, handle any received-packet events from the host of the lobby.
	/// </summary>
	protected void InLobbyClientLogic()
	{
		// Need to delay a bit before transferring packets. NwMgr needs to establish connection event after calling Run()
		// before sending packets.
		if (inLobbyClientLogicCurStatus == inLobbyClientLogicStatus.NotStarted)
		{
			inLobbyClientLogicStartDelayFinish = Time.time + inLobbyClientLogicStartDelayTime;
			inLobbyClientLogicCurStatus = inLobbyClientLogicStatus.DelayingStart;
			return;
		}
		else if (inLobbyClientLogicCurStatus == inLobbyClientLogicStatus.DelayingStart)
		{
			if (Time.time > inLobbyClientLogicStartDelayFinish)
				inLobbyClientLogicCurStatus = inLobbyClientLogicStatus.Started;
			else
				return;
		}
			


		// If just joined the lobby, request a player id.
		if (myIdStatus == PlayerIdStatus.None)
		{
			if (IsDebug) Debug.Log("InLobbyClientLogic: Requesting a player id.");

			NwMgr.SendPacket<LobbyRequestPlayerIdPacket>("info", new LobbyRequestPlayerIdPacket());
			myIdStatus = PlayerIdStatus.Requested;
		}

		// If received my requested player id.
		if (NwMgr.PacketQueue.HasPacket<LobbyGivenPlayerIdPacket>())
		{
			LobbyGivenPlayerIdPacket recvPkt = NwMgr.PacketQueue.GetNextPacket<LobbyGivenPlayerIdPacket>();
			myId = recvPkt.PlayerId;
			myIdStatus = PlayerIdStatus.Received;
			if (IsDebug) Debug.Log("InLobbyClientLogic: Received my player id: " + myId);
		}



		// Retrieved true lobby state from server.
		if (NwMgr.PacketQueue.HasPacket<LobbyStatePacket>())
		{
			if (IsDebug) Debug.Log("InLobbyClientLogic: Received lobby state from server.");
			TrueLobbyState = NwMgr.PacketQueue.GetNextPacket<LobbyStatePacket>();

			Menus.RefreshLobby(TrueLobbyState, myId);

			// Go in-game mode if signalled by server.
			if (TrueLobbyState.IsStartGame)
			{
				Debug.Log("Lobby.cs: Got packet to start game. Time: " + Time.time);
				Menus.OnClick_StartGame();
			}
		}
			
		// Received alert that server is disconnecting. Exit the lobby.
		if (NwMgr.PacketQueue.HasPacket<LobbyPlayerDisconnectPacket>())
		{
			if (IsDebug) Debug.Log("InLobbyClientLogic: Received alert that server is disconnecting.");
			NwMgr.PacketQueue.GetNextPacket<LobbyPlayerDisconnectPacket>();

			Menus.OnClick_Disconnect("Server has ended the game.");
			return;
		}

		// Received signal to return to the lobby.
		if (NwMgr.PacketQueue.HasPacket<LobbyReturnPacket>())
		{
			if (IsDebug) Debug.Log("InLobbyClientLogic: Received signal to return to lobby.");
			NwMgr.PacketQueue.GetNextPacket<LobbyReturnPacket>();

			Menus.OnClick_ReturnToLobby();
		}
	}

	/// <summary>
	/// Send alive packets at regularly intervals. In addition, receive and handle alive packets. If don't receive
	/// alive packet in time, automatically disconnect.
	/// 
	/// TODO: Take a look at ClientDisconnectEvent() and isServer body code of this function. This function 
	/// has been commented out in Update().
	/// </summary>
	public void AliveCheck()
	{
		// If I am the server and there are no connected clients, no need to deal with alive packets.
		if (this.isServer && TrueLobbyState.NumActivePlayers == 1)
			return;

		// Send "server alive" packet regularly.
		this.timeSinceAlivePacketSent += Time.deltaTime;
		if (this.timeSinceAlivePacketSent > this.sentAlivePacketInterval)
		{
			if (IsDebug) Debug.Log("AliveCheck: Sent alive packet.");
			NwMgr.SendPacket<AlivePacket>("input", new AlivePacket());
			this.timeSinceAlivePacketSent = 0;
		}

		// Receive alive packets regularly to know that sender (client or server) is still alive.
		if (NwMgr.PacketQueue.HasPacket<AlivePacket>())
		{
			INetworkConnection recvNwConn;
			NwMgr.PacketQueue.GetNextPacket<AlivePacket>(out recvNwConn);

			if (this.isServer)
			{
				PlayerId playerId = DetermineClientPlayerId(recvNwConn);
				if (IsDebug) Debug.Log("AliveCheck: Received alive packet from playerId#"+playerId+".");
				this.timeSinceAlivePacketRecv[(int)playerId] = 0;
			}
			else 
			{
				if (IsDebug) Debug.Log("AliveCheck: Received alive packet from server.");
				this.timeSinceAlivePacketRecv[0] = 0;
			}
		}
			
		// Increment time since received alive packet.
		for (int playerId = 0; playerId < 4; playerId++)
			this.timeSinceAlivePacketRecv[(int)playerId] += Time.deltaTime;
		
		// If haven't received alive packet for a long time, disconnect from lobby/in-game.
		if (this.isServer)
		{
			// For each connected client...
			foreach (INetworkConnection conn in NwMgr.NetworkHost.Connections)
			{
				PlayerId clientId = DetermineClientPlayerId(conn);
				if (this.timeSinceAlivePacketRecv[(int)clientId] > this.recvAlivePacketTimeout)
				{
					if (IsDebug) Debug.Log("AliveCheck: Failed to receive alive packet in time from player ID: " + 
										   clientId);

					Menus.OnClick_Disconnect("Lost connection to client #"+clientId+". Timeout exceeded.");
				}
			}
		}
		else 
		{
			if (this.timeSinceAlivePacketRecv[0] > this.recvAlivePacketTimeout)
			{
				if (IsDebug) Debug.Log("AliveCheck: Failed to receive alive packet in time from server.");

				Menus.OnClick_Disconnect("Lost connection to server. Timeout exceeded.");
			}
		}
	}

	/// <summary>
	/// Let server know about my lobby changes (e.g. changed role). This is called by the main menu.
	/// </summary>
	public void SendLobbyUpdateFromClient(LobbyUpdateFromClientPacket clientUpdate)
	{
		NwMgr.SendPacket<LobbyUpdateFromClientPacket>("info", clientUpdate);
	}
		
	/// <summary>
	/// As host, update the true lobby state locally and broadcast this new state to every client. 
	/// </summary>
	/// <param name="newTrueLobbyState">New true lobby state.</param>
	public void SendNewTrueLobbyState(LobbyStatePacket newTrueLobbyState)
	{
		TrueLobbyState = newTrueLobbyState;
		NwMgr.SendPacket<LobbyStatePacket>("info", TrueLobbyState);
	}

	/// <summary>
	/// Players are assigned a unique ID by the server based on client's connection ID from the server
	/// perspective. 
	/// </summary>
	/// 
	/// <param name="nwConn">
	/// Connection info from a client packet which can be retrieved from
	/// NetworkManager.PacketQueue.GetNextPacket<T>(out nwConn)
	/// </param>
	public PlayerId DetermineClientPlayerId(INetworkConnection nwConn)
	{
		return ((PlayerId)nwConn.Id);
	}

	public TeamRole DetermineTeamRole(INetworkConnection nwConn)
	{
		PlayerId playerId = DetermineClientPlayerId(nwConn);
		for (int i = 0; i < 4; i++)
		{
			if (TrueLobbyState.PlayerIds[i] == playerId)
				return (TeamRole)i;
		}

		Debug.LogError("Lobby.DetermineTeamRole() failed to find team role of network connection.");
		return (TeamRole)0;
	}


	/// <summary>
	/// As a server,
	/// 	- Send LobbyReturnPacket to trigger clients to return to the lobby. 
	/// 	- Send the latest lobby state to the clients for them to refresh their lobby menu.
	/// 	- Refresh my own lobby menu.
	/// 
	///	As a server, you'll need to switch to the lobby menu GUI manually.
	/// </summary>
	public void ServerReturnToLobby()
	{
		NwMgr.SendPacket<LobbyReturnPacket>("info", new LobbyReturnPacket());

		//TrueLobbyState.IsStartGame = false;
		TrueLobbyState = new LobbyStatePacket(true);	// Reset lobby slots.
		TrueLobbyState.NumActivePlayers = 1 + NwMgr.NetworkHost.ConnectionCount;

		SendNewTrueLobbyState(TrueLobbyState);
		Menus.RefreshLobby(TrueLobbyState, myId);
	}

	/// <summary>
	/// Disconnect from lobby when Unity player closes.
	/// </summary>
	void OnApplicationQuit()
	{
		if (toggle_InLobbyClientLogic)
			Disconnect(false, nwType);
		else if (toggle_InLobbyHostLogic)
			Disconnect(true, nwType);
	}
}
