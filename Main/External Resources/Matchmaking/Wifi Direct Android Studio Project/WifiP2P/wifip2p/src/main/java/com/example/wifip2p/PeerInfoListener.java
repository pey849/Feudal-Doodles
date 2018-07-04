package com.example.wifip2p;

import android.net.wifi.p2p.WifiP2pInfo;
import android.net.wifi.p2p.WifiP2pManager;
import android.util.Log;

public class PeerInfoListener implements WifiP2pManager.ConnectionInfoListener
{
    protected static final String DEBUG_TAG = "PeerInfoListener";

    protected String mGroupOwnerIpAddr = "";

    /* This function is invoked upon PeerConnection.connectToGroup() ->
       WIFI_P2P_CONNECTION_CHANGED_ACTION -> requestConnectionInfo() */
    @Override
    public void onConnectionInfoAvailable(WifiP2pInfo info)
    {
        Log.d(DEBUG_TAG, "onConnectionInfoAvailable() - called");

        if (info == null || info.groupOwnerAddress == null)
        {
            mGroupOwnerIpAddr = "";
        }
        else if (info.groupFormed && info.isGroupOwner)
        {
            // Do whatever tasks are specific to the group owner.
            // One common case is creating a group owner thread and accepting
            // incoming connections.
            mGroupOwnerIpAddr = info.groupOwnerAddress.getHostAddress();
            Log.d(DEBUG_TAG, "onConnectionInfoAvailable() :: I am group owner. IP: " + mGroupOwnerIpAddr);
        }
        else if (info.groupFormed)
        {
            // The other device acts as the peer (client). In this case,
            // you'll want to create a peer thread that connects
            // to the group owner.
            mGroupOwnerIpAddr = info.groupOwnerAddress.getHostAddress();
            Log.d(DEBUG_TAG, "onConnectionInfoAvailable() - Group formed. IP: " + mGroupOwnerIpAddr);
        }
        else
        {
            mGroupOwnerIpAddr = "";
            Log.d(DEBUG_TAG, "onConnectionInfoAvailable() - Group not formed.");
        }
    }

    public String getGroupOwnerIpAddr()
    {
        return mGroupOwnerIpAddr;
    }
}
