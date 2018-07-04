namespace Doodle.Game
{
	// How client and server will discover and connect to each other before UDP/TCP connections are created.
	public enum NetworkType
	{
		InternetIP,
		InternetRoomCode,
		LocalUDPBroadcast,
		LocalWifiDirect
	};
}
