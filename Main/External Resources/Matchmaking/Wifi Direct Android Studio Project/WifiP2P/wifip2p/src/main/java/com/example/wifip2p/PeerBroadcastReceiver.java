package com.example.wifip2p;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.NetworkInfo;
import android.net.wifi.p2p.WifiP2pManager;
import android.util.Log;

import java.util.List;

public class PeerBroadcastReceiver extends BroadcastReceiver
{
    public static final String DEBUG_TAG = "PeerBroadcastReceiver";

    protected WifiP2pManager mManager;
    protected WifiP2pManager.Channel mChannel;

    protected List<String> mDiscoveredPeers;
    protected PeerListListener mPeerListListener;
    protected PeerInfoListener mPeerInfoListener;

    public PeerBroadcastReceiver(WifiP2pManager manager,
                                 WifiP2pManager.Channel channel,
                                 List<String> discoveredPeers)
    {
        super();
        mManager = manager;
        mChannel = channel;

        mDiscoveredPeers = discoveredPeers;

        mPeerListListener = new PeerListListener(mDiscoveredPeers);
        mPeerInfoListener = new PeerInfoListener();
    }

    @Override
    public void onReceive(Context context, Intent intent)
    {
        String action = intent.getAction();

        if (WifiP2pManager.WIFI_P2P_STATE_CHANGED_ACTION.equals(action))
        {
            // Check to see if Wi-Fi is enabled and notify appropriate activity
            int state = intent.getIntExtra(WifiP2pManager.EXTRA_WIFI_STATE, -1);
            if (state == WifiP2pManager.WIFI_P2P_STATE_ENABLED)
            {
                // Wifi P2P is enabled
                Log.d(DEBUG_TAG, "Wifi P2P is enabled");
            }
            else
            {
                // Wi-Fi P2P is not enabled
                Log.d(DEBUG_TAG, "Wi-Fi P2P is not enabled");
            }

        }
        /* Invoked from PeerConnection.discoverPeers() */
        else if (WifiP2pManager.WIFI_P2P_PEERS_CHANGED_ACTION.equals(action))
        {
            Log.d(DEBUG_TAG, "onReceive() - WIFI_P2P_PEERS_CHANGED_ACTION");
            /* Async call will be made to PeerListListener.onPeersAvailable() when peer list
               is available. */
            mManager.requestPeers(mChannel, mPeerListListener);
        }
        else if (WifiP2pManager.WIFI_P2P_CONNECTION_CHANGED_ACTION.equals(action))
        {
            // Respond to new connection or disconnections
            Log.d(DEBUG_TAG, "onReceive() - WIFI_P2P_CONNECTION_CHANGED_ACTION");

            NetworkInfo networkInfo = (NetworkInfo) intent
                    .getParcelableExtra(WifiP2pManager.EXTRA_NETWORK_INFO);

            if (networkInfo.isConnected())
            {
                // We are connected with the other device, request connection
                // info to find group owner IP
                mManager.requestConnectionInfo(mChannel, mPeerInfoListener);
            }
        }
        else if (WifiP2pManager.WIFI_P2P_THIS_DEVICE_CHANGED_ACTION.equals(action))
        {
            // Respond to this device's wifi state changing
            Log.d(DEBUG_TAG, "onReceive() - WIFI_P2P_THIS_DEVICE_CHANGED_ACTION");
        }
    }

    public String getGroupOwnerIpAddr()
    {
        return mPeerInfoListener.mGroupOwnerIpAddr;
    }
}
