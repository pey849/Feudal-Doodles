using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkP2PInfo
{
	public string mName;
	public string mListenPort;
	public string mMacAddress;

	public NetworkP2PInfo()
	{
	}

	public NetworkP2PInfo(AndroidJavaObject service)
	{
		mName = service.Get<string>("mBuddyName");
		mListenPort = service.Get<string>("mListenPort");
		mMacAddress = service.Get<string>("mMacAddress");
	}
}
