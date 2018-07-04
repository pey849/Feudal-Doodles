using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Tips to get Android Wifi Direct working on Nvidia Shield:
 * 
 * 		- Settings > Apps > Shield Controller > Modify System Settings to "No"
 * 										      > Draw over other apps to "No"
 * 										      > Force Stop
 * 
 * 		- Settings > Developer Options > Forget all controllers
 * 
 * 		- Use RemoveRememberedNetwork() before DisconnectFromNetwork()
 * 
 */


/// <summary>
/// Independent class used to create or join a Wifi-Direct P2P network group.
/// </summary>
public class WifiP2P 
{
	// Handles creating and connecting to network groups.
	protected AndroidJavaObject mPeerConn;

	// Handles advertising and discovering network groups.
	protected AndroidJavaObject mServiceDiscovery;


    /// <summary>
    /// Create an object that can be used to create or join a Wifi-Direct P2P network group.
    /// This is only compatible with Android devices.
    /// </summary>
	public WifiP2P() 
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass  player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity"); 

            mPeerConn = new AndroidJavaObject("com.example.wifip2p.PeerConnection", activity);
            mServiceDiscovery = new AndroidJavaObject("com.example.wifip2p.ServiceDiscovery", activity);
        #else 
            Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
        #endif
	}



	/// <summary>
	/// Create and join an ad-hoc network in the public wifi space.
	/// </summary>
	/// 
	/// <remarks>
	/// Call AdvertiseNetwork() afterwards for remote devices to discover your network.
	/// </remarks>
	/// 
	/// <returns>
	/// Call GetCreateNetworkStatus() afterwards to retrieve the status of this callback function:
	/// -2 if not ready yet.
	/// -1 if success.
	/// otherwise, failure - likely due to you already being in a network. Call DisconnectFromNetwork() first.
	/// </returns>
	public void CreateNetwork()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
            mPeerConn.Call("createGroup");
        #else 
            Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
        #endif
	}
	public int GetCreateNetworkStatus()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			return mPeerConn.Get<int>("mCreateGroupStatus");
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
			return -2;
		#endif
	}


	/// <summary>
	/// Advertise your ad-hoc network in the public wifi space for remote devices to see.
	/// </summary>
	/// 
	/// <remarks>
	/// Precond :: CreateNetwork() was called beforehand and successful. Otherwise, a network will 
    ///            automatically be created when a client calls JoinNetwork() to your advertisement.
	/// 
	/// WARNING: Use lowercase only for the parameters.
	/// 
	/// Remote devices will be able to see your advertised network by them calling DiscoverNetworks().
    ///
    /// To prevent the advertisement from "sleeping" unexpectedly, call DiscoverNetworks() repeatedly 
    /// every 5 seconds to keep your advertisement broadcast active.
	/// </remarks>
	/// 
	/// <param name="networkName">Name of your network (e.g. "billy").</param>
	/// <param name="networkType">Purpose of your network (e.g. "_feudalDoodles").</param>
	/// <param name="listenPort">When a client joins your network, the client can retrieve your listen port
	///                          to create a connection to a socket of yours.</param>
	/// 
	/// <returns>
	/// Call GetAdvertiseNetworkStatus() afterwards to retrieve the status of this callback function:
	///  -2 if not ready yet.
	///  -1 if success.
	///  otherwise, failure.
	/// </returns>
	public void AdvertiseNetwork(string networkName, string networkType, string listenPort)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			mServiceDiscovery.Call("advertiseService", networkName, networkType, listenPort);
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
		#endif
	}
	public int GetAdvertiseNetworkStatus()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			return mServiceDiscovery.Get<int>("mAdvertiseServiceStatus");
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
			return -2;
		#endif
	}


	/// <summary>
    /// Find all ad-hoc networks in the public wifi space.
	/// </summary>
    ///
    /// <remarks>
    /// Use GetDiscoveredNetworks() afterwards to retrived discovered networks.
    ///
    /// To prevent the discovery from sleeping unexpectedly, repeatedly call this function every 5 seconds.
	/// </remarks> 
    ///
	/// <param name="networkType">Type of network to discover (e.g. "_fuedalDoodles")</param>
	/// 
    /// <returns>
	/// Call GetDiscoverNetworksStatus_part1of2() and GetDiscoverNetworksStatus_part2of2() afterwards
	/// to retrieve the status of this callback function:
    ///  -2 if not ready yet.
    ///  -1 if success.
    ///  otherwise, failure.
    /// </returns>
	public void DiscoverNetworks(string networkType)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
            mServiceDiscovery.Call("discoverServices", networkType);
        #else 
            Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
        #endif
	}
	public int GetDiscoverNetworksStatus_part1of2()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			return mServiceDiscovery.Get<int>("mDiscoverServicesStatus_addServiceRequest");
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
			return -2;
		#endif
	}
	public int GetDiscoverNetworksStatus_part2of2()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			return mServiceDiscovery.Get<int>("mDiscoverServicesStatus_discoverServices");
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
			return -2;
		#endif
	}


    /// <summary>
	/// Retrieve a list of discovered networks.
	/// </summary>
    ///
    /// <remarks>
    /// Precond :: DiscoverNetworks() was called beforehand and successful.
    /// </remarks>
    ///
    /// <returns>
	/// An empty list will be returned if there currently isn't any discovered networks yet. You can call this 
	/// repeatedly until the returned list is non-empty.
    ///
    /// You can access each NetworkP2PInfo class members from the list to gain info about the discovered network 
    /// such as its name. Pass the NetworkP2PInfo to JoinNetwork() to join the network.
    /// </returns> 
	public List<NetworkP2PInfo> GetDiscoveredNetworks()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject jNetworks = mServiceDiscovery.Call<AndroidJavaObject>("getDiscoveredServices");
            int numNetworks = jNetworks.Call<int>("size");

            List<NetworkP2PInfo> networkList = new List<NetworkP2PInfo>();
            for (int i = 0; i < numNetworks; i++)
            {
                AndroidJavaObject jService = jNetworks.Call<AndroidJavaObject>("get", i);
                NetworkP2PInfo nwInfo = new NetworkP2PInfo(jService);
                networkList.Add( nwInfo );
            }

            return networkList;
        #else 
            Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
            return null;
        #endif
	}

	/// <summary>
    /// Join a discovered network.
	/// </summary>
    ///
    /// <remarks>
    /// Use CountNumNetworkMembers() afterwards to confirm whether or not you are connected to 
    /// the network.
    /// </remarks>
    ///
	/// <param name="network">Network to join. This can be obtained from GetDiscoveredNetworks().</param>
	/// 
    /// <returns>
	/// Call GetJoinNetworkStatus() afterwards to retrieve the status of this callback function:
    ///  -2 if not ready yet.
    ///  -1 if success.
    ///  otherwise, failure.
	/// </returns>
	public void JoinNetwork(NetworkP2PInfo network)
	{
		string macAddress = network.mMacAddress;

		#if UNITY_ANDROID && !UNITY_EDITOR
            mServiceDiscovery.Call("connectToService", macAddress);
        #else 
            Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
			Debug.Log("Ignore: " + macAddress);
        #endif
	}
	public int GetJoinNetworkStatus()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			return mServiceDiscovery.Get<int>("mConnectToServiceStatus");
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
			return -2;
		#endif
	}


	/// <summary>
    /// Cancel an on-going join request.
	/// </summary>
    ///
    /// <remarks>
    /// Precond :: JoinNetwork() was called beforehand.
    ///
    /// This function is useful to retry JoinNetwork().
    /// </remarks>
	/// 
    /// <returns>
	/// Call GetCancelJoinNetworkStatus() afterwards to retrieve the status of this callback function:
    ///  -2 if not ready yet.
    ///  -1 if success.
    ///  otherwise, failure.
	/// </returns>
	public void CancelJoinNetwork()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			mPeerConn.Call("cancelConnectToGroup");
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
		#endif
	}
	public int GetCancelJoinNetworkStatus()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			return mPeerConn.Get<int>("mCancelConnectToGroupStatus");
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
			return -2;
		#endif
	}



    /// <summary> 
	/// Get the network owner's IP address.
	/// </summary>
    /// 
	/// <remarks>
    /// Precond :: CreateNetwork() or JoinNetwork() called beforehand. And,
    ///            CountNumNetworkMembers_RV() >= 0.
	/// </remarks>
    ///
    /// <returns>
    /// IP address of the owner of the current joined network.
    /// </returns>
	public string GetNetworkOwnerIP()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			return mPeerConn.Call<string>("getGroupOwnerIpAddr");
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
			return "";
		#endif
	}


    /// <summary>
	/// Disconnect from the current network.
	/// </summary>
    /// 
	/// <remarks>
    /// Precond :: Either CreateNetwork() or JoinNetwork() was called beforehand.
    /// </remarks>
	public void DisconnectFromNetwork()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			mPeerConn.Call("disconnectFromGroup");
        #else 
            Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
        #endif
	}
	public int GetDisconnectFromNetworkStatus()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			return mPeerConn.Get<int>("mDisconnectFromGroupStatus");
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
			return -2;
		#endif
	}


	/// <summary>
	/// Debugging purposes. Do not use.
	/// </summary>
	public void RemoveRememberedNetwork()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			mPeerConn.Call("forceRemoveGroup");
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
		#endif
	}



    /// <summary>
    /// Stop searching for networks.
    /// </summary>
    /// 
    /// <remarks>
    /// Precond :: CreateNetwork() or getDiscoveredNetworks() was called beforehand.
    /// </remarks>
	public void StopDiscoverNetworks()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
            mServiceDiscovery.Call("cancelDiscoverServices");
        #else 
            Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
        #endif
	}


    /// <summary>
    /// Stop advertising the network.
    /// </summary>
    /// 
    /// <remarks>
    /// Precond ::  CreateNetwork() was called beforehand.
    /// </remarks>
	public void StopAdvertiseNetwork()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
            mServiceDiscovery.Call("cancelAdvertiseService");
        #else 
            Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
        #endif
	}


    /// <summary>
    /// Count the number of network members in the current network, excluding the host and yourself.
    /// </summary>
    /// 
    /// <remarks>
    /// Precond :: Either CreateNetwork() or JoinNetwork() was called beforehand.
    /// </remarks>
    ///
    /// <returns>
    /// Call CountNumNetworkMembers_RV() afterwards to retrieve the count.
    /// It will return -2 if count is not yet available, or -3 if not in a group.
    /// <returns>
	public void CountNumNetworkMembers()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			mPeerConn.Call("countNumGroupMembers");
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
		#endif
	}
	public int CountNumNetworkMembers_RV()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
			return mPeerConn.Call<int>("getNumGroupMembers");
		#else 
			Debug.Log("WARNING: WifiP2P object is disabled when not running on Android device.");
			return -2;
		#endif
	}

}
