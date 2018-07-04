using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;  
using System.IO;  
using System.Net;  
using System.Text;  
using Newtonsoft.Json;
using Doodle.Networking;

public class ServerLookupHTTP : MonoBehaviour,IServerLookup
{
	public string ServerLookupHostname = "http://feudaldoodleslookupserver-fightinggames.rhcloud.com";
	public int mTimeout_ms = 3000;
	protected string mResult = null;

	public void AdvertiseServer()
	{
		mResult = null;

		string publicIP = NetworkHost.GetLocalAddress();
		string url = ServerLookupHostname + "/api/addserver?ip="+publicIP;
		Debug.Log("Making webrequest to: " + url);
		WebRequest req = WebRequest.Create( url );
		req.ContentType = "application/json; charset=utf-8";
		req.Timeout = mTimeout_ms;

		WebResponse res = req.GetResponse();

		Stream resStream = res.GetResponseStream();
		StreamReader reader = new StreamReader(resStream);
		string resData = reader.ReadToEnd();

		reader.Close();
		res.Close();

		Dictionary<string,string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(resData);
		if (data["status"] == "ok")
			mResult = data["roomCode"];
		else 
			mResult = "failure";
	}
	public string AdvertiseServerResult()
	{
		return mResult;
	}

	public void Cancel()
	{
		mResult = null;

		string publicIP = NetworkHost.GetLocalAddress();
		string url = ServerLookupHostname + "/api/removeserver?ip="+publicIP;
		Debug.Log("Making webrequest to: " + url);
		WebRequest req = WebRequest.Create( url );
		req.ContentType = "application/json; charset=utf-8";
		req.Timeout = mTimeout_ms;

		WebResponse res = req.GetResponse();

		Stream resStream = res.GetResponseStream();
		StreamReader reader = new StreamReader(resStream);
		string resData = reader.ReadToEnd();

		reader.Close();
		res.Close();

		Dictionary<string,string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(resData);
		if (data["status"] == "ok")
			mResult = "success";
		else 
			mResult = "failure";
	}
	public string CancelResult()
	{
		return mResult;
	}

	public void GetServerIP(string roomCode)
	{
		// If host is wanting its own local IP address
		if (roomCode == null)
		{
			mResult = NetworkHost.GetLocalAddress();
			return;
		}

		mResult = null;

		string url = ServerLookupHostname + "/api/queryserver" +
			         "?roomCode=" + roomCode;
		Debug.Log("Making webrequest to: " + url);
		WebRequest req = WebRequest.Create( url );
		req.ContentType = "application/json; charset=utf-8";

		WebResponse res = req.GetResponse();

		Stream resStream = res.GetResponseStream();
		StreamReader reader = new StreamReader(resStream);
		string resData = reader.ReadToEnd();
		Debug.Log(resData);
		reader.Close();
		res.Close();

		Dictionary<string,string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(resData);
		if (data["status"] == "ok")
			mResult = data["ip"];
		else 
			mResult = "failure";
	}
	public string GetServerIPResult()
	{
		return mResult;
	}




}
