using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerLookupWifiDirect : MonoBehaviour,IServerLookup
{
	public bool mIsDebug = true;
	public string mWifiDirectServiceName = "_feudalDoodles";
	public string mWifiDirectAdvertisedPort = "5000";

	protected WifiP2P mWifiP2P;

	// Timeout for each function (e.g. advertiseServer()).
	protected float mFuncTimeoutSec = 20.0f;
	// Time to wait after sub-function call.
	protected float mSubCallCoolDown = 1.0f;
	// Time to wait before re-try after a sub-function returns a non-success.
	protected float mSubErrCoolDown = 3.0f;

	// Interface functions are executed using Update(). Update() will do the work when at least one is set to true.
	protected bool mAdvertiseServerTriggered = false;
	protected bool mGetServerIPTriggered = false;
	protected bool mCancelTriggered = false;

	// AdvertiseServer() progress tracking.
	protected enum AdvertiseServerSubStatus {Creating, Created, Advertising, Advertised};
	protected AdvertiseServerSubStatus mAdvertiseServerSubStatus = AdvertiseServerSubStatus.Creating;
	public float mAdvertiseCoolDown = 15.0f;
	protected bool mAdvertiseServerMaintainTriggered = false;

	// GetServerIP() progress tracking.
	protected enum GetServerIPSubStatus {Discovering, Discovered, DiscoveryFetching, Joining, Joined}
	protected GetServerIPSubStatus mGetServerIPSubStatus = GetServerIPSubStatus.Discovering;
	// Timeout specifically for GetServerIP(). This overrides mFuncTimeoutSec.
	public float mGetServerIPTimeoutSec = 50.0f;
	// Time to wait after an attempt to join a server.
	public float mSubJoinCallCoolDown = 10.0f;
	// Current and maximum attempts to fetch discovered networks before a server discovery re-try.
	protected int mGetDiscoveredNetworksAttempts = 0;
	public int mMaxGetDiscoveredNetworksAttempts = 3;

	// Cancel() progress tracking.
	protected enum CancelSubStatus {Unjoining, Unjoined, MemoryRemoving, Disconnecting, Disconnected, AdvertiseStopping, 
								    AdvertiseStopped};
	protected CancelSubStatus mCancelSubStatus = CancelSubStatus.Unjoining;

	// Used during GetServerIP().
	protected NetworkP2PInfo mDiscoveredNetwork = null;
	protected string mRoomCode = null;

	// Time when sub-function is executed.
	protected float mNextSubCall;

	// Timeout for function.
	protected float mEndTime;

	protected string mResult = null;




	void Start()
	{
		mWifiP2P = new WifiP2P();
	}








	public void AdvertiseServer()
	{
		mResult = null;

		mRoomCode = GenerateRoomCode();

		mNextSubCall = Time.time;
		mEndTime = Time.time + mFuncTimeoutSec;

		mAdvertiseServerSubStatus = AdvertiseServerSubStatus.Creating;

		// Tell Update() to do the work.
		mAdvertiseServerTriggered = true;
	}
	public string AdvertiseServerResult()
	{
		return mResult;
	}




	public void GetServerIP(string roomCode)
	{
		// If host is wanting its own local IP address
		if (roomCode == null)
		{
			mResult = mWifiP2P.GetNetworkOwnerIP();
			return;
		}

		mResult = null;

		mRoomCode = roomCode;

		mNextSubCall = Time.time;
		mEndTime = Time.time + mGetServerIPTimeoutSec;

		mGetServerIPSubStatus = GetServerIPSubStatus.Discovering;

		mGetDiscoveredNetworksAttempts = 0;
		mDiscoveredNetwork = null;

		// Tell Update() to do the work.
		mGetServerIPTriggered = true;
	}
	public string GetServerIPResult()
	{
		return mResult;
	}




	public void Cancel()
	{
		Debug.Log("ServerLookupWifiDirect: Cancel() called.");

		mResult = null;

		mNextSubCall = Time.time;
		mEndTime = Time.time + mFuncTimeoutSec;

		mCancelSubStatus = CancelSubStatus.Unjoining;
		mAdvertiseServerMaintainTriggered = false;

		// Tell Update() to do the work.
		mCancelTriggered = true;
	}
	public string CancelResult()
	{
		return mResult;
	}






	void Update()
	{
		AdvertiseServer_Update();
		Cancel_Update();
		GetServerIP_Update();
	}





	protected void AdvertiseServer_Update()
	{
		if (mAdvertiseServerTriggered)
		{
			if (Time.time > mEndTime)
			{
				if (mIsDebug) Debug.Log("ServerLookupWifiDirect: AdvertiseServerUpdate() timeout.");
				mResult = "failure";
				mAdvertiseServerTriggered = false;
			}
			else if (Time.time > mNextSubCall)
			{
				if (mAdvertiseServerSubStatus == AdvertiseServerSubStatus.Creating)
				{
					if (mIsDebug) Debug.Log("ServerLookupWifiDirect: Calling CreateNetwork().");
					mWifiP2P.CreateNetwork();
					mAdvertiseServerSubStatus = AdvertiseServerSubStatus.Created;
					mNextSubCall = Time.time + mSubCallCoolDown;
				}
				else if (mAdvertiseServerSubStatus == AdvertiseServerSubStatus.Created)
				{
					if (mWifiP2P.GetCreateNetworkStatus() == -1)
					{
						// Success. Go to next phase.
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: CreateNetwork() success.");
						mAdvertiseServerSubStatus = AdvertiseServerSubStatus.Advertising;
						mNextSubCall = Time.time + mSubCallCoolDown;
					}
					else 
					{
						// Try again. Don't continue further below.
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: CreateNetwork() failure.");
						mAdvertiseServerSubStatus = AdvertiseServerSubStatus.Creating;
						mNextSubCall = Time.time + mSubErrCoolDown;
					}
				}
				else if (mAdvertiseServerSubStatus == AdvertiseServerSubStatus.Advertising)
				{
					if (mIsDebug) Debug.Log("ServerLookupWifiDirect: Calling AdvertiseNetwork().");
					mWifiP2P.AdvertiseNetwork(mRoomCode, mWifiDirectServiceName, mWifiDirectAdvertisedPort);
					mAdvertiseServerSubStatus = AdvertiseServerSubStatus.Advertised;
					mNextSubCall = Time.time + mSubCallCoolDown;
				}
				else if (mAdvertiseServerSubStatus == AdvertiseServerSubStatus.Advertised)
				{
					if (mWifiP2P.GetAdvertiseNetworkStatus() == -1)
					{
						// Success.
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: AdvertiseNetwork() success - Room Code: "+
							mRoomCode+".");
						mResult = mRoomCode;
						mAdvertiseServerTriggered = false;

						mAdvertiseServerMaintainTriggered = true;
						mNextSubCall = Time.time + mAdvertiseCoolDown;
					}
					else 
					{
						// Try again.
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: AdvertiseNetwork() failure.");
						mAdvertiseServerSubStatus = AdvertiseServerSubStatus.Advertising;
						mNextSubCall = Time.time + mSubErrCoolDown;
					}
				}
			}
		}
		else if (mAdvertiseServerMaintainTriggered)
		{
//			if (Time.time > mNextSubCall)
//			{
//				mWifiP2P.AdvertiseNetwork(mRoomCode, mWifiDirectServiceName, mWifiDirectAdvertisedPort);
//				mNextSubCall = Time.time + mAdvertiseCoolDown;
//			}
		}
	}


	protected void GetServerIP_Update()
	{
		if (mGetServerIPTriggered)
		{
			// Stop trying if exceeded time allocated.
			if (Time.time > mEndTime)
			{
				if (mIsDebug) Debug.Log("ServerLookupWifiDirect: GetServerIPUpdate() timeout.");
				mResult = "failure";
				mGetServerIPTriggered = false;
			}
			else if (Time.time > mNextSubCall)
			{
				// Turn on network discovery.
				if (mGetServerIPSubStatus == GetServerIPSubStatus.Discovering)
				{
					if (mIsDebug) Debug.Log("ServerLookupWifiDirect: Calling DiscoverNetworks().");
					mWifiP2P.DiscoverNetworks(mWifiDirectServiceName);
					mGetServerIPSubStatus = GetServerIPSubStatus.Discovered;
					mNextSubCall = Time.time + mSubCallCoolDown;
				}
				else if (mGetServerIPSubStatus == GetServerIPSubStatus.Discovered)
				{
					if (mWifiP2P.GetDiscoverNetworksStatus_part1of2() == -1 && 
						mWifiP2P.GetDiscoverNetworksStatus_part2of2() == -1)
					{
						// Success.
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: DiscoverNetworks() success.");
						mGetServerIPSubStatus = GetServerIPSubStatus.DiscoveryFetching;
						mNextSubCall = Time.time + mSubCallCoolDown;
					}
					else 
					{
						// Try again.
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: DiscoverNetworks() failure.");
						mGetServerIPSubStatus = GetServerIPSubStatus.Discovering;	
						mNextSubCall = Time.time + mSubErrCoolDown;
					}
				}
				// Fetch discovered networks.
				else if (mGetServerIPSubStatus == GetServerIPSubStatus.DiscoveryFetching)
				{
					if (mIsDebug) Debug.Log("ServerLookupWifiDirect: Calling GetDiscoveredNetworks().");
					List<NetworkP2PInfo> nws = mWifiP2P.GetDiscoveredNetworks();
					mDiscoveredNetwork = SearchNetworkP2PInfo(mRoomCode, nws);
					if (mDiscoveredNetwork != null)
					{
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: GetDiscoveredNetworks() success.");
						mGetServerIPSubStatus = GetServerIPSubStatus.Joining;	
						mNextSubCall = Time.time;
					}
					else 
					{
						mGetDiscoveredNetworksAttempts++;
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: GetDiscoveredNetworks() failure ("
							+mGetDiscoveredNetworksAttempts+"/"+mMaxGetDiscoveredNetworksAttempts+")");
						if (mGetDiscoveredNetworksAttempts < mMaxGetDiscoveredNetworksAttempts)
						{
							// Try fetching again.
							mNextSubCall = Time.time + mSubCallCoolDown;
						}
						else // Exhausted fetching attempts.
						{
							// Try discovering again.
							mGetServerIPSubStatus = GetServerIPSubStatus.Discovering; 
							mNextSubCall = Time.time;
							mGetDiscoveredNetworksAttempts = 0;
						}
					}
				}
				// Join fetched network.
				else if (mGetServerIPSubStatus == GetServerIPSubStatus.Joining)
				{
					if (mIsDebug) Debug.Log("ServerLookupWifiDirect: Calling JoinNetwork().");
					mWifiP2P.JoinNetwork( mDiscoveredNetwork );
					mGetServerIPSubStatus = GetServerIPSubStatus.Joined; 
					mNextSubCall = Time.time + mSubJoinCallCoolDown;
				}
				else if (mGetServerIPSubStatus == GetServerIPSubStatus.Joined)
				{
					// Successful to join, but ensure can retrieve server host's IP.
					if (mWifiP2P.GetJoinNetworkStatus() == -1)
					{
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: JoinNetwork() success.");
						string hostIP = mWifiP2P.GetNetworkOwnerIP();
						if (hostIP != "")
						{
							if (mIsDebug) Debug.Log("ServerLookupWifiDirect: GetNetworkOwnerIP() success - Host IP: "+
								hostIP+".");
							// Success on retrieve host IP
							mWifiP2P.StopDiscoverNetworks();
							mResult = hostIP;
							mGetServerIPTriggered = false;
						}
						else
						{
							if (mIsDebug) Debug.Log("ServerLookupWifiDirect: GetNetworkOwnerIP() failure.");
							// Failure to retrieve host IP. Try joining again.
							mWifiP2P.CancelJoinNetwork();
							mGetServerIPSubStatus = GetServerIPSubStatus.Joining;
							mNextSubCall = Time.time + mSubErrCoolDown;
						}
					}
					// Failure to join.
					else
					{
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: JoinNetwork() failure.");
						mWifiP2P.CancelJoinNetwork();
						mGetServerIPSubStatus = GetServerIPSubStatus.Joining;
						mNextSubCall = Time.time + mSubErrCoolDown;
					}
				}
			}
		}
	}


	protected void Cancel_Update()
	{
		if (mCancelTriggered)
		{
			// Stop trying if timeout.
			if (Time.time > mEndTime)
			{
				if (mIsDebug) Debug.Log("ServerLookupWifiDirect: CancelAdvertiseServer() timeout.");
				mResult = "failure";
				mCancelTriggered = false;
			}
			else if (Time.time > mNextSubCall)
			{
				if (mCancelSubStatus == CancelSubStatus.Unjoining)
				{
					if (mIsDebug) Debug.Log("ServerLookupWifiDirect: Calling CancelJoinNetwork().");
					mWifiP2P.CancelJoinNetwork();
					mCancelSubStatus = CancelSubStatus.Unjoined;
					mNextSubCall = Time.time + mSubCallCoolDown;
				}
				else if (mCancelSubStatus == CancelSubStatus.Unjoined)
				{
					if (mWifiP2P.GetCancelJoinNetworkStatus() == -1 ||
						mWifiP2P.GetCancelJoinNetworkStatus() == 2)		// Nothing to cancel joining. Ok.
					{
						// Success.
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: CancelJoinNetwork() success.");
						mCancelSubStatus = CancelSubStatus.MemoryRemoving;
						mNextSubCall = Time.time;
					}
					else if (mWifiP2P.GetCancelJoinNetworkStatus() == -2)
					{
						// Results not ready. Try again later.
						mNextSubCall = Time.time + mSubCallCoolDown;
					}
					else
					{
						// Failure.
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: CancelJoinNetwork() failure.");
						mResult = "failure";
						mCancelTriggered = false;
					}
				}
				else if (mCancelSubStatus == CancelSubStatus.MemoryRemoving)
				{
					if (mIsDebug) Debug.Log("ServerLookupWifiDirect: Calling RemoveRememberedNetwork().");
					mWifiP2P.RemoveRememberedNetwork();
					mCancelSubStatus = CancelSubStatus.Disconnecting;
					mNextSubCall = Time.time + mSubCallCoolDown;
				}
				else if (mCancelSubStatus == CancelSubStatus.Disconnecting)
				{
					if (mIsDebug) Debug.Log("ServerLookupWifiDirect: Calling DisconnectFromNetwork().");
					mWifiP2P.DisconnectFromNetwork();
					mCancelSubStatus = CancelSubStatus.Disconnected;
					mNextSubCall = Time.time + mSubCallCoolDown;
				}
				else if (mCancelSubStatus == CancelSubStatus.Disconnected)
				{
					if (mWifiP2P.GetDisconnectFromNetworkStatus() == -1 ||
						mWifiP2P.GetDisconnectFromNetworkStatus() == 2)		// Nothing to disconnect. Ok.
					{
						// Success.
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: DisconnectFromNetwork() success.");
						mCancelSubStatus = CancelSubStatus.AdvertiseStopping;
						mNextSubCall = Time.time;
					}
					else if (mWifiP2P.GetDisconnectFromNetworkStatus() == -2)
					{
						// Results not ready. Try again later.
						mNextSubCall = Time.time + mSubCallCoolDown;
					}
					else 
					{
						// Failure.
						if (mIsDebug) Debug.Log("ServerLookupWifiDirect: DisconnectFromNetwork() failure.");
						mResult = "failure";
						mCancelTriggered = false;
					}
				}
				else if (mCancelSubStatus == CancelSubStatus.AdvertiseStopping)
				{
					if (mIsDebug) Debug.Log("ServerLookupWifiDirect: Calling StopAdvertiseNetwork().");
					mWifiP2P.StopAdvertiseNetwork();
					mCancelSubStatus = CancelSubStatus.AdvertiseStopped;
					mNextSubCall = Time.time;
				}
				else if (mCancelSubStatus == CancelSubStatus.AdvertiseStopped)
				{
					mResult = "success";
					mCancelTriggered = false;
				}
			}
		}
	}




	/* Helper functions */

	protected string GenerateRoomCode()
	{
		int roomCode = Random.Range(1000,9999);
		return roomCode.ToString();
	}
		
	protected NetworkP2PInfo SearchNetworkP2PInfo(string roomCode, List<NetworkP2PInfo> nws)
	{
		foreach (NetworkP2PInfo nw in nws)
			if (nw.mName == roomCode)
				return nw;
		
		return null;
	}
}
