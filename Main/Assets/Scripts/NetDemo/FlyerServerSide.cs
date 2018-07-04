using UnityEngine;
using Doodle.InputSystem.Handlers;
using Doodle.Networking.Packets;
using Doodle.Networking;
using Doodle.Game;
using System.Collections.Generic;
using System;
using System.Linq;

public class FlyerServerSide : MonoBehaviour
{
    private NetworkManager Network;

    public GameObject FlyerPrefab;

    private Dictionary<INetworkConnection, PlayerId> ConnectionToPlayer;
    private Dictionary<PlayerId, NetworkInputHandler> InputHandlers;
    private Dictionary<PlayerId, GameObject> FlyerObjects;

    private int PID = 0;

    private void Start()
    {
        ConnectionToPlayer = new Dictionary<INetworkConnection, PlayerId>();
        InputHandlers = new Dictionary<PlayerId, NetworkInputHandler>();
        FlyerObjects = new Dictionary<PlayerId, GameObject>();

        // 
        Network = GetComponent<NetworkManager>();

        // 
        Debug.Log( "Starting Server Side" );
        Network.NetworkHost.Connected += Host_Connected;
        Network.NetworkHost.Disconnected += Host_Disconnected;
    }

    private void Stop()
    {
        Debug.Log( "Stopping Server Side" );
        Network.NetworkHost.Connected -= Host_Connected;
        Network.NetworkHost.Disconnected -= Host_Disconnected;
    }

    private void Host_Connected( INetworkConnection connection )
    {
        Debug.LogFormat( "Connected to {0}", connection.Id );

        PlayerId playerId;
        if( !GetNextPlayerId( out playerId ) )
        {
            connection.Dispose(); // Diconnect!
            throw new InvalidOperationException( "No more room! Unable to find next available player id." );
        }
        else
        {
            // Associate player id
            ConnectionToPlayer[connection] = playerId;

            // Create network input handler for player
            InputHandlers[playerId] = gameObject.AddComponent<NetworkInputHandler>();

            // Create flyer
            var flyerObject = Instantiate( FlyerPrefab );
            FlyerObjects[playerId] = flyerObject;

            // Get 
            var flyer = flyerObject.GetComponent<FlyerPlayer>();
            flyer.Input = InputHandlers[playerId];

            // Move object into camera's 2D coordinate
            flyer.transform.position = (Vector2) Camera.main.transform.position;

            // Inform player of their respective identifier.
            Debug.LogFormat( "Sending {0} to {1}", playerId, connection.Id );
            Network.SendPacket( "info", connection.Id, new PlayerAcceptPacket( playerId ) );
        }
    }

    private void Host_Disconnected( INetworkConnection connection )
    {
        var playerId = ConnectionToPlayer[connection];
        ConnectionToPlayer.Remove( connection );

        // Kill unity objects for the player
        Destroy( InputHandlers[playerId] );
        Destroy( FlyerObjects[playerId] );

        // Unmap the references
        InputHandlers.Remove( playerId );
        FlyerObjects.Remove( playerId );
    }

    /// <summary>
    /// Finds the next available identifier.
    /// </summary>
    private bool GetNextPlayerId( out PlayerId id )
    {
        var ids = (PlayerId[]) Enum.GetValues( typeof( PlayerId ) );
        var all = new HashSet<PlayerId>( ids );
        all.ExceptWith( FlyerObjects.Keys );
        id = all.FirstOrDefault();
        return all.Any();
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
        // Get packets
        while( Network.PacketQueue.HasPacket<InputStatePacket>() )
        {
            var inputState = Network.PacketQueue.GetNextPacket<InputStatePacket>();
            var playerId = inputState.PlayerId;

            // If we know about this player, update handler
            if( InputHandlers.ContainsKey( playerId ) )
                InputHandlers[playerId].EnqueueStatePacket( inputState );
        }
    }

    private void SendPackets()
    {
        // Update everyone about each others state.
        foreach( var flyerKV in FlyerObjects )
            Network.SendPacket( "state", new TransformStatePacket( flyerKV.Key, flyerKV.Value.transform ) );
    }
}
