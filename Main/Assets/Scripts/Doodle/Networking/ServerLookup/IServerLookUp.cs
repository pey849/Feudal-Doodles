using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;  
using System.IO;  
using System.Net;  
using System.Text;  
using Newtonsoft.Json;

/// <summary>
/// Establish a common IP network between hosts and clients. Hosts will be able to advertise 
/// its network using AdvertiseServer(). Clients will be able to discover and join a host's network using 
/// GetServerIP(). Afterwards, the client can use the host's IP directly (e.g. C# sockets) to communicate.
/// 
/// There are 3 implementations of the interface: 
/// 	ServerLookupHTTP.cs
/// 	ServerLookupWifiDirect.cs
/// 	ServerLookupUDPBroadcast.cs
/// </summary>
public interface IServerLookup
{
	/// <summary>
	/// Advertises your server in the public network space.
	/// </summary>
	/// 
	/// <remarks>
	/// Your IP address will automatically be displayed to the public.
	/// 
	/// For Wifi Direct or UDP Broadcast implementation of this function, this function will
	/// automatically handle making your IP address connectable by the clients.
	/// </remarks>
	/// 
	/// <returns>
	/// Call AdvertiseServerResult() to retrieve the result:
	/// 	If success, returns four-digit room code of your server. Share it with your
	///     clients for them to discover your IP address using GetServerIP(roomCode). 
	///     If failure, returns "failure".
	/// 	If not ready yet, returns null.
	/// </returns>
	void AdvertiseServer();
	string AdvertiseServerResult();

	/// <summary>
	/// Client: Lookup a server room code to retrieve the corresponding server host's IP address. For Wifi Direct or
	/// UDP Broadcast implementation of this function, this function will automatically handle making the host IP 
	/// address connectable as a client.
	/// 
	/// Host: Get your IP address that clients will connect to. AdvertiseServer() must be called first and successful.
	/// </summary>
	/// 
	/// <param name="roomCode">
	/// If client, provide the server room code. This is obtained from the host's AdvertiseServer() call. If host,
	/// provide null to tell this function to obtain your local IP.
	/// </param>
	/// 
	/// <returns>
	/// Call GetServerIPResult() to retrieve the result:
	/// 	If success, returns the corresponding server host's IP address. 
	/// 	If server cannot be found, returns "failure".
	/// 	If not ready yet, returns null.
	/// </returns>
	void GetServerIP(string roomCode);
	string GetServerIPResult();

	/// <summary>
	/// Cancel AdvertiseServer() or GetServerIP(). 
	/// 
	/// For Wifi Direct implementation, as a server, this will destroy your wifi group and disconnect
	/// any connected clients. As a client, you will simply leave the wifi group. In both cases of the server and
	/// client, you should call Cancel() before calling AdvertiseServer() or GetServerIP().
	/// 
	/// For HTTP implementation, cancel() is only needed for server to undo AdvertiseServer(). Do not call this 
	/// as a client.
	/// </summary>
	/// 
	/// <returns>
	/// Call CancelResult() to retrieve the result:
	/// 	If success, returns "success".
	///     If failure, returns "failure".
	/// 	If not ready, returns null.
	/// </returns>
	void Cancel();
	string CancelResult();
}
