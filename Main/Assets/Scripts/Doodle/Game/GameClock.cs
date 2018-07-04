using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doodle.Game;
using Doodle.Networking.Packets;

public class GameClock : MonoBehaviour, IGameListener
{
    //Time to begin countdown from in seconds.
    public float timeRemaining;

    //next second value to give players gold on
    public int goldCheck;

    //Interval in seconds to give players gold
    public int goldIncreaseInterval;

    //Bool to trigger host behaviour in Update(); decrement Time and increment gold.
    //Clients will receive commands from host in order to decrease gold etc
    public volatile bool isHost;

    protected bool endLock;

	// Use this for initialization
	void Start () 
	{
        //initially set to 5 minutes, can bedeleted to change in scene
        this.timeRemaining = 240.0f;
		UpdateClock(this.timeRemaining);

        //First interval to give gold on
        this.goldCheck = 60 - goldIncreaseInterval;

        EventManager.Instance.AddGameListener(EventType.FlagCaptured, this);
        this.endLock = true;
    }

	// Update is called once per frame
	void Update () {
        if(timeRemaining <= 0 && isHost && endLock)
        {
            EventManager.Instance.PostNotification(EventType.TimeUp, this, null);
            Debug.Log("Time Up!");
            endLock = false;
        }
        //Update time
        timeRemaining -= Time.deltaTime;
        UpdateClock(timeRemaining);
    }

    public void setTimeRemaining(float newTime)
    {
        timeRemaining = newTime;
    }

    public void setIsHost(bool h)
    {
        isHost = h;
        print("setIsHost called.");
    }

    public void UpdateClock(float timeRemaining)
    {
		int seconds = (int)timeRemaining % 60;

        //increase gold and set next time to grant gold at.
        if (seconds == goldCheck)
        {
            if (isHost)
            {
                incrementGold();
                SendClockState("Interval", GameObject.Find("Economy(Clone)").GetComponent<Economy>().captureReward);
            }
            goldCheck -= goldIncreaseInterval;
            //reset minute cycle
            if (goldCheck < 0)
            {
                goldCheck = 60 - goldIncreaseInterval;
            }
        }
    }

    public void SendClockState(string eventSender, int goldSent)
    {
        var economy = GameObject.Find("Economy(Clone)").GetComponent<Economy>();
        var nwMgr = GameObject.Find("Game Manager").GetComponent<NetworkManager>();

        GameClockPacket clockInfo = new GameClockPacket(true);
        clockInfo.currentTime = timeRemaining;
        if (eventSender.Equals("Left"))
        {
            clockInfo.leftGold = economy.captureReward;
            clockInfo.rightGold = 0;
        }
        else if (eventSender.Equals("Right"))
        {
            clockInfo.leftGold = 0;
            clockInfo.rightGold = economy.captureReward;
        }
        else if(eventSender.Equals("Interval")){

            clockInfo.leftGold = economy.goldPerTurn;
            clockInfo.rightGold = economy.goldPerTurn;
        }

        nwMgr.SendPacket("info", clockInfo);
    }

    public void OnEvent(EventType eventType, Component sender, object param = null)
    {
        Team myTeam = ((EventManagerArg)param).ToTeam();

        //if ((int)sender.GetComponent<FlagTrigger>().myTeam == 0)
        //{
        //    //Purple flag cap
        //    Debug.Log("Gold for Left!");
        //    var economy = GameObject.Find("Economy(Clone)").GetComponent<Economy>();
        //    SendClockState("Left", economy.captureReward);
        //}
        //else if ((int)sender.GetComponent<FlagTrigger>().myTeam == 1)
        //{
        //    //Yellow flag captured
        //    Debug.Log("Gold for Right!");
        //    var economy = GameObject.Find("Economy(Clone)").GetComponent<Economy>();
        //    SendClockState("Right", economy.captureReward);
        //}

        if (myTeam == Team.Purple)
        {
            //Purple flag cap
            Debug.Log("Gold for Left!");
            var economy = GameObject.Find("Economy(Clone)").GetComponent<Economy>();
            SendClockState("Left", economy.captureReward);
        }
        else if (myTeam == Team.Yellow)
        {
            //Yellow flag captured
            Debug.Log("Gold for Right!");
            var economy = GameObject.Find("Economy(Clone)").GetComponent<Economy>();
            SendClockState("Right", economy.captureReward);
        }
    }


    //Grant both player their gold
    private void incrementGold()
    {
		var economyGO = GameObject.Find("Economy(Clone)");
		if (economyGO == null)
			return;

		economyGO.GetComponent<Economy>().incrementGold();
    }
}
