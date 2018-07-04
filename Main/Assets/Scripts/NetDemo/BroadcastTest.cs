using Doodle.Networking;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using Doodle.Networking.Serialization;

public class BroadcastTest : MonoBehaviour
{
    NetworkHost Network;

    PacketSerializer Serializer;
    HashSet<NetworkInfo> KnownPeople;

    // Use this for initialization
    void Start()
    {
        KnownPeople = new HashSet<NetworkInfo>();
        Serializer = new JSONPacketSerializer();

        // 
        Network = new NetworkHost( 9560, 1 );
        Network.ConfigureChannel( "broadcast", ChannelType.UnreliableUnordered );
        Network.Open();

        // Mark broadcast credentials
        Network.SetBroadcastCredentials( 1234, 1 );

        // 
        NetworkHost.StartBroadcast( Network, Encoding.ASCII.GetBytes( NetworkHost.GetLocalAddress() ) );

        // 
        var textUi = GetComponent<Text>();
        textUi.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        Network.ProcessEvents();

        // Broadcast Packets
        BroadcastPacket bPacket;
        while( Network.ReceiveBroadcast( out bPacket ) )
        {
            // If not a local address
            if( !NetworkHost.IsLocalAddress( bPacket.RemoteInfo.Address ) )
            {
                var address = bPacket.RemoteInfo.Address;
                var port = bPacket.RemoteInfo.Port;

                Debug.LogFormat( "Broadcast Received From: {0}:{1}", address, port );

                if( KnownPeople.Contains( bPacket.RemoteInfo ) ) continue;
                else
                {
                    KnownPeople.Add( bPacket.RemoteInfo );

                    // 
                    var textUi = GetComponent<Text>();
                    textUi.text += address + "\n";
                }
            }
        }

        // Regular Packets
        Packet packet;
        while( Network.Receive( out packet ) )
        {
            // 
        }
    }
}

[Packet( PacketType.Broadcast )]
public struct BroadcastStruct
{
    public byte[] Address;
    public int Port;
}
