using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;  
using System.IO;  
using System.Net;  
using System.Text;  
using Newtonsoft.Json;

public class ServerLookupUDPBroadcast : MonoBehaviour,IServerLookup
{
	protected string mResult = null;

	public void AdvertiseServer()
	{
		// TODO
	}
	public string AdvertiseServerResult()
	{
		return mResult;
	}
		

	public void GetServerIP(string roomCode)
	{
		// TODO
	}
	public string GetServerIPResult()
	{
		return mResult;
	}


	public void Cancel()
	{
		// TODO
	}
	public string CancelResult()
	{
		return mResult;
	}
}
