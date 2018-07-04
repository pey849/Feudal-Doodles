using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.Game;
using Doodle.Networking;
using Doodle.Networking.Packets;
using Doodle.InputSystem.Handlers;

public class GameEngine : MonoBehaviour, IGameListener
{
    public bool IsDebug = true;

    /// <summary>
    /// Menus interface. Use this to call actions such as displaying the scoreboard or start menu.
    /// </summary>
    public MenusHandler menusHandler;

    /// <summary>
    /// Lobby manager that tracks current players and their roles.
    /// </summary>
    public Lobby lobby;

    /// <summary>
    /// Module to handle sending and receiving messages. The lobby will have already initialized it and will be 
    /// ready for use in this.Run(), this.Update(), and this.Stop().
    /// </summary>
    public NetworkManager nwMgr;

    /// <summary>
    /// Your unique ID. Use this to determine your team and role (e.g. myTeamRole == TeamRole.PurpleRunner).
    /// </summary>
    protected TeamRole myTeamRole;

    /// <summary>
    /// Every thing you need to know about the connected players, including yourself. Use these fields:
    ///     playersInfo.NumActivePlayers		// Number of players in-game, including yourself.
    /// 	playersInfo.usernames[i]			// Username of player i
    /// 	playersInfo.isHosts[i]				// Is player i the server/host?
    /// 	playersInfo.isOccupieds[i]			// Is player i exist?
    /// 
    /// 	These elements are indexed by TeamRole. For example:
    /// 		playersInfo.usernames[(int)myTeamRole] == "Billy"  			  // My username is "Billy"
    /// 		playersInfo.isHosts[(int)TeamRole.PurpleRunner] == true       // The purple runner is the host.
    /// </summary>
    protected LobbyStatePacket playersInfo;

    /// <summary>
    /// Separate game object containing Scoreboard and TimeKeeper script.
    /// </summary>
    public GameObject gameObjectives;

    /// <summary>
    /// GameObject to be put in Canvas that controls economy
    /// </summary>
    public Economy economyPrefab;

    /// <summary>
    /// Handles displaying the time and scores.
    /// </summary>
    public RolelessUI rolelessUI;

    /// <summary>
    /// GameObject to be put in Canvas that displays the builder UI
    /// </summary>
    public GameObject builderUIPrefab;

    /// <summary>
    /// GameObject to be put in Canvas that controls gameclock
    /// </summary>
    public GameObject gameclockPrefab;
    [HideInInspector]
    public GameClock gameclock;

    public Canvas canvas;

    /// <summary>
    /// Tracks player score. Includes IsGameOver boolean.
    /// </summary>
    protected Scoreboard scoreboard;

    public bool isRunning = false;

    public GameObject parallaxCamArt;
    public GameObject parallaxCameraScript;
    public GameObject runner;
    public GameObject builder;
    public GameObject stage;

    public GameObject cannonball;
    private List<GameObject> projectileList;
    private int lastCannonId;

    private GameObject runnerPurple;
    private GameObject runnerYellow;
    private GameObject builderPurple;
    private GameObject builderYellow;

    private Rigidbody2D runnerPurpleRB;
    private Rigidbody2D runnerYellowRB;

    private PlatformerController runnerPurpleController;
    private PlatformerController runnerYellowController;

    private Animator purpleAnim;
    private Animator yellowAnim;

    private Economy econClone;


    /// <summary>
    /// Start the gameplay phase. This will be invoked when:
    /// 	As server: "Start Game" button is pressed in the lobby.
    /// 	As client: Received packet from server to start game.
    /// </summary>
    /// 
    /// <param name="myTeamRole">
    /// Your unique ID. Use this to determine your team and role.
    /// </param>
    /// 
    /// <param name="playerInfo">
    /// Every thing you need to know about the connected players, including yourself. Use these fields:
    ///     playersInfo.NumActivePlayers		// Number of players in-game, including yourself.
    /// 	playersInfo.usernames[i]			// Username of player i
    /// 	playersInfo.isHosts[i]				// Is player i the server/host?
    /// 	playersInfo.isOccupieds[i]			// Is player i exist?
    /// 
    /// 	These elements are indexed by TeamRole. For example:
    /// 		playersInfo.usernames[(int)myTeamRole] == "Billy"  			  // My username is "Billy"
    /// 		playersInfo.isHosts[(int)TeamRole.PurpleRunner] == true       // The purple runner is the host.
    /// </param>
    public void Run(TeamRole myTeamRole, LobbyStatePacket playersInfo)
    {
        this.myTeamRole = myTeamRole;
        this.playersInfo = playersInfo;

        this.scoreboard = gameObjectives.GetComponent<Scoreboard>();
        this.scoreboard.Run(playersInfo, nwMgr, playersInfo.IsHosts[(int)myTeamRole], myTeamRole);

		EventManagerNetworked.Instance.Run(nwMgr, getIsHost(), playersInfo.PlayerIds[(int)myTeamRole]);

        if (IsDebug) Debug.Log("GameEngine :: My team role is: " + myTeamRole);
        if (IsDebug) Debug.Log("GameEngine :: My username is : " + playersInfo.Usernames[(int)myTeamRole]);
        if (IsDebug) Debug.Log("GameEngine :: Am I the host? " + playersInfo.IsHosts[(int)myTeamRole]);

        // TODO. Put your initialization code here. Do not put anything in this.Start().

        this.rolelessUI.ToggleVisibility(true);

        stage.SetActive(true);
        parallaxCamArt.SetActive(true);
        parallaxCameraScript.GetComponent<CameraBehaviour>().ReassignBackgrounds();
        runnerPurple = Instantiate(runner, new Vector3(-3, 4, 6), Quaternion.identity);
        runnerYellow = Instantiate(runner, new Vector3(32, 4, 6), Quaternion.identity);
		runnerPurple.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(0xa3, 0x00, 0xDD, 0xFF); //RGBA
        runnerPurple.transform.GetChild(4).GetComponent<SpriteRenderer>().color = new Color32(0xa3, 0x00, 0xDD, 0xFF); //RGBA
        /*runnerPurple.transform.GetChild(3).GetComponent<SpriteRenderer>().color = new Color32(0xff, 0xcc, 0xff, 0xFF); //RGBA
        runnerPurple.transform.GetChild(4).GetComponent<SpriteRenderer>().color = new Color32(0xff, 0xcc, 0xff, 0xFF); //RGBA
        runnerPurple.transform.GetChild(5).GetComponent<SpriteRenderer>().color = new Color32(0xff, 0xcc, 0xff, 0xFF); //RGBA
        runnerPurple.transform.GetChild(6).GetComponent<SpriteRenderer>().color = new Color32(0xff, 0xcc, 0xff, 0xFF); //RGBA
		*/
		runnerYellow.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(0xFB, 0xFF, 0x00, 0xFF); //RGBA
        runnerYellow.transform.GetChild(4).GetComponent<SpriteRenderer>().color = new Color32(0xFB, 0xFF, 0x00, 0xFF); //RGBA
        /*runnerYellow.transform.GetChild(3).GetComponent<SpriteRenderer>().color = new Color32(0xff, 0xff, 0xcc, 0xFF); //RGBA
        runnerYellow.transform.GetChild(4).GetComponent<SpriteRenderer>().color = new Color32(0xff, 0xff, 0xcc, 0xFF); //RGBA
        runnerYellow.transform.GetChild(5).GetComponent<SpriteRenderer>().color = new Color32(0xff, 0xff, 0xcc, 0xFF); //RGBA
        runnerYellow.transform.GetChild(6).GetComponent<SpriteRenderer>().color = new Color32(0xff, 0xff, 0xcc, 0xFF); //RGBA
		*/
        runnerPurpleRB = runnerPurple.GetComponent<Rigidbody2D>();
        runnerYellowRB = runnerYellow.GetComponent<Rigidbody2D>();
        runnerPurpleController = runnerPurple.GetComponent<PlatformerController>();
        runnerYellowController = runnerYellow.GetComponent<PlatformerController>();
        runnerYellowController.myTeam = Team.Yellow;
        runnerPurpleController.myTeam = Team.Purple;
        purpleAnim = runnerPurple.GetComponent<Animator>();
        yellowAnim = runnerYellow.GetComponent<Animator>();

        Economy economy = (Economy)Instantiate(economyPrefab);
        economy.transform.SetParent(canvas.transform, false);

        GameObject gameclockGO = Instantiate(gameclockPrefab);
        this.gameclock = gameclockGO.GetComponent<GameClock>();
        this.gameclock.setIsHost(playersInfo.IsHosts[(int)myTeamRole]);

        GameObject builderUI = Instantiate(builderUIPrefab);

        RefreshRolelessUI();

        switch (myTeamRole)
        {
            case TeamRole.PurpleBuilder:
                builderPurple = Instantiate(builder, new Vector3(-3, 0, 5.5f), Quaternion.identity);
                runnerPurpleController.enabled = false;
                runnerYellowController.enabled = false;
                runnerPurpleRB.gravityScale = 0;
                runnerYellowRB.gravityScale = 0;
                parallaxCameraScript.GetComponent<CameraBehaviour>().target = builderPurple.transform;
				parallaxCameraScript.GetComponent<CameraBehaviour>().SetupRoles(TeamRole.PurpleBuilder);

                builderPurple.GetComponent<BuilderPlatformer>().myTeam = Team.Purple;
                builderUI.transform.SetParent(canvas.transform, false);
                //normalbtn = Instantiate(normalBlockButton, new Vector3(1, 4, 6), Quaternion.identity);
                //firebtn = Instantiate(fireBlockButton, new Vector3(-1, 4, 6), Quaternion.identity);
                //icebtn = Instantiate(iceBlockButton, new Vector3(-3, 4, 6), Quaternion.identity);
                //         cannonbtn = Instantiate(cannonButton, new Vector3(-5, 4, 6), Quaternion.identity);
                //         smokebtn = Instantiate(smokeButton, new Vector3(-7, 4, 6), Quaternion.identity);
                break;

            case TeamRole.PurpleRunner:
                var gamepadPurple = runnerPurple.AddComponent<GamepadInputHandler>();
                runnerPurple.GetComponent<PlatformerController>().gamepad = gamepadPurple;
                for (int i = 0; i < runnerPurple.transform.childCount; i++)
                {
                    if (runnerPurple.transform.GetChild(i).name == "Explosion")
                    {
                        var explosion = runnerPurple.transform.GetChild(i).gameObject.AddComponent<PlatformerController_BlockExplosion>();
                        explosion.GetComponent<PlatformerController_BlockExplosion>().player = runnerPurple.GetComponent<PlatformerController>();//) ) as PlatformerController_BlockExplosion;
                    }
                }
                runnerYellowRB.gravityScale = 0;
                runnerYellow.GetComponent<PlatformerController>().enabled = false;
                parallaxCameraScript.GetComponent<CameraBehaviour>().target = runnerPurple.transform;
				parallaxCameraScript.GetComponent<CameraBehaviour>().SetupRoles(TeamRole.PurpleRunner);
                break;

            case TeamRole.YellowBuilder:
                builderYellow = Instantiate(builder, new Vector3(32, 0, 5.5f), Quaternion.identity);
                runnerPurpleController.enabled = false;
                runnerYellowController.enabled = false;
                runnerPurpleRB.gravityScale = 0;
                runnerYellowRB.gravityScale = 0;
                parallaxCameraScript.GetComponent<CameraBehaviour>().target = builderYellow.transform;
				parallaxCameraScript.GetComponent<CameraBehaviour>().SetupRoles(TeamRole.YellowBuilder);

                builderYellow.GetComponent<BuilderPlatformer>().myTeam = Team.Yellow;
                builderUI.transform.SetParent(canvas.transform, false);

                //normalbtn = Instantiate(normalBlockButton, new Vector3(30, 4, 6), Quaternion.identity);
                //firebtn = Instantiate(fireBlockButton, new Vector3(32, 4, 6), Quaternion.identity);
                //icebtn = Instantiate(iceBlockButton, new Vector3(34, 4, 6), Quaternion.identity);
                //         cannonbtn = Instantiate(cannonButton, new Vector3(36, 4, 6), Quaternion.identity);
                //         smokebtn = Instantiate(smokeButton, new Vector3(38, 4, 6), Quaternion.identity);
                break;

            case TeamRole.YellowRunner:
                var gamepadYellow = runnerYellow.AddComponent<GamepadInputHandler>();
                runnerYellow.GetComponent<PlatformerController>().gamepad = gamepadYellow;
                for (int i = 0; i < runnerYellow.transform.childCount; i++)
                {
                    if (runnerYellow.transform.GetChild(i).name == "Explosion")
                    {
                        var explosion = runnerYellow.transform.GetChild(i).gameObject.AddComponent<PlatformerController_BlockExplosion>();
                        explosion.GetComponent<PlatformerController_BlockExplosion>().player = runnerYellow.GetComponent<PlatformerController>();//) ) as PlatformerController_BlockExplosion;
                    }
                }
                runnerPurpleRB.gravityScale = 0;
                runnerPurple.GetComponent<PlatformerController>().enabled = false;
                parallaxCameraScript.GetComponent<CameraBehaviour>().target = runnerYellow.transform;
				parallaxCameraScript.GetComponent<CameraBehaviour>().SetupRoles(TeamRole.YellowRunner);
                break;

            default:
                break;
        }

        nwMgr.NetworkHost.Disconnected -= new ConnectionDisconnectedHandler(NwMgrDisconnectEvent);
        nwMgr.NetworkHost.Disconnected += new ConnectionDisconnectedHandler(NwMgrDisconnectEvent);

		/* Play battle music */
		EventManager.Instance.PostNotification(EventType.GameStarted, this, null);

		this.isRunning = true;
    }


    /// <summary>
    /// Event that triggers when network manager detects a disconnection.
    /// </summary>
    /// <param name="conn">Conn.</param>
    private void NwMgrDisconnectEvent(INetworkConnection conn)
    {
        //		Debug.Log("GameEngine.NwMgrDisconnectEvent()");
        //
        //		// If client...
        //		if (isRunning && !playersInfo.IsHosts[(int)myTeamRole])
        //		{
        //			menusHandler.OnClick_Disconnect(true, "Lost connection to host.");
        //			return;
        //		}
    }

    void Start()
    {
        EventManager.Instance.AddGameListener(EventType.PlayerDied, this);
        EventManager.Instance.AddGameListener(EventType.FlagCaptured, this);
        EventManager.Instance.AddGameListener(EventType.FlagGrabbed, this);

		/* Play main menu music */
		EventManager.Instance.PostNotification(EventType.ReturnToMenu, this, null);
    }

    void Update()
    {

        if ( ! this.isRunning )
			return;
        if (econClone == null) {
            if (GameObject.Find("Economy(Clone)") != null)
                econClone = GameObject.Find("Economy(Clone)").GetComponent<Economy>();
            else
                return;

        }
		RefreshRolelessUI();

        ReceiveRunnerStates();
        ReceiveBlockUpdates();
        ReceiveGameStates();
        ReceiveClockStates();
        ReceiveProjtilePacket();
        ReceivePushPacket();
        switch (myTeamRole)
        {
            case TeamRole.PurpleRunner:
                SendRunnerState();
                break;

            case TeamRole.YellowRunner:
                SendRunnerState();
                break;

            default:
                break;
        }

        if (scoreboard.mPacket.IsGameOver)
            GameOver();

        //Ignore. Game over testing.
        if (Input.GetKeyDown(KeyCode.F1) && playersInfo.IsHosts[(int)myTeamRole])
        {
            Debug.Log("F1 key pressed down. Game over testing triggered.");
            scoreboard.mPacket.IsGameOver = true;
            scoreboard.mPacketDirtyBit = true;
            GameOver();
        }
    }



    /// <summary>
    /// This will be invoked when user clicks on the "Disconnect" or "Return to Lobby".
    /// </summary>
    public void Stop()
    {
        if (!this.isRunning)
            return;

        if (IsDebug) Debug.Log("GameEngine.Stop() invoked.");
        this.isRunning = false;

        // Stop the scoreboard from changing.
        this.scoreboard.Stop();

        EventManagerNetworked.Instance.Stop();

        rolelessUI.ToggleVisibility(false);

        // Hide background art.
        this.stage.SetActive(false);
        this.parallaxCamArt.SetActive(false);

        // Camera will track this game object instead of player because player will be destroyed.
        this.parallaxCameraScript.GetComponent<CameraBehaviour>().target = this.transform;

        GameObject economy = GameObject.Find("Economy(Clone)");
        GameObject builderUI = GameObject.Find("BuilderUI(Clone)");
        GameObject gameClock = GameObject.Find("Clock(Clone)");
        if (economy != null)
            Destroy(economy);
        if (builderUI != null)
            Destroy(builderUI);
        if (gameClock != null)
            Destroy(gameClock);


        // Delete players.
        Destroy(runnerPurple);
        Destroy(runnerYellow);
        Destroy(builderPurple);
        Destroy(builderYellow);

        // Delete blocks.
        GameObject.Find("Renderer").GetComponent<Renderer>().RenderRemoveBlocksAll();

        nwMgr.NetworkHost.Disconnected -= new ConnectionDisconnectedHandler(NwMgrDisconnectEvent);

		/* Main menu music */
		EventManager.Instance.PostNotification(EventType.ReturnToMenu, this, null);

        // TODO. Put your code here to shutdown the gameplay phase such as destroying game object blocks and avatars.

        // Do not worry about turning off the NetworkManager. It is already handled by the disconnect button itself.
        // Do not worry about bringing up the main menu. It is already handled by the disconnect button itself.
    }

    protected void RefreshRolelessUI()
    {
        this.rolelessUI.SetScore(Team.Purple, this.scoreboard.mPacket.PurpleRunner.FlagsCaptured);
        this.rolelessUI.SetScore(Team.Yellow, this.scoreboard.mPacket.YellowRunner.FlagsCaptured);

        this.rolelessUI.SetTime(this.gameclock.timeRemaining);
    }

    public void SendGameState()
    {
        print("sending game state");
        GameState state = GameObject.Find("Renderer").gameObject.GetComponent<Renderer>().generateCurrentBlockState();
        if (playersInfo.IsHosts[(int)myTeamRole])
        {
            state.fromHost = true;
        }
        nwMgr.SendPacket("info", state);
    }

    public void sendBlockUpdate(GameState.Block b, bool add)
    {
        if (add)
        {
            if (playersInfo.IsHosts[(int)myTeamRole])
            {
                GameObject.Find("Renderer").GetComponent<Renderer>().RenderAddBlock(b);
                SendGameState();
            }
            else
            {
                BlockUpdatePacket bup = new BlockUpdatePacket(b.position, b.type, add);
                bup.role = b.role;
                nwMgr.SendPacket("info", bup);
                print("im not host, I'm trying to send blockupdate packet");
            }
        }
        else
        {
            if (playersInfo.IsHosts[(int)myTeamRole])
            {
                GameObject.Find("Renderer").GetComponent<Renderer>().RenderRemoveBlock(b);
                SendGameState();
            }
            else
            {
                BlockUpdatePacket bup = new BlockUpdatePacket(b.position, b.type, add);
                bup.role = b.role;
                nwMgr.SendPacket("info", bup);
                print("im not host, I'm trying to send blockupdate packet");
            }
        }

    }

    private void SendRunnerState()
    {
        RunnerState state = new RunnerState(runnerPurple, runnerYellow);
        if (playersInfo.IsHosts[(int)myTeamRole] == true)
        {
            state.fromHost = true;
        }
        if (myTeamRole == TeamRole.PurpleRunner)
        {
            state.fromPurple = true;
        }
        nwMgr.SendPacket("state", state);
    }

    private void ReceiveBlockUpdates()
    {
        while (nwMgr.PacketQueue.HasPacket<BlockUpdatePacket>())
        {
            if (playersInfo.IsHosts[(int)myTeamRole])
            {
                print("im host, and i got your block update packet.");
                INetworkConnection recvConn;
                BlockUpdatePacket packet = nwMgr.PacketQueue.GetNextPacket<BlockUpdatePacket>(out recvConn);

                if (packet.blockWasAdded)
                {
                    GameState.Block b = new GameState.Block();
                    b.position = packet.position;
                    b.type = packet.type;
                    b.role = packet.role;
                    GameObject.Find("Renderer").GetComponent<Renderer>().RenderAddBlock(b);
                    SendGameState();
                    print("i'm host and i just sent a packet back after adding");
                }
                else
                {
                    GameState.Block b = new GameState.Block();
                    b.position = packet.position;
                    b.type = packet.type;
                    b.role = packet.role;
                    GameObject.Find("Renderer").GetComponent<Renderer>().RenderRemoveBlock(b);
                    SendGameState();
                    print("i'm host and i just sent a packet back after removing");
                }
            }
        }

    }

    public void SendProjectileFromHost(GameObject cb)
    {
        print("sendprojfromhost called.");
        //Vector3 bulletSpawnPosition = new Vector3(cb.transform.position.x + 1, cb.transform.position.y, 6f);
        var bullet = Instantiate(cannonball, cb.transform.position, this.transform.rotation);

        if (myTeamRole == TeamRole.PurpleBuilder)
        {
            bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(350, 0f));
        }
        if (myTeamRole == TeamRole.YellowBuilder)
        {
            bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(-350, 0f));
        }
        ProjectilePacket p = new ProjectilePacket(true, bullet);
        p.formHost = true;
        p.instanti = true;
        p.ID = lastCannonId + 1;
        p.role = myTeamRole;
        lastCannonId++;
        nwMgr.SendPacket("info", p);
    }

    private void ReceiveProjtilePacket()
    {
        while (nwMgr.PacketQueue.HasPacket<ProjectilePacket>())
        {
            ProjectilePacket packet = nwMgr.PacketQueue.GetNextPacket<ProjectilePacket>();
            //do something with packets
            if (playersInfo.IsHosts[(int)myTeamRole])
            {
                print("I am host. i have received a request to create projectile.");
                if (packet.instanti)
                {
                    Vector3 bulletSpawnPosition = new Vector3(packet.pos.x, packet.pos.y, 6f);
                    var bullet = Instantiate(cannonball, bulletSpawnPosition, this.transform.rotation);
                    bullet.GetComponent<Rigidbody2D>().velocity = packet.velocity;
                    bullet.GetComponent<Rigidbody2D>().angularVelocity = packet.AnglarVel;
                    bullet.GetComponent<Rigidbody2D>().rotation = packet.rotation;

                    if (packet.role == TeamRole.PurpleBuilder)
                    {
                        bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(350, 0f));
                    }
                    else if (packet.role == TeamRole.YellowBuilder)
                    {
                        bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(-350, 0f));
                    }

                    ProjectilePacket p = new ProjectilePacket(true, bullet);
                    p.formHost = true;
                    p.instanti = true;
                    p.role = packet.role;
                    p.ID = lastCannonId + 1;
                    lastCannonId++;
                    nwMgr.SendPacket("info", p);
                }
            }
            else
            {
                print("i am not host. i am receiving an update to the packets location");
                if (packet.formHost)
                {
                    if (packet.instanti)
                    {
                        print("i am not host. instantiating projectile.");
                        Vector3 bulletSpawnPosition = new Vector3(packet.pos.x, packet.pos.y, 6f);
                        var bullet = Instantiate(cannonball, bulletSpawnPosition, this.transform.rotation);
                        if (packet.role == TeamRole.PurpleBuilder)
                        {
                            bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(350, 0f));
                        }
                        else if (packet.role == TeamRole.YellowBuilder)
                        {
                            bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(-350, 0f));
                        }
                        bullet.GetComponent<CannonBall>().ID = packet.ID;
                    }
                    
                }
            }
        }
    }

    public bool getIsHost()
    {
        return playersInfo.IsHosts[(int)myTeamRole];
    }

    public void SendPushPacket(Vector2 dir)
    {
        pushPacket p = new pushPacket();
        if (myTeamRole == TeamRole.YellowRunner)
        {
            p.fromPurple = false;
        } else
        {
            p.fromPurple = true;
        }
        p.dir = dir;
        nwMgr.SendPacket("info", p);
        print("sending push packet...");
    }

    public void SendProjectilePacket(GameObject bullet, TeamRole role)
    {
        //if they are the host
        if (playersInfo.IsHosts[(int)myTeamRole])
        {
            //get bullet obhect to make packet
            ProjectilePacket p = new ProjectilePacket(false, bullet);
            p.formHost = true;
            p.role = role;
            nwMgr.SendPacket("info", p);
            print("i am host. i am sending a location packet.");
        }
        else
        {
            ProjectilePacket p = new ProjectilePacket(true, bullet);
            p.role = role;
            nwMgr.SendPacket("info", p);
            print("I am not host, sent initial packet for projectile.");
        }
    }

    private void ReceiveRunnerStates()
    {
        while (nwMgr.PacketQueue.HasPacket<RunnerState>())
        {
            RunnerState packet = nwMgr.PacketQueue.GetNextPacket<RunnerState>();
            if (packet.fromHost || (playersInfo.IsHosts[(int)myTeamRole]))
            {
                switch (myTeamRole)
                {
                    case TeamRole.PurpleBuilder:
                        if (playersInfo.IsHosts[(int)myTeamRole] == false)
                        {
                            runnerPurple.transform.position = packet.runnerPurple.position;
                            runnerPurpleRB.velocity = packet.runnerPurple.velocity;
                            runnerPurpleRB.angularVelocity = packet.runnerPurple.AnglarVel;
                            runnerPurpleRB.rotation = packet.runnerPurple.rotation;
                            runnerPurple.transform.localScale = packet.runnerPurple.scale;
                            if (packet.runnerPurple.jump)
                            {
                                purpleAnim.SetTrigger("Jumping");
                            }
                            purpleAnim.SetFloat("Speed", packet.runnerPurple.speed);

                            runnerYellow.transform.position = packet.runnerYellow.position;
                            runnerYellowRB.velocity = packet.runnerYellow.velocity;
                            runnerYellowRB.angularVelocity = packet.runnerYellow.AnglarVel;
                            runnerYellowRB.rotation = packet.runnerYellow.rotation;
                            runnerYellow.transform.localScale = packet.runnerYellow.scale;
                            if (packet.runnerYellow.jump)
                            {
                                yellowAnim.SetTrigger("Jumping");
                            }
                            yellowAnim.SetFloat("Speed", packet.runnerYellow.speed);
                        }
                        else
                        {
                            if (packet.fromPurple)
                            {
                                runnerPurple.transform.position = packet.runnerPurple.position;
                                runnerPurpleRB.velocity = packet.runnerPurple.velocity;
                                runnerPurpleRB.angularVelocity = packet.runnerPurple.AnglarVel;
                                runnerPurpleRB.rotation = packet.runnerPurple.rotation;
                                runnerPurple.transform.localScale = packet.runnerPurple.scale;
                                if (packet.runnerPurple.jump)
                                {
                                    purpleAnim.SetTrigger("Jumping");
                                }
                                purpleAnim.SetFloat("Speed", packet.runnerPurple.speed);
                            }
                            if (packet.fromPurple == false || (playersInfo.IsHosts[(int)myTeamRole] == false))
                            {
                                runnerYellow.transform.position = packet.runnerYellow.position;
                                runnerYellowRB.velocity = packet.runnerYellow.velocity;
                                runnerYellowRB.angularVelocity = packet.runnerYellow.AnglarVel;
                                runnerYellowRB.rotation = packet.runnerYellow.rotation;
                                runnerYellow.transform.localScale = packet.runnerYellow.scale;
                                if (packet.runnerYellow.jump)
                                {
                                    yellowAnim.SetTrigger("Jumping");
                                }
                                yellowAnim.SetFloat("Speed", packet.runnerYellow.speed);
                            }
                        }
                        break;

                    case TeamRole.PurpleRunner:
                        runnerYellow.transform.position = packet.runnerYellow.position;
                        runnerYellowRB.velocity = packet.runnerYellow.velocity;
                        runnerYellowRB.angularVelocity = packet.runnerYellow.AnglarVel;
                        runnerYellowRB.rotation = packet.runnerYellow.rotation;
                        runnerYellow.transform.localScale = packet.runnerYellow.scale;

                        if (packet.runnerYellow.jump)
                        {
                            yellowAnim.SetTrigger("Jumping");
                        }
                        yellowAnim.SetFloat("Speed", packet.runnerYellow.speed);
                        break;

                    case TeamRole.YellowBuilder:
                        if (playersInfo.IsHosts[(int)myTeamRole] == false)
                        {
                            runnerPurple.transform.position = packet.runnerPurple.position;
                            runnerPurpleRB.velocity = packet.runnerPurple.velocity;
                            runnerPurpleRB.angularVelocity = packet.runnerPurple.AnglarVel;
                            runnerPurpleRB.rotation = packet.runnerPurple.rotation;
                            runnerPurple.transform.localScale = packet.runnerPurple.scale;
                            if (packet.runnerPurple.jump)
                            {
                                purpleAnim.SetTrigger("Jumping");
                            }
                            purpleAnim.SetFloat("Speed", packet.runnerPurple.speed);

                            runnerYellow.transform.position = packet.runnerYellow.position;
                            runnerYellowRB.velocity = packet.runnerYellow.velocity;
                            runnerYellowRB.angularVelocity = packet.runnerYellow.AnglarVel;
                            runnerYellowRB.rotation = packet.runnerYellow.rotation;
                            runnerYellow.transform.localScale = packet.runnerYellow.scale;
                            if (packet.runnerYellow.jump)
                            {
                                yellowAnim.SetTrigger("Jumping");
                            }
                            yellowAnim.SetFloat("Speed", packet.runnerYellow.speed);
                        }
                        else
                        {
                            if (packet.fromPurple)
                            {
                                runnerPurple.transform.position = packet.runnerPurple.position;
                                runnerPurpleRB.velocity = packet.runnerPurple.velocity;
                                runnerPurpleRB.angularVelocity = packet.runnerPurple.AnglarVel;
                                runnerPurpleRB.rotation = packet.runnerPurple.rotation;
                                runnerPurple.transform.localScale = packet.runnerPurple.scale;
                                if (packet.runnerPurple.jump)
                                {
                                    purpleAnim.SetTrigger("Jumping");
                                }
                                purpleAnim.SetFloat("Speed", packet.runnerPurple.speed);
                            }
                            if (packet.fromPurple == false || (playersInfo.IsHosts[(int)myTeamRole] == false))
                            {
                                runnerYellow.transform.position = packet.runnerYellow.position;
                                runnerYellowRB.velocity = packet.runnerYellow.velocity;
                                runnerYellowRB.angularVelocity = packet.runnerYellow.AnglarVel;
                                runnerYellowRB.rotation = packet.runnerYellow.rotation;
                                runnerYellow.transform.localScale = packet.runnerYellow.scale;
                                if (packet.runnerYellow.jump)
                                {
                                    yellowAnim.SetTrigger("Jumping");
                                }
                                yellowAnim.SetFloat("Speed", packet.runnerYellow.speed);
                            }
                        }
                        break;

                    case TeamRole.YellowRunner:
                        runnerPurple.transform.position = packet.runnerPurple.position;
                        runnerPurpleRB.velocity = packet.runnerPurple.velocity;
                        runnerPurpleRB.angularVelocity = packet.runnerPurple.AnglarVel;
                        runnerPurpleRB.rotation = packet.runnerPurple.rotation;
                        runnerPurple.transform.localScale = packet.runnerPurple.scale;
                        if (packet.runnerPurple.jump)
                        {
                            purpleAnim.SetTrigger("Jumping");
                        }
                        purpleAnim.SetFloat("Speed", packet.runnerPurple.speed);
                        break;
                }
                if (playersInfo.IsHosts[(int)myTeamRole] == true)
                {
                    SendRunnerState();
                }
            }
        }
    }

    private void ReceiveGameStates()
    {
        while (nwMgr.PacketQueue.HasPacket<GameState>())
        {
            GameState packet = nwMgr.PacketQueue.GetNextPacket<GameState>();
            if (packet.fromHost)
            {
                GameObject.Find("Renderer").GetComponent<Renderer>().compareStatesBlocks(packet);
            }
        }
    }

    private void ReceivePushPacket()
    {
        if (myTeamRole == TeamRole.YellowRunner || myTeamRole == TeamRole.PurpleRunner)
        {
            while (nwMgr.PacketQueue.HasPacket<pushPacket>())
            {
                print("got push packet!");
                pushPacket packet = nwMgr.PacketQueue.GetNextPacket<pushPacket>();
                if (packet.fromPurple)
                {
                    print("push packet from purple!");
                    //runnerYellow.GetComponent<Rigidbody2D>().AddForce(packet.dir, ForceMode2D.Force);
                    runnerPurple.GetComponent<Rigidbody2D>().drag = 0;
                    runnerYellow.GetComponent<Rigidbody2D>().velocity = packet.dir;
                }
                else
                {
                    print("push packet from yellow!");
                    //runnerPurple.GetComponent<Rigidbody2D>().AddForce(packet.dir, ForceMode2D.Force);
                    runnerPurple.GetComponent<Rigidbody2D>().drag = 0;
                    runnerPurple.GetComponent<Rigidbody2D>().velocity = packet.dir;
                }
            }
        }
    }

    private void ReceiveClockStates()
    {
        while (nwMgr.PacketQueue.HasPacket<GameClockPacket>())
        {
            print("receiving clock state.");
            GameClockPacket packet = nwMgr.PacketQueue.GetNextPacket<GameClockPacket>();
            econClone.incrementLeft(packet.leftGold);
            econClone.incrementRight(packet.rightGold);
            GameObject.Find("Clock(Clone)").GetComponent<GameClock>().setTimeRemaining(packet.currentTime);
        }
    }


    /// <summary>
    /// Turn the current player controls on or off.
    /// </summary>
    public void SetActiveControllers(bool isOn)
    {
        switch (this.myTeamRole)
        {
            case TeamRole.PurpleRunner:
                this.runnerPurpleController.enabled = isOn;
                break;
            case TeamRole.YellowRunner:
                this.runnerYellowController.enabled = isOn;
                break;
        }
    }

    public TeamRole getTeamRole()
    {
        return this.myTeamRole;
    }

	public Team GetTeam()
	{
		return Common.GetTeam(this.myTeamRole);
	}

	public Role GetRole()
	{
		return Common.GetRole(this.myTeamRole);
	}

    /* Scoreboard and game over handling */


    /// <summary>
    /// Display scoreboard. Server will have a button displayed for everyone to return to the lobby.
    /// </summary>
    private void GameOver()
    {
        SetActiveControllers(false);
        Debug.Log("GameOver");

        this.rolelessUI.ToggleVisibility(false);

        GameObject economy = GameObject.Find("Economy(Clone)");
        GameObject builderUI = GameObject.Find("BuilderUI(Clone)");
        GameObject gameClock = GameObject.Find("Clock(Clone)");
        if (economy != null)
            Destroy(economy);
        if (builderUI != null)
            Destroy(builderUI);
        if (gameClock != null)
            Destroy(gameClock);

        menusHandler.ShowScoreboardMenu(scoreboard.mPacket, myTeamRole);
    }

    public void OnEvent(EventType eventType, Component sender, object arg = null)
    {
        Team teamArg = ((EventManagerArg)arg).ToTeam();
        switch (eventType)
        {   
            case EventType.PlayerDied:
                if (teamArg == Team.Purple)
                    runnerPurple.GetComponent<PlatformerController>().SetHasFlag(false);
                else
                    runnerYellow.GetComponent<PlatformerController>().SetHasFlag(false);
                break;
            case EventType.FlagCaptured:
                if (teamArg == Team.Purple)
                    runnerPurple.GetComponent<PlatformerController>().SetHasFlag(false);
                else
                    runnerYellow.GetComponent<PlatformerController>().SetHasFlag(false);
                break;
            case EventType.FlagGrabbed:
                if (teamArg == Team.Purple)
                {
                    runnerPurple.GetComponent<PlatformerController>().SetHasFlag(true);
                }
                else
                {
                    runnerYellow.GetComponent<PlatformerController>().SetHasFlag(true);
                }
                break;
        }
    }
}
