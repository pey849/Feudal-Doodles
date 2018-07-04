using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;  
using System.Net.Sockets;  
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine.UI;

public class WifiP2P_Example : MonoBehaviour 
{
	protected WifiP2P mWifiP2P;
	protected List<NetworkP2PInfo> mDiscoveredNetworks;

	public Text mCreateNetworkRC_text;
	public Text mAdvertiseNetworkRC_text;
	public InputField mAdvertiseNetwork_inputField;

	public InputField mListenIP_inputField;
	public InputField mListenRecvFromClient_inputField;
	public InputField mListenRecvToClient_inputField;

	public Text mDiscoverNetworksRC1_text;
	public Text mDiscoverNetworksRC2_text;
	public InputField mGetDiscoveredNetworks_inputField;
	public Text mJoinNetworkRC_text;
	public InputField mJoinNetwork_inputField;
	public Text mCancelJoinNetworkTC_text;

	public InputField mConnectIP_inputField;
	public InputField mConnectFromServer_inputField;
	public InputField mConnectToServer_inputField;

	public Text mDisconnectFromNetworkRC_text;
	public Text mNetworkOwnerIP_display_text;
	public InputField mPort_inputField;
	public InputField mPortStatus_inputField;
	public Text mMemberCount_text;

	public Socket mSendSock = null;
	public int mNumSent = 1;

	public Socket mListSock = null;
	public Socket mServSock = null;
	public int mNumRecv = 0;

	void Start () 
	{
		mWifiP2P = new WifiP2P();
	}
	

	void Update () 
	{
		mCreateNetworkRC_text.text = "Return Code: " + mWifiP2P.GetCreateNetworkStatus();
		mAdvertiseNetworkRC_text.text = "Return Code: " + mWifiP2P.GetAdvertiseNetworkStatus();

		mDiscoverNetworksRC1_text.text = "Return Code: " + mWifiP2P.GetDiscoverNetworksStatus_part1of2();
		mDiscoverNetworksRC2_text.text = "Return Code: " + mWifiP2P.GetDiscoverNetworksStatus_part2of2();
		mJoinNetworkRC_text.text = "Return Code: " + mWifiP2P.GetJoinNetworkStatus();

		mDisconnectFromNetworkRC_text.text = "Return Code: " + mWifiP2P.GetDisconnectFromNetworkStatus();

		mCancelJoinNetworkTC_text.text = "Return Code: " + mWifiP2P.GetCancelJoinNetworkStatus();
	}



	/* Host operations */

	public void onClick_createNetwork()
	{
		mWifiP2P.CreateNetwork();
	}

	public void onClick_advertiseNetwork()
	{
		string port = mPort_inputField.text;
		mWifiP2P.AdvertiseNetwork(mAdvertiseNetwork_inputField.text, "_feudalDoodles", port);
	}

	public void onClick_stopAdvertiseNetwork()
	{
		mWifiP2P.StopAdvertiseNetwork();
	}

	public void onClick_serverListen()
	{
		int port = Int32.Parse(mPort_inputField.text);
		IPEndPoint localEndPoint =  new IPEndPoint(IPAddress.Parse(mListenIP_inputField.text), port);
		mListSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		Debug.Log("Using IP: " + mListenIP_inputField.text);

		try 
		{
			mListSock.Bind(localEndPoint);
			mListSock.Listen(10);

			//mListenRecv_inputField.text = "Waiting for connection!";

			// Blocking.
			mServSock = mListSock.Accept();
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

	public void onClick_serverRecv()
	{
		string recvData = null;
		byte[] recvDataRaw = new byte[1024];

		try 
		{
			mNumRecv = 0;

			while (mNumRecv < 1)
			{
				recvData = "";

				while (true)
				{
					recvDataRaw = new byte[1024];
					//Debug.Log("Server Receive()");
					int bytesRec = mServSock.Receive(recvDataRaw);
					recvData += Encoding.ASCII.GetString(recvDataRaw, 0, bytesRec);
					if (recvData.IndexOf("<EOF>") > -1)
						break;
				}
				Debug.Log("Server Recv: " + recvData);

				mListenRecvFromClient_inputField.text = recvData;

				mNumRecv++;

				byte[] replyMsgRaw = Encoding.ASCII.GetBytes(mListenRecvToClient_inputField.text);
				mServSock.Send(replyMsgRaw);
			}

		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

	public void onClick_serverClose()
	{
		try 
		{
			mServSock.Shutdown(SocketShutdown.Both);
			mServSock.Close();
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}



	/* Client operations */

	public void onClick_discoverNetworks()
	{
		mWifiP2P.DiscoverNetworks("_feudalDoodles");
	}

	public void onClick_getDiscoveredNetworks()
	{
		string networkNames = "";
		mDiscoveredNetworks = mWifiP2P.GetDiscoveredNetworks();
		foreach (NetworkP2PInfo nw in mDiscoveredNetworks)
		{
			networkNames += nw.mName + "\n";
		}
		mGetDiscoveredNetworks_inputField.text = networkNames;
	}

	public void onClick_joinNetwork()
	{
		string targetNwName = mJoinNetwork_inputField.text;

		foreach (NetworkP2PInfo nw in mDiscoveredNetworks)
		{
			if (nw.mName == targetNwName)
			{
				mWifiP2P.JoinNetwork(nw);
				break;
			}
		}
	}
		
	public void onClick_cancelJoinNetwork()
	{
		mWifiP2P.CancelJoinNetwork();
	}

	public void onClick_stopDiscoverNetworks()
	{
		mWifiP2P.StopDiscoverNetworks();
	}

	public void onClick_clientConnect()
	{
		try 
		{
			int port = Int32.Parse(mPort_inputField.text);
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(mConnectIP_inputField.text), port);
			mSendSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			mSendSock.Connect(remoteEP);
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

	public void onClick_clientSend()
	{
		byte[] inBytes = new byte[1024];

		try 
		{
			byte[] outMsg = Encoding.ASCII.GetBytes(mConnectToServer_inputField.text + " <EOF>");
			mSendSock.Send(outMsg);
			mNumSent++;

			//Listen for reply.
			int bytesRecv = mSendSock.Receive(inBytes);
			mConnectFromServer_inputField.text = Encoding.ASCII.GetString(inBytes, 0, bytesRecv);
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

	public void onClick_clientClose()
	{
		try 
		{
			mSendSock.Shutdown(SocketShutdown.Both);
			mSendSock.Close();
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

	public void onClick_removeRemembered()
	{
		mWifiP2P.RemoveRememberedNetwork();
	}



	/* Host and client operations */

	public void onClick_disconnectFromNetwork()
	{
		mWifiP2P.DisconnectFromNetwork();
	}
		
	public void onClick_getNetworkOwnerIP()
	{
		string ip = mWifiP2P.GetNetworkOwnerIP();
		mNetworkOwnerIP_display_text.text = ip;
	}

	public void onClick_testPort()
	{
		int port = Int32.Parse(mPort_inputField.text);
		bool isAvailable = true;

		IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

		TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
		foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
		{
			if (tcpi.LocalEndPoint.Port == port)
			{
				isAvailable = false;
				break;
			}
		}

		IPEndPoint[] ipEndPoints = ipGlobalProperties.GetActiveTcpListeners();
		foreach(IPEndPoint endPt in ipEndPoints)
		{
			if (endPt.Port == port)
			{
				isAvailable = false;
				break;
			}
		}

		if (isAvailable)
			mPortStatus_inputField.text = "Open!";
		else
			mPortStatus_inputField.text = "Closed!";
	}


	public void onClick_countNumNetworkMembers()
	{
		mWifiP2P.CountNumNetworkMembers();
	}
	public void onClick_getNumNetworkMembers()
	{
		int count = mWifiP2P.CountNumNetworkMembers_RV();
		mMemberCount_text.text = Convert.ToString(count);
	}
}
