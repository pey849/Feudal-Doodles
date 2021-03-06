using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using UnityEngine.EventSystems;
using Doodle.Game;
using Doodle.Networking;
using Doodle.Networking.Packets;

/// <summary>
/// Handles menu functionalities such as buttons.
/// </summary>
public class MenusHandler : MonoBehaviour
{
	public bool mIsDebug = true;

	public Lobby mLobby;
	public GameEngine mEngine;

	// Available menus.
	public GameObject mStartMenu;
	public GameObject mMultiplayerTypeMenu;
	public GameObject mUsernameMenu;
	public GameObject mHostOrFindLobbyMenu;
	public GameObject mClientEnterLobbyCodeMenu;
	public GameObject mLobbyMenu;
	public GameObject mInGameMenu;
	public GameObject mScoreboardMenu;
	public GameObject mTutorialPlatformer;
	public GameObject mTutorialPlatformerPC;
    public GameObject mTutorialPlatformerAbilities;
    public GameObject mTutorialBuilder;
	public GameObject mTutorialBuilderPC;
    public GameObject mTutorialBlock;
    public GameObject mTutorialSafeZones;
    public GameObject mTutorialGameObjectives;
	public GameObject mCredits;

	public GameObject lobbyTutorialPlatformer;
	public GameObject lobbyTutorialPlatformerPC;
	public GameObject lobbyTutorialPlatformerAbilities;
	public GameObject lobbyTutorialBuilder;
	public GameObject lobbyTutorialBuilderPC;
	public GameObject lobbyTutorialBlock;
	public GameObject lobbyTutorialSafeZones;

	// UI widgets within the menus.
	public Button mMultiplayerTypeWifiDirect_but;
	public InputField mUsernameMenu_field;
	public Text mUsernameMenu_infoText;
	public Button mUsernameMenu_nextBut;
	public Text mClientEnterLobbyCodeMenu_infoText;
	public InputField mClientEnterLobbyCodeMenu_codeField;
	public Text mLobbyMenu_playerCountText;
	public Text mLobbyMenu_lobbyCodeWithHeaderText;
	public Button mLobbyMenu_startGameBut;
	public Button mInGameMenu_menuBut;
	public Button mInGameMenu_resumeBut;
	public Button mInGameMenu_disconnectBut;
	public Text mScoreboardMenu_youWinLoseText;
	public Text mScoreboardMenu_purpleScoreText;
	public Text mScoreboardMenu_yellowScoreText;
	public Button mScoreboardMenu_returnToLobbyBut;

	// 4 scoreboard slots for the 4 players.
	[System.Serializable]
	public class ScoreboardSlot
	{
		public TeamRole teamRole;
		public Text roleUsername;
		public Image avatar;
		public Text summary;
	}
	public List<ScoreboardSlot> mScoreboardSlots;

	// 4 lobby slots for the 4 players.
	[System.Serializable]
	public class LobbySlot 
	{
		public TeamRole teamRole;
		public string username;
		public bool isHost;
		public bool isOccupied;
		public bool isYou;

		public Button joinButton;
		public Button leaveRoleButton;
		public GameObject playerInfoContainer;
		public Text usernameText;
//		public Toggle isReadyToggle;
	}
	public List<LobbySlot> mLobbySlots;

	// Loading Splash 
	public GameObject mLoadingSplashPanel;
	public Text mLoadingSplashText;
	public Button mLoadingSplashCloseButton;

	// Attributes to track (e.g. inputted username)
	protected GameObject mCurMenu;
	protected bool mIsLoadingSplashShown;
	protected string mUsername = "";
	protected bool mIsHost = false;
	protected NetworkType mMP_Mode;
	// "192.168.01" or "5966" format.
	protected string mLobbyCode;

	// This handles the frequency that Update() does it work, such as inquiring whether or 
	// not the lobby has been joined yet.
	protected float mUpdate_nextTime;
	protected float mUpdate_intervalTime = 0.5f;
	protected float mUpdate_maxDuration = 15.0f;
	protected float mUpdate_maxDuration_wifidirect_ext = 50.0f;
	protected float mUpdate_endTime;
	protected enum JoinStatus {None, Disconnected, NetworkStarting, NetworkStarted};
	protected JoinStatus mUpdate_joinStatus = JoinStatus.None;
	protected enum HostStatus {None, Disconnected, NetworkCreating, NetworkCreated};
	protected HostStatus mUpdate_hostStatus = HostStatus.None;
	protected enum DisconnectStatus {None, Disconnected};
	protected DisconnectStatus mUpdate_disconnectStatus = DisconnectStatus.None;

	protected bool mIsLobbyRefreshed = false;

	void Start () 
	{
		mCurMenu = mStartMenu;
		mUsernameMenu_nextBut.interactable = false;
		mIsLoadingSplashShown = false;

		#if !UNITY_ANDROID || UNITY_EDITOR
			mMultiplayerTypeWifiDirect_but.interactable = false;
		#endif
	}
	

	void Update () 
	{
		Update_CreatingLobby();
		Update_JoiningLobby();	

		Update_HostDetermineStartGameOK();

		Update_Disconnect();

//		if (Input.GetKeyDown("escape"))
//		{
//			ShowLoadingSplash("loading...");
//			RevealCloseLoadingSplashButton();
//		}
//
	}



	protected void SwitchMenu(GameObject menu)
	{
		mCurMenu.SetActive(false);
		mCurMenu = menu;
		mCurMenu.SetActive(true);

		if (mCurMenu == mLobbyMenu)
			mLobbyMenu_startGameBut.gameObject.SetActive(mIsHost);
		else if (mCurMenu == mClientEnterLobbyCodeMenu)
			mClientEnterLobbyCodeMenu_codeField.text = "";
		else if (mCurMenu == mUsernameMenu)
			OnInputFieldChange_Username();
		else if (mCurMenu == mInGameMenu)
		{
			mInGameMenu_menuBut.gameObject.SetActive(true);
			mInGameMenu_resumeBut.gameObject.SetActive(false);
			mInGameMenu_disconnectBut.gameObject.SetActive(false);
		}
	}






	/* 1 - Main Menu Buttons */

	/// <summary>
	/// Raises the click event when the PLAY button is pressed.
	/// </summary>
	public void OnClick_Play()
	{
		SwitchMenu(mMultiplayerTypeMenu);
	}

	/// <summary>
	/// Raises the click event when the QUIT button is pressed.
	/// </summary>
	public void OnClick_Quit()
	{
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false; 
		#else
			Application.Quit();
		#endif
	}

	/// <summary>
	/// Raises the click event when the HOW TO PLAY button is pressed
	/// Added by Kent
	/// </summary>
	public void OnClick_HowToPlay()
	{
		SwitchMenu(mTutorialGameObjectives);
	}


	/// <summary>
	/// Raises the click event when the HOW TO PLAY button is pressed
	/// Added by Peter
	/// </summary>
	public void OnClick_LobbyHowToPlay()
	{
		
		#if !UNITY_ANDROID
		SwitchMenu(lobbyTutorialPlatformerPC);
		#else
		SwitchMenu(lobbyTutorialPlatformer);
		#endif
		

	}

	/// <summary>
	/// Raises the click event when the HOW TO PLAY button is pressed
	/// Added by Peter
	/// </summary>
	public void OnClick_Credits()
	{
		SwitchMenu(mCredits);
	}

	/// <summary>
	/// Raises the click event when the BACK button is pressed.
	/// </summary>
	public void OnClick_Back()
	{
        if (mCurMenu == mMultiplayerTypeMenu)
            SwitchMenu(mStartMenu);
        else if (mCurMenu == mUsernameMenu)
            SwitchMenu(mMultiplayerTypeMenu);
        else if (mCurMenu == mHostOrFindLobbyMenu)
            SwitchMenu(mUsernameMenu);
        else if (mCurMenu == mClientEnterLobbyCodeMenu)
            SwitchMenu(mHostOrFindLobbyMenu);
        else if (mCurMenu == mLobbyMenu)
        {
            //mIsHost = false;
            SwitchMenu(mHostOrFindLobbyMenu);
            OnClick_Disconnect();
        }
        /* added by Kent */
        else if (mCurMenu == mTutorialGameObjectives)
            SwitchMenu(mStartMenu);
        else if (mCurMenu == mTutorialPlatformer)
            SwitchMenu(mTutorialGameObjectives);
		else if (mCurMenu == mTutorialPlatformerPC)
			SwitchMenu(mTutorialGameObjectives);
		else if (mCurMenu == mTutorialPlatformerAbilities){
			#if !UNITY_ANDROID
			SwitchMenu(mTutorialPlatformerPC);
			#else
			SwitchMenu(mTutorialPlatformer);
			#endif
		}
        else if (mCurMenu == mTutorialBuilder)
            SwitchMenu(mTutorialPlatformerAbilities);
		else if (mCurMenu == mTutorialBuilderPC)
			SwitchMenu(mTutorialPlatformerAbilities);
		else if (mCurMenu == mTutorialBlock) {
			#if !UNITY_ANDROID
			SwitchMenu(mTutorialBuilderPC);
			#else
			SwitchMenu(mTutorialBuilder);
			#endif
		}else if(mCurMenu == mTutorialSafeZones) {
            SwitchMenu(mTutorialBlock);
        }
		/*added by Peter*/
		else if(mCurMenu == mCredits){
			SwitchMenu(mStartMenu);
		}
		else if (mCurMenu == lobbyTutorialPlatformer || mCurMenu == lobbyTutorialPlatformerPC)
			SwitchMenu(mLobbyMenu);
		else if (mCurMenu == lobbyTutorialPlatformerAbilities) {
			#if !UNITY_ANDROID
			SwitchMenu(lobbyTutorialPlatformerPC);
			#else
			SwitchMenu(lobbyTutorialPlatformer);
			#endif
		}else if (mCurMenu == lobbyTutorialBuilder || mCurMenu == lobbyTutorialBuilderPC)
			SwitchMenu(lobbyTutorialPlatformerAbilities);
		else if (mCurMenu == lobbyTutorialBlock) {
			#if !UNITY_ANDROID
			SwitchMenu(lobbyTutorialBuilderPC);
			#else
			SwitchMenu(lobbyTutorialBuilder);
			#endif
		}else if(mCurMenu == lobbyTutorialSafeZones) {
			SwitchMenu(lobbyTutorialBlock);
		}
	}





	/* 2 - Multiplayer Type Menu Buttons */

	public void OnClick_MultiplayerType(string mode)
	{
		mMP_Mode = (NetworkType) Enum.Parse(typeof(NetworkType), mode);
		SwitchMenu(mUsernameMenu);
	}




	/* 3 - Username Menu Buttons */

	public bool IsUsernameOK(string name)
	{
		return Regex.IsMatch(name, "^(\\w| )+$") &&
			!Regex.IsMatch(name, " {2,}") && 
			!Regex.IsMatch(name, "(^ )|( $)");
	}

	public void OnInputFieldChange_Username()
	{
		mUsername = mUsernameMenu_field.text;

		if (mUsername == "")
		{
			mUsernameMenu_infoText.text = "Enter your username.";
			mUsernameMenu_nextBut.interactable = false;
		}
		else if (IsUsernameOK(mUsername))
		{
			mUsername = mUsernameMenu_field.text;
			mUsernameMenu_infoText.text = "Username is valid!";
			mUsernameMenu_nextBut.interactable = true;
		}
		else 
		{
			mUsername = null;
			mUsernameMenu_infoText.text = "Username is invalid. Please use" +
				" alphanumeric characters only!";
			mUsernameMenu_nextBut.interactable = false; 
		}
	}

	public void OnClick_Next()
	{
		SwitchMenu(mHostOrFindLobbyMenu);
	}

	/* Added by Kent */
	public void OnClick_NextTutorial()
	{
		if (mCurMenu == mTutorialGameObjectives) {
			#if !UNITY_ANDROID
			SwitchMenu(mTutorialPlatformerPC);
			#else
			SwitchMenu(mTutorialPlatformer);
			#endif
		}
		else if (mCurMenu == mTutorialPlatformer || mCurMenu == mTutorialPlatformerPC)
			SwitchMenu(mTutorialPlatformerAbilities);
		else if (mCurMenu == mTutorialPlatformerAbilities){
			#if !UNITY_ANDROID
			SwitchMenu(mTutorialBuilderPC);
			#else
			SwitchMenu(mTutorialBuilder);
			#endif
		}
		else if (mCurMenu == mTutorialBuilder || mCurMenu == mTutorialBuilderPC)
			SwitchMenu(mTutorialBlock);
        else if (mCurMenu == mTutorialBlock)
            SwitchMenu(mTutorialSafeZones);
        else if (mCurMenu == mTutorialSafeZones)
            SwitchMenu(mStartMenu);
    }


	/* Added by Peter */
	public void OnClick_NextLobbyTutorial()
	{
		if (mCurMenu == lobbyTutorialPlatformer || mCurMenu == lobbyTutorialPlatformerPC)
			SwitchMenu(lobbyTutorialPlatformerAbilities);
		else if (mCurMenu == lobbyTutorialPlatformerAbilities){
			#if !UNITY_ANDROID
			SwitchMenu(lobbyTutorialBuilderPC);
			#else
			SwitchMenu(lobbyTutorialBuilder);
			#endif
		}
		else if (mCurMenu == lobbyTutorialBuilder || mCurMenu == lobbyTutorialBuilderPC)
			SwitchMenu(lobbyTutorialBlock);
		else if (mCurMenu == lobbyTutorialBlock)
			SwitchMenu(lobbyTutorialSafeZones);
		else if (mCurMenu == lobbyTutorialSafeZones)
			SwitchMenu(mLobbyMenu);
	}



	/* 4 - Host or Find Lobby Menu Buttons */

	/// <summary>
	/// Event that occurs when the "Host Game" button is pressed.
	/// </summary>
	public void OnClick_Host()
	{
		mIsHost = true;

		ShowLoadingSplash("Creating lobby...");

		// Start timer.
		mUpdate_nextTime = Time.time + mUpdate_intervalTime;
		mUpdate_endTime = Time.time + mUpdate_maxDuration;
	
		mLobby.Disconnect(mIsHost, mMP_Mode);
		mUpdate_hostStatus = HostStatus.Disconnected;
	}

	/// <summary>
	/// This is called by Update() to repeatedly inquire status of successful lobby creation).
	/// 
	/// It is only active if OnClick_Host() is clicked.
	/// </summary>
	protected void Update_CreatingLobby()
	{
		if (mUpdate_hostStatus == HostStatus.None)
			return;		// Not invoked yet.
		
		if (Time.time > mUpdate_endTime)
		{
			// Timeout. Report Failure.
			Host_Post(false, null);
		}
		else if (Time.time > mUpdate_nextTime)
		{
			if (mUpdate_hostStatus == HostStatus.Disconnected)
			{
				string result = mLobby.DisconnectResult();
				if (result == null)
					; // Try again later.
				else if (result == "failure")
					Host_Post(false, null);	 // Report failure.
				else 
					mUpdate_hostStatus = HostStatus.NetworkCreating;	 // Success. Next phase.
			}
			else if (mUpdate_hostStatus == HostStatus.NetworkCreating)
			{
				mLobby.HostLobby_CreateNetwork(mMP_Mode);
				mUpdate_hostStatus = HostStatus.NetworkCreated;
			}
			else if (mUpdate_hostStatus == HostStatus.NetworkCreated)
			{
				string lobbyCode = mLobby.HostLobby_CreateNetworkResult();
				if (lobbyCode == null)
					; // Try again later.
				else if (lobbyCode == "failure")	
					Host_Post(false, null);		// Report failure.
				else
					Host_Post(true, lobbyCode);		// Report success.
			}
				
			mUpdate_nextTime = Time.time + mUpdate_intervalTime;
		}
	}

	/// <summary>
	/// The event that occurs after we received the status of hosting a game. This is a followup of Update_Host().
	/// </summary>
	protected void Host_Post(bool isSuccess, string lobbyCode)
	{
		mUpdate_hostStatus = HostStatus.None;

		if (isSuccess)
		{
			UnoccupyLobbySlotAll();
			HideLoadingSplash();

			mLobbyCode = lobbyCode;
			if (mMP_Mode == NetworkType.InternetIP) 
				mLobbyMenu_lobbyCodeWithHeaderText.text = "Lobby IP: " + mLobbyCode;
			else
				mLobbyMenu_lobbyCodeWithHeaderText.text = "Lobby Room Code: " + mLobbyCode;

			SwitchMenu(mLobbyMenu);
			mLobby.HostLobby_ToggleLobbyHostLogic(true);
		}
		else
		{
			UpdateLoadingSplashMessage("Failed to create a lobby.");
			RevealCloseLoadingSplashButton();
		}
	}






	/// <summary>
	/// Event that occurs when the "Join" button is pressed. User will be brought to a menu to join a game.
	/// </summary>
	public void OnClick_Join()
	{
		mIsHost = false;

		if (mMP_Mode == NetworkType.InternetIP) 
		{
			mClientEnterLobbyCodeMenu_infoText.text = "Type in the host's IP address.";
			mClientEnterLobbyCodeMenu_codeField.characterLimit = 39;
			mClientEnterLobbyCodeMenu_codeField.contentType = InputField.ContentType.Standard;
			mClientEnterLobbyCodeMenu_codeField.placeholder.GetComponent<Text>().text = "176.49.108.27";

			// Adjust input field location and width.
			RectTransform rectTrans = mClientEnterLobbyCodeMenu_codeField.GetComponent<RectTransform>();
			rectTrans.sizeDelta = new Vector2(331, rectTrans.sizeDelta.y);
			rectTrans.localPosition = new Vector2(195f, rectTrans.localPosition.y);
		}
		else
		{
			mClientEnterLobbyCodeMenu_infoText.text = "Type in the host's 4-digit room code.";
			mClientEnterLobbyCodeMenu_codeField.characterLimit = 4;
			mClientEnterLobbyCodeMenu_codeField.contentType = InputField.ContentType.IntegerNumber;
			mClientEnterLobbyCodeMenu_codeField.placeholder.GetComponent<Text>().text = "5709";

			// Adjust input field location and width.
			RectTransform rectTrans = mClientEnterLobbyCodeMenu_codeField.GetComponent<RectTransform>();
			rectTrans.sizeDelta = new Vector2(136.7f, rectTrans.sizeDelta.y);
			rectTrans.localPosition = new Vector2(68.8f, rectTrans.localPosition.y);
		}

		SwitchMenu(mClientEnterLobbyCodeMenu);
	}



	/* 4.5 - Client - Enter Lobby Code Menu */

	/// <summary>
	/// Event that occurs when the "Search" button is pressed. The specified lobby will be queried and joined.
	/// </summary>
	public void OnClick_Search()
	{
		mLobbyCode = mClientEnterLobbyCodeMenu_codeField.text;
		if (mIsDebug) Debug.Log("OnClick_Search(): Lobby Code - " + mLobbyCode);

		ShowLoadingSplash("Searching lobby...");

		// Start the timer.
		mUpdate_nextTime = Time.time + mUpdate_intervalTime;
		mUpdate_endTime = Time.time + mUpdate_maxDuration;
		if (mMP_Mode == NetworkType.LocalWifiDirect) mUpdate_endTime += mUpdate_maxDuration_wifidirect_ext;

		mLobby.Disconnect(mIsHost, mMP_Mode);
		mUpdate_joinStatus = JoinStatus.Disconnected;
	}

	/// <summary>
	/// This is called by Update() repeatedly to inquire the status of the success of joining the lobby. This function 
	/// is only triggered if OnClick_Search() is called.
	/// </summary>
	protected void Update_JoiningLobby()
	{
		if (mUpdate_joinStatus == JoinStatus.None)
			return;
		
		if (Time.time > mUpdate_endTime)
		{
			if (mUpdate_joinStatus == JoinStatus.NetworkStarted)
				mLobby.JoinLobby_DisposeNetworkManager();

			// Timeout. Report Failure.
			Search_Post(false);
			return;
		}
		else if  (Time.time > mUpdate_nextTime)
		{
			if (mUpdate_joinStatus == JoinStatus.Disconnected)
			{
				string result = mLobby.DisconnectResult();
				if (result == null)
					; // Try again later.
				else if (result == "failure")
					Search_Post(false);	 // Report failure.
				else 
				{
					// Success. Next phase.
					mLobby.JoinLobby_DiscoverIP(mMP_Mode, mLobbyCode);
					mUpdate_joinStatus = JoinStatus.NetworkStarting;	
				}
			}
			else if (mUpdate_joinStatus == JoinStatus.NetworkStarting)
			{
				bool? rv = mLobby.JoinLobby_StartNetworkManager();
				if (rv == null)
					;   // Try again later.
				else if (rv == true)
					mUpdate_joinStatus = JoinStatus.NetworkStarted;		// Success. Next phase.
				else
					Search_Post(false);		// Report failure.
			}
			else if (mUpdate_joinStatus == JoinStatus.NetworkStarted)
			{
				bool? rv = mLobby.JoinLobby_ConnectResult();
				if (rv == null)
					;   // Try again later.
				else if (rv == true)
					Search_Post(true);		// Report success.
				else
					Search_Post(false);		// Report failure.
			}

			mUpdate_nextTime = Time.time + mUpdate_intervalTime;
		}
	}

	/// <summary>
	/// The event that occurs when we received the status of joining the lobby. This is a follow-up of Update_Search().
	/// </summary>
	protected void Search_Post(bool isSuccess)
	{
		// Stop Update_Search() from being invoked.
		mUpdate_joinStatus = JoinStatus.None;

		if (isSuccess)
		{
			HideLoadingSplash();

			if (mMP_Mode == NetworkType.InternetIP) 
				mLobbyMenu_lobbyCodeWithHeaderText.text = "Lobby IP: " + mLobbyCode;
			else
				mLobbyMenu_lobbyCodeWithHeaderText.text = "Lobby Room Code: " + mLobbyCode;

			SwitchMenu(mLobbyMenu);
			mLobby.JoinLobby_ToggleLobbyClientLogic(true);
		}
		else
		{
			UpdateLoadingSplashMessage("Failed to join lobby.");
			RevealCloseLoadingSplashButton();
		}
	}







	/* 5 - Lobby Menu */

	/// <summary>
	/// Get the GUI lobby slot that corresponds to a specific team role (e.g. purple runner).
	/// </summary>
	protected LobbySlot GetLobbySlot(TeamRole teamRole)
	{
		foreach (LobbySlot slot in mLobbySlots)
			if (slot.teamRole == teamRole)
				return slot;

		return null;
	}

	/// <summary>
	/// Get the GUI lobby slot that I occupy.
	/// </summary>
	protected LobbySlot GetMyLobbySlot()
	{
		foreach (LobbySlot slot in mLobbySlots)
			if (slot.isYou)
				return slot;

		return null;
	}

	/// <summary>
	/// Occupy (or refresh) a GUI lobby slot.
	/// </summary>
	public bool OccupyLobbySlot(TeamRole teamRole, string username, bool isHost, bool isYou)//, bool isReady)
	{
		LobbySlot slot = GetLobbySlot(teamRole);
		if (slot.isOccupied)
			return false;

		slot.isYou = isYou;
		slot.isHost = isHost;
		slot.isOccupied = true;
		slot.username = username;
//		slot.isReadyToggle.isOn = isReady;

		slot.playerInfoContainer.gameObject.SetActive(true);
		slot.joinButton.gameObject.SetActive(false);
		slot.usernameText.gameObject.SetActive(true);
		slot.leaveRoleButton.gameObject.SetActive(isYou);

		slot.usernameText.text = username;

		if (slot.isHost)
		{
			slot.usernameText.text += " (Host)";
//			slot.isReadyToggle.gameObject.SetActive(false);
		}
		else
		{
//			slot.isReadyToggle.gameObject.SetActive(true);

//			if (slot.isReadyToggle.isOn)
//				slot.isReadyToggle.GetComponentInChildren<Text>().text = "Ready";
//			else 
//				slot.isReadyToggle.GetComponentInChildren<Text>().text = "Not ready";
//
//			slot.isReadyToggle.interactable = isYou;
		}

		return true;
	}

	/// <summary>
	/// Unoccupy a GUI lobby slot that corresponds to a specific team role (e.g. purple runner).
	/// </summary>
	public bool UnoccupyLobbySlot(TeamRole teamRole)
	{
		LobbySlot slot = GetLobbySlot(teamRole);
		if (!slot.isOccupied)
			return false;

		slot.isYou = false;
		slot.isHost = false;
		slot.isOccupied = false;
		slot.username = "";

		slot.playerInfoContainer.gameObject.SetActive(false);
		slot.joinButton.gameObject.SetActive(true);
		slot.usernameText.gameObject.SetActive(false);
//		slot.isReadyToggle.gameObject.SetActive(false);
//		slot.isReadyToggle.GetComponentInChildren<Text>().text = "Not ready";
		slot.leaveRoleButton.gameObject.SetActive(false);

		return true;
	}

	/// <summary>
	/// Unoccupy all four GUI lobby slots.
	/// </summary>
	public void UnoccupyLobbySlotAll()
	{
		foreach (LobbySlot slot in mLobbySlots)
			if (slot.isOccupied)
				UnoccupyLobbySlot(slot.teamRole);
	}

	/// <summary>
	/// Copy the current state of my GUI lobby slots into a sendable packet.
	/// </summary>
	public LobbyStatePacket CopyLobbyStateToPacket()
	{
		LobbyStatePacket pkt = new LobbyStatePacket(true);
		pkt.NumActivePlayers = mLobby.TrueLobbyState.NumActivePlayers;

		for (int i = 0; i < 4; i++)
		{
			LobbySlot slot = mLobbySlots[i];
			pkt.IsOccupieds[i] = slot.isOccupied;
			pkt.IsHosts[i] = slot.isHost;
//			pkt.IsReadys[i] = slot.isReadyToggle.isOn;
			pkt.PlayerIds[i] = mLobby.TrueLobbyState.PlayerIds[i];
			pkt.Usernames[i] = slot.username;
		}

		return pkt;
	}

	/// <summary>
	/// Event when the "Join Role" is pressed for a particular GUI lobby slot.
	/// </summary>
	public void OnClick_JoinRole(string teamRole)
	{
		TeamRole dTeamRole = (TeamRole) Enum.Parse(typeof(TeamRole), teamRole);

		LobbySlot myCurSlot = GetMyLobbySlot();
		if (myCurSlot != null)
			UnoccupyLobbySlot(myCurSlot.teamRole);

		// As host, let everyone know about my change.
		if (mIsHost)
		{
			OccupyLobbySlot(dTeamRole, mUsername, mIsHost, true);
			LobbyStatePacket lobbyState = CopyLobbyStateToPacket();
			mLobby.SendNewTrueLobbyState(lobbyState);
		}
		// As client, inform server.
		else 
		{
			LobbyUpdateFromClientPacket sendPkt = new LobbyUpdateFromClientPacket();
//			sendPkt.IsReady = false;
			sendPkt.IsOccupied = true;
			sendPkt.TeamRole = dTeamRole;
			sendPkt.Username = mUsername;
			mLobby.SendLobbyUpdateFromClient(sendPkt);
		}
	}

	/// <summary>
	/// Event when the "Is Ready" toggle is pressed for a particular GUI lobby slot.
	/// </summary>
	public void OnToggle_Ready(bool unused)
	{
		LobbySlot slot = GetMyLobbySlot();

		// Update GUI.
//		if (slot.isReadyToggle.isOn)
//			slot.isReadyToggle.GetComponentInChildren<Text>().text = "Ready";
//		else 
//			slot.isReadyToggle.GetComponentInChildren<Text>().text = "Not ready";

		// Inform server know.
		LobbyUpdateFromClientPacket sendPkt = new LobbyUpdateFromClientPacket();
//		sendPkt.IsReady = slot.isReadyToggle.isOn;
		sendPkt.IsOccupied = true;
		sendPkt.TeamRole = slot.teamRole;
		sendPkt.Username = mUsername;
		mLobby.SendLobbyUpdateFromClient(sendPkt);
	}

	/// <summary>
	/// Event when the "Leave Role" button is pressed for a particular GUI lobby slot.
	/// </summary>
	public void OnClick_LeaveRole(string teamRole)
	{
		TeamRole dTeamRole = (TeamRole) Enum.Parse(typeof(TeamRole), teamRole);

		// Update GUI.
		UnoccupyLobbySlot(dTeamRole);

		if (mIsHost)	// Inform clients.
		{
			LobbyStatePacket lobbyState = CopyLobbyStateToPacket();
			mLobby.SendNewTrueLobbyState(lobbyState);
		}
		else 	// Inform server.
		{
			LobbyUpdateFromClientPacket sendPkt = new LobbyUpdateFromClientPacket();
//			sendPkt.IsReady = false;
			sendPkt.IsOccupied = false;
			sendPkt.TeamRole = dTeamRole;
			sendPkt.Username = mUsername;
			mLobby.SendLobbyUpdateFromClient(sendPkt);
		}
	}




	/// <summary>
	/// As the host of the lobby, determine if I can press the "Start Game" button.
	/// "Start Game" button will be enabled if so.
	/// </summary>
	protected void Update_HostDetermineStartGameOK()
	{
		if (mIsHost && mCurMenu == mLobbyMenu)
		{
			int numOccupiedSlots = 0;
			foreach (LobbySlot slot in mLobbySlots)
				if (slot.isOccupied)
					numOccupiedSlots++;

			mLobbyMenu_startGameBut.interactable = (numOccupiedSlots == mLobby.TrueLobbyState.NumActivePlayers);
		}
	}


	/// <summary>
	/// Event when the "Start Game" button is pressed by the server. This will be greyed out on client side, however, 
	/// it will be invoked by Lobby.cs on the client side when it receives the "LobbyStartGamePacket" from the 
	/// server.
	/// </summary>
	public void OnClick_StartGame()
	{
		LobbySlot mySlot = GetMyLobbySlot();

		if ( ! mIsHost && mySlot == null )
		{
			OnClick_Disconnect("Server has already started the game before you could pick your team and role.");
			return;
		}

		// BUG FIX. This must be done before switching menu in order for GUI lobby slots to be queried.
		if (mIsHost)
		{
			// Alert clients that game starts.

			LobbyStatePacket sendPkt = CopyLobbyStateToPacket();
			sendPkt.IsStartGame = true;

			mLobby.SendNewTrueLobbyState(sendPkt);
		}

		// Switch to in-game mode.

		mEngine.Run( mySlot.teamRole, mLobby.TrueLobbyState );

		SwitchMenu(mInGameMenu);
	}



	/* 6 - In-Game Menu */

	public void OnClick_Disconnect(string reasonMsg="")
	{
		OnClick_Disconnect(false, reasonMsg);
	}

	public void OnClick_Disconnect(bool isHostTimeout, string reasonMsg)
	{
		mEngine.Stop();
		UnoccupyLobbySlotAll();

		SwitchMenu(mStartMenu);

		ShowLoadingSplash(reasonMsg + " Disconnecting...");

		// Start timer.
		mUpdate_nextTime = Time.time + mUpdate_intervalTime;
		mUpdate_endTime = Time.time + mUpdate_maxDuration;

		mLobby.Disconnect(mIsHost, mMP_Mode, isHostTimeout);
		mUpdate_disconnectStatus = DisconnectStatus.Disconnected;
	}

	protected void Update_Disconnect()
	{
		if (mUpdate_disconnectStatus == DisconnectStatus.None)
			return;
	
		if (Time.time > mUpdate_endTime)
		{
			Debug.Log("Update_Disconnect(): Disconnect timeout.");
			Disconnect_Post(false);
		}
		else if (Time.time > mUpdate_nextTime)
		{
			if (mUpdate_disconnectStatus == DisconnectStatus.Disconnected)
			{
				Debug.Log("Update_Disconnect(): Checking disconnect result: " + mLobby.DisconnectResult());

				string result = mLobby.DisconnectResult();
				if (result == null)
					; // Try again later.
				else if (result == "failure")
					Disconnect_Post(false);	     // Report failure.
				else 
					Disconnect_Post(true);		 // Report success.
			}

			mUpdate_nextTime = Time.time + mUpdate_intervalTime;
		}
	}

	protected void Disconnect_Post(bool isSuccess)
	{
		mUpdate_disconnectStatus = DisconnectStatus.None;

		if (isSuccess)
		{
			HideLoadingSplash();
		}
		else 
		{
			UpdateLoadingSplashMessage("Disconnect failure.");
			RevealCloseLoadingSplashButton();
		}
	}



	/// <summary>
	/// Show or hide the in-game menu.
	/// </summary>
	public void OnClick_InGameMenuToggle()
	{
		mInGameMenu_menuBut.gameObject.SetActive( ! mInGameMenu_menuBut.gameObject.activeSelf );
		mInGameMenu_resumeBut.gameObject.SetActive( ! mInGameMenu_resumeBut.gameObject.activeSelf );
		mInGameMenu_disconnectBut.gameObject.SetActive( ! mInGameMenu_disconnectBut.gameObject.activeSelf );

		// Allow in-game player controllers if button to make in-game menu appear is shown.
		mEngine.SetActiveControllers( mInGameMenu_menuBut.gameObject.activeSelf );
	}




	/* Loading Splash */

	/// <summary>
	/// Shows the loading splash. Menu will be disable until loading screen is removed.
	/// </summary>
	/// 
	/// <param name="message">
	/// Message to display on the loading splash for the end-user to see.
	/// </param>
	public void ShowLoadingSplash(string message)
	{
		foreach (CanvasRenderer rend in mCurMenu.GetComponentsInChildren<CanvasRenderer>())
			//rend.SetAlpha(0.2f);
		foreach (Button but in mCurMenu.GetComponentsInChildren<Button>())
			but.interactable = false;
		foreach (InputField field in mCurMenu.GetComponentsInChildren<InputField>())
			field.interactable = false;
		foreach (LobbySlot slot in mLobbySlots)
		{
//			if (slot.isYou)
//				slot.isReadyToggle.interactable = false;
		}

		mIsLoadingSplashShown = true;
		mLoadingSplashPanel.SetActive(true);
		mLoadingSplashText.text = message;
	}

	/// <summary>
	/// Updates the loading splash message.
	/// </summary>
	public void UpdateLoadingSplashMessage(string message)
	{
		mLoadingSplashText.text = message;
	}

	/// <summary>
	/// Is the loading splash screen being displayed right now?
	/// </summary>
	public bool IsLoadingSplash()
	{
		return (mLoadingSplashPanel.activeSelf);
	}

	/// <summary>
	/// Hides the loading splash. Menu will be re-enabled for interaction.
	/// </summary>
	public void HideLoadingSplash()
	{
		foreach (CanvasRenderer rend in mCurMenu.GetComponentsInChildren<CanvasRenderer>())
			rend.SetAlpha(1.0f);
		foreach (Button but in mCurMenu.GetComponentsInChildren<Button>())
			but.interactable = true;
		foreach (InputField field in mCurMenu.GetComponentsInChildren<InputField>())
			field.interactable = true;
		foreach (LobbySlot slot in mLobbySlots)
		{
//			if (slot.isYou)
//				slot.isReadyToggle.interactable = true;
		}
		OnInputFieldChange_Username();		// "Next" Button may still need to be disabled.

		mIsLoadingSplashShown = false;
		mLoadingSplashCloseButton.gameObject.SetActive(false);
		mLoadingSplashPanel.SetActive(false);
	}

	/// <summary>
	/// Reveal the button to close the loading splash. When clicked by the user, HideLoadingSplash() is called.
	/// </summary>
	public void RevealCloseLoadingSplashButton()
	{
		mLoadingSplashCloseButton.gameObject.SetActive(true);
	}




	/* Functions invoked by the external lobby logic class (Lobby.cs) */

	/// <summary>
	/// Refresh the GUI lobby slots using the given true lobby state.
	/// </summary>
	public void RefreshLobby(LobbyStatePacket lobbyState, PlayerId myId)
	{
		RefreshNumActivePlayers(lobbyState.NumActivePlayers);

		for (int i = 0; i < 4; i++)
		{
			if (lobbyState.IsOccupieds[i] == false)
			{
				UnoccupyLobbySlot((TeamRole)i);
			}
			else 
			{
				string username = lobbyState.Usernames[i];
				bool isHost = lobbyState.IsHosts[i];
				bool isYou = (lobbyState.PlayerIds[i] == myId);
//				bool isReady = lobbyState.IsReadys[i];

				OccupyLobbySlot((TeamRole)i, username, isHost, isYou);//, isReady); 
			}
		}

		Debug.Log("Lobby refreshed.");
		mIsLobbyRefreshed = true;

		// If loading screen because waiting for lobby to refresh, we can hide it now.
		if (IsLoadingSplash())
			HideLoadingSplash();
	}


	/// <summary>
	/// Refreshes GUI text that displays # of players.
	/// </summary>
	/// <param name="numActivePlayers">Number active players.</param>
	public void RefreshNumActivePlayers(int numActivePlayers)
	{
		mLobbyMenu_playerCountText.text = numActivePlayers + "/4";
	}






	/* Handle game over / scoreboard display */

	/// <summary>
	/// Invoked by GameOver() in GameEngine.cs.
	/// </summary>
	public void ShowScoreboardMenu(ScoreboardStatePacket scorePacket, TeamRole myTeamRole)
	{
		foreach (ScoreboardSlot slot in mScoreboardSlots)
		{
			if (slot.teamRole == TeamRole.PurpleRunner)
			{
				slot.roleUsername.text = "Runner - " + scorePacket.PurpleRunner.Username;
				slot.summary.text = "Flags Captured: " + scorePacket.PurpleRunner.FlagsCaptured + "\n";
				slot.summary.text += "Deaths: " + scorePacket.PurpleRunner.Deaths;
			}
			else if (slot.teamRole == TeamRole.PurpleBuilder)
			{
				slot.roleUsername.text = "Builder - " + scorePacket.PurpleBuilder.Username;
				slot.summary.text = "Blocks Placed: " + scorePacket.PurpleBuilder.BlocksPlaced + "\n";
			}
			else if (slot.teamRole == TeamRole.YellowRunner)
			{
				slot.roleUsername.text = "Runner - " + scorePacket.YellowRunner.Username;
				slot.summary.text = "Flags Captured: " + scorePacket.YellowRunner.FlagsCaptured + "\n";
				slot.summary.text += "Deaths: " + scorePacket.YellowRunner.Deaths;
			}
			else if (slot.teamRole == TeamRole.YellowBuilder)
			{
				slot.roleUsername.text = "Builder - " + scorePacket.YellowBuilder.Username;
				slot.summary.text = "Blocks Placed: " + scorePacket.YellowBuilder.BlocksPlaced + "\n";
			}
		}

		mScoreboardMenu_purpleScoreText.text = scorePacket.PurpleRunner.FlagsCaptured.ToString();
		mScoreboardMenu_yellowScoreText.text = scorePacket.YellowRunner.FlagsCaptured.ToString();

		Team myTeam = Common.GetTeam(myTeamRole);
        
		bool isPurpleWin = (scorePacket.PurpleRunner.FlagsCaptured > scorePacket.YellowRunner.FlagsCaptured);
        bool isYellowWin = (scorePacket.YellowRunner.FlagsCaptured > scorePacket.PurpleRunner.FlagsCaptured);
		bool isTie = (scorePacket.PurpleRunner.FlagsCaptured == scorePacket.YellowRunner.FlagsCaptured);

		if (isTie)
			mScoreboardMenu_youWinLoseText.text = "Draw Game!";
		else if (myTeam == Team.Purple && isPurpleWin || myTeam == Team.Yellow && isYellowWin)
			mScoreboardMenu_youWinLoseText.text = "You Win!";
		else 
			mScoreboardMenu_youWinLoseText.text = "You Lose!";

		mScoreboardMenu_returnToLobbyBut.gameObject.SetActive(mIsHost);

		SwitchMenu(mScoreboardMenu);
	}

	/// <summary>
	/// Event that occurs when server clicks on "Return to Lobby" button on game over scoreboard. Server will
	/// broadcast packet to clients to return to the lobby. Server itself will return to the lobby.
	/// 
	/// From client perspective, this function is invoked when received LobbyReturnPacket.
	/// 
	/// Game engine will be stopped.
	/// </summary>
	public void OnClick_ReturnToLobby()
	{
		Debug.Log("Returning to lobby. Host? " + mIsHost);

		if (mIsHost)
		{
			// Signal clients to return to lobby. Also, send latest lobby state for clients to refresh
			// their lobby. I will refresh my lobby as well.
			mLobby.ServerReturnToLobby();
		}
		else 
		{
			// Loading screen will be removed when RefreshLobby() invoked.
			if ( mIsLobbyRefreshed )
				mIsLobbyRefreshed = false;
			else 
				ShowLoadingSplash("Returning to lobby...");
		}

		mEngine.Stop();
		SwitchMenu(mLobbyMenu);
	}

	/* TutorialPlatformer */

	/* TutorialBuilder */

	/* TutorialGameObjectives */
}
