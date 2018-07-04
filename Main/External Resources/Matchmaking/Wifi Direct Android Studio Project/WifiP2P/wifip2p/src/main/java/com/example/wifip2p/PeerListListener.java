package com.example.wifip2p;

import android.net.wifi.p2p.WifiP2pDevice;
import android.net.wifi.p2p.WifiP2pDeviceList;
import android.net.wifi.p2p.WifiP2pManager;
import android.util.Log;

import java.util.Collection;
import java.util.List;

public class PeerListListener implements WifiP2pManager.PeerListListener
{
    protected static final String DEBUG_TAG = "PeerListListener";

    protected List<String> mDiscoveredPeers;

    public PeerListListener(List<String> discoveredPeers)
    {
        mDiscoveredPeers = discoveredPeers;
    }

    @Override
    public void onPeersAvailable(WifiP2pDeviceList incomingPeerList)
    {
        Collection<WifiP2pDevice> refreshedPeers = incomingPeerList.getDeviceList();
        for (WifiP2pDevice refreshedPeer : refreshedPeers)
        {
            Log.d(DEBUG_TAG, "onPeersAvailable(): " + refreshedPeer.deviceAddress);
            mDiscoveredPeers.add(refreshedPeer.deviceAddress);
        }
    }
}
