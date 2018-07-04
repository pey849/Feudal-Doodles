using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doodle.Game;
using Doodle.Networking.Packets;

public class EventManagerNetworked : MonoBehaviour
{
	public bool mIsDebug = true;

	/// <summary>
	/// Is it available to send and receive event packets? 
	/// </summary>
	protected bool mIsRunning = false;

	/// <summary>
	/// Shared network manager from the game engine to send and receive packets. 
	/// </summary>
	protected NetworkManager mNwMgr = null;

	/// <summary>
	/// Am I the host?
	/// </summary>
	protected bool mIsHost = false;

	protected PlayerId mPlayerId = PlayerId.None;

	/// <summary>
	/// This script can be access globally using EventManagerNetworked.instance
	/// </summary>
	private static EventManagerNetworked instance = null;
	public static EventManagerNetworked Instance
	{
		get{return instance;}
		set{}
	}
		
	void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	/// <summary>
	/// Turn on the event manager. Only GameEngine.Start() should call this.
	/// </summary>
	/// <param name="nwMgr">Same network manager from the game engine.</param>
	/// <param name="isHost">Am I the host?</param>
	public void Run(NetworkManager nwMgr, bool isHost, PlayerId playerId)
	{
		mIsRunning = true;
		mNwMgr = nwMgr;
		mIsHost = isHost;
		mPlayerId = playerId;
	}

	void Update () 
	{
		if ( ! mIsRunning )
			return;

		ForwardBroadcastedEventPacketsAsHost();
		ReceiveEventPackets();
	}

	/// <summary>
	/// As host, forward any received broadcasted client event packet to every client including myself such that 
	/// EventManager.PostNotification() is triggered on all players.
	/// </summary>
	protected void ForwardBroadcastedEventPacketsAsHost()
	{
		if ( ! mIsHost )
			return;

		while (mNwMgr.PacketQueue.HasPacket<ClientToHostBroadcastEventPacket>())
		{
			ClientToHostBroadcastEventPacket recvPkt = 
				mNwMgr.PacketQueue.GetNextPacket<ClientToHostBroadcastEventPacket>();

			Debug.LogFormat("EventManagerNetworked :: Received client packet: {0},{1}.",recvPkt.eventType, recvPkt.arg);

			/* Broadcast same packet to every client, excluding myself */
			mNwMgr.SendPacket("info", new HostToClientsBroadcastEventPacket(recvPkt.senderPlayerId, recvPkt.eventType, recvPkt.arg));

			/* For myself, I can interpret the packet already */
			EventManager.Instance.PostNotification(recvPkt.eventType, null, new EventManagerArg(recvPkt.arg));
		}
	}

	/// <summary>
	/// Receive a broadcast client event packet from the host. Each received packet will be trigger a 
	/// EventManager.Instance.PostNotification().
	/// </summary>
	protected void ReceiveEventPackets()
	{
		while (mNwMgr.PacketQueue.HasPacket<HostToClientsBroadcastEventPacket>())
		{
			HostToClientsBroadcastEventPacket recvPkt = 
				mNwMgr.PacketQueue.GetNextPacket<HostToClientsBroadcastEventPacket>();
			Debug.LogFormat("EventManagerNetworked :: Received host packet: {0},{1}.",recvPkt.eventType,recvPkt.arg);

			/* Ignore packet if I was the one that sent it - I would have already triggered event on my local machine. */
			if (recvPkt.senderPlayerId != mPlayerId)
				EventManager.Instance.PostNotification(recvPkt.eventType, null, new EventManagerArg(recvPkt.arg));
		}
	}

	/// <summary>
	/// Turn off the event manager. Only GameEngine.Stop() should call this. 
	/// </summary>
	public void Stop()
	{
		mIsRunning = false;
		mNwMgr = null;
	}
		
	/// <summary>
	/// Trigger an event to every player including myself. 
	/// </summary>
	/// 
	/// <param name="eventType">Event type to broadcast.</param>
	/// <param name="arg">Optional argument to append to the event.</param>
	public void BroadcastEvent(EventType eventType, EventManagerArg arg)
	{
		Debug.LogFormat("EventManagerNetworked.BroadcastEvent({0},{1}) called.",eventType,arg.ToString());

		if ( mIsHost )
			mNwMgr.SendPacket("info", new HostToClientsBroadcastEventPacket(mPlayerId, eventType,arg.ToString()));
		else 
			mNwMgr.SendPacket("info", new ClientToHostBroadcastEventPacket(mPlayerId, eventType,arg.ToString()));

		EventManager.Instance.PostNotification(eventType, null, arg);
	}
}
