using UnityEngine.UI;
using UnityEngine;
using Doodle.Networking;
using Doodle.Game;

public class DisplayIPAddress : MonoBehaviour
{
    private string LocalAddress;
    private NetworkManager Network;

    // Use this for initialization
    void Start()
    {
        LocalAddress = NetworkHost.GetLocalAddress();
        Network = FindObjectOfType<NetworkManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var text = GetComponent<Text>();
        var count1 = Network.NetworkHost.ConnectionCount;
        text.text = string.Format( "IP: {0} | Connections: {1}", LocalAddress, count1 );
    }
}
