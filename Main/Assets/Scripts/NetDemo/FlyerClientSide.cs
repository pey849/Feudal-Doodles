using UnityEngine;
using Doodle.InputSystem.Handlers;
using Doodle.Networking.Serialization;
using Doodle.Networking.Packets;
using Doodle.Networking;
using System.Text;
using Doodle.Game;
using System.Collections.Generic;
using Doodle.InputSystem;

public class FlyerClientSide : MonoBehaviour
{
    private InputHandler Input;
    private NetworkManager Network;

    public GameObject FlyerPrefab;

    private Dictionary<PlayerId, GameObject> FlyerObjects;
    private PlayerId? PlayerId = null;

    private void Start()
    {
        FlyerObjects = new Dictionary<PlayerId, GameObject>();
        Network = GetComponent<NetworkManager>();
        Input = GetComponent<InputHandler>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if( Network.NetworkHost.ConnectionCount > 0 )
        {
            ReceivePackets();
            SendPackets();
        }
    }

    private void ReceivePackets()
    {
        // 
        while( Network.PacketQueue.HasPacket<PlayerAcceptPacket>() )
        {
            var packet = Network.PacketQueue.GetNextPacket<PlayerAcceptPacket>();
            PlayerId = packet.PlayerId;

            // 
            Debug.LogFormat( "Accepted as {0}", PlayerId.Value );
        }

        // Wait for player to be accepted
        if( PlayerId.HasValue )
        {
            // 
            while( Network.PacketQueue.HasPacket<TransformStatePacket>() )
            {
                var packet = Network.PacketQueue.GetNextPacket<TransformStatePacket>();
                var playerId = packet.PlayerId;

                // We haven't seen this flyer before, create it
                if( FlyerObjects.ContainsKey( playerId ) == false )
                {
                    FlyerObjects[playerId] = Instantiate( FlyerPrefab );

                    // Local object
                    if( playerId == PlayerId.Value )
                    {
                        var gamepad = gameObject.AddComponent<GamepadInputHandler>();
                        FlyerObjects[playerId].GetComponent<FlyerPlayer>().Input = gamepad;
                    }
                }

                // Update flyers transform
                var flyer = FlyerObjects[playerId];
                flyer.transform.rotation = Quaternion.AngleAxis( packet.Angle, Vector3.forward );
                flyer.transform.position = packet.Position;
            }
        }
    }

    private void SendPackets()
    {
        // Waits for player to be accepted
        if( PlayerId.HasValue )
        {
            var inputPacket = new InputStatePacket( PlayerId.Value, Input );
            Network.SendPacket( "input", inputPacket );
        }
    }
}
