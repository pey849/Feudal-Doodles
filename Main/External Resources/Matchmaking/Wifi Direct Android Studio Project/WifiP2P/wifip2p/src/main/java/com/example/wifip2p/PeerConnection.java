package com.example.wifip2p;

import android.app.Activity;
import android.content.Context;
import android.content.IntentFilter;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.net.wifi.WpsInfo;
import android.net.wifi.p2p.WifiP2pConfig;
import android.net.wifi.p2p.WifiP2pGroup;
import android.net.wifi.p2p.WifiP2pManager;
import android.util.Log;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

public class PeerConnection
{
    public static final String DEBUG_TAG = "PeerConnection";

    protected Activity mActivity;
    protected WifiP2pManager mManager;
    protected WifiP2pManager.Channel mChannel;

    protected PeerBroadcastReceiver mReceiver;
    protected IntentFilter mIntentFilter;

    protected List<String> mDiscoveredPeers;
    protected int mNumGroupMembers;

    public int mDiscoverPeersStatus;
    public int mConnectToGroupStatus;
    public int mCreateGroupStatus;
    public int mDisconnectFromGroupStatus;
    public int mCancelConnectToGroupStatus;

    public PeerConnection(Activity activity)
    {
        mActivity = activity;
        mManager = (WifiP2pManager) mActivity.getSystemService(Context.WIFI_P2P_SERVICE);
        mChannel = mManager.initialize(mActivity, mActivity.getMainLooper(), null);

        mIntentFilter = new IntentFilter();
        mIntentFilter.addAction(WifiP2pManager.WIFI_P2P_STATE_CHANGED_ACTION);
        mIntentFilter.addAction(WifiP2pManager.WIFI_P2P_PEERS_CHANGED_ACTION);
        mIntentFilter.addAction(WifiP2pManager.WIFI_P2P_CONNECTION_CHANGED_ACTION);
        mIntentFilter.addAction(WifiP2pManager.WIFI_P2P_THIS_DEVICE_CHANGED_ACTION);

        mNumGroupMembers = -2;

        mDiscoverPeersStatus = -2;
        mConnectToGroupStatus = -2;
        mCreateGroupStatus = -2;
        mDisconnectFromGroupStatus = -2;

        /* Should move to onResume() */
        mDiscoveredPeers = new ArrayList<String>();
        mReceiver = new PeerBroadcastReceiver(mManager, mChannel, mDiscoveredPeers);
        activity.registerReceiver(mReceiver, mIntentFilter);

        WifiManager manager = (WifiManager) mActivity.getSystemService(Context.WIFI_SERVICE);
        WifiInfo info = manager.getConnectionInfo();
        String address = info.getMacAddress();
        Log.d(DEBUG_TAG, "MAC: " + address);
    }

    /* Scan for devices in the Wifi space.
     *
     * Remote peers need to invoke discoverPeers() to be discovered.
     *
     * Detected peers will appear in the Android Monitor:
     *      D/PeerListListener: onPeersAvailable(): 1e:b7:2c:04:0c:86
     *
     * Call getDiscoveredPeers() afterwards to retrieved discovered peers.
     *
     * Check mDiscoverPeersStatus to see current status.
     *  -2 if not ready yet.
     *  -1 if success.
     *  otherwise, failure.
     */
    public void discoverPeers()
    {
        mDiscoveredPeers.clear();
        mDiscoverPeersStatus = -2;

        mManager.discoverPeers(mChannel, new WifiP2pManager.ActionListener() {
            @Override
            public void onSuccess()
            {
                /* System broadcasts the WIFI_P2P_PEERS_CHANGED_ACTION intent, which you can
                   listen for in PeerBroadcastReceiver.onReceive() to obtain list of peers. */
                Log.d(DEBUG_TAG, "discoverPeers() - success");
                mDiscoverPeersStatus = -1;
            }

            @Override
            public void onFailure(int reason)
            {
                Log.d(DEBUG_TAG, "discoverPeers() - failure");
                mDiscoverPeersStatus = reason;
            }
        });
    }

    /* Get MAC address of each discovered peers.
     *
     * discoverPeers() should be called beforehand.
     *
     * Empty list will be returned if haven't found any yet. If so, wait a bit before
     * calling this method again.
     */
    public ArrayList<String> getDiscoveredPeers()
    {
        ArrayList<String> clonedList = new ArrayList<String>();

        for (String peerAddress : mDiscoveredPeers)
        {
            Log.d(DEBUG_TAG, "getDiscoveredPeers(): " + peerAddress);
            clonedList.add(peerAddress);
        }

        return clonedList;
    }

    /* Connect to a Wifi Direct group by providing the group owner's MAC address.
     *
     * The group owner should have called createGroup() already.
     * You may need to call discoverPeers() beforehand for this method to work.
     *
     * In the Android Monitor, you should see:
     *      D/PeerConnection: connectToGroup() - success
     *
     * As well, you should see a Wifi Direct icon in the android status bar.
     *
     * Call getGroupOwnerIpAddr() afterwards to retrieve the group owner's IP address.
     *
     * Check mConnectToGroup to see current status.
     *  -2 if not ready yet.
     *  -1 if success.
     *  otherwise, failure.
     */
    public void connectToGroup(String groupOwnerMacAddress)
    {
        mConnectToGroupStatus = -2;

        WifiP2pConfig cfg = new WifiP2pConfig();
        cfg.deviceAddress = groupOwnerMacAddress;
        cfg.wps.setup = WpsInfo.PBC;

        mManager.connect(mChannel, cfg, new WifiP2pManager.ActionListener() {
            @Override
            public void onSuccess()
            {
                // PeerBroadcastReceiver will notify us. Ignore for now.
                Log.d(DEBUG_TAG, "connectToGroup() - success");
                mConnectToGroupStatus = -1;
            }

            @Override
            public void onFailure(int reason)
            {
                Log.d(DEBUG_TAG, "connectToGroup() - failure: " + reason);
                mConnectToGroupStatus = reason;
            }
        });
    }

    /* Cancel an on-going connectToGroup() call.
     *
     * This is useful if connectToGroup() returns success but countNumGroupMembers()
     * is reporting a failure.
     *
     * After calling this function, you can retry calling connectToGroup().     
     *
     * Check mCancelConnectToGroupStatus to see current status.
     *  -2 if not ready yet.
     *  -1 if success.
     *  otherwise, failure.
     */
    public void cancelConnectToGroup()
    {
        mCancelConnectToGroupStatus = -2;

        mManager.cancelConnect(mChannel, new WifiP2pManager.ActionListener() {
            @Override
            public void onSuccess()
            {
                Log.d(DEBUG_TAG, "cancelConnectToGroup() - success");
                mCancelConnectToGroupStatus = -1;
            }

            @Override
            public void onFailure(int reason)
            {
                Log.d(DEBUG_TAG, "cancelConnectToGroup() - failure");
                mCancelConnectToGroupStatus = reason;
            }
        });

    }


    /* Create a ad-hoc group via Wifi Direct.
     *
     * You will become a peer (group owner) of the group.
     *
     * In the Android Monitor, you should see:
     *      D/PeerConnection: createGroup() - success
     *      D/PeerInfoListener: Group owner IP address: 192.168.49.1
     *
     * The Wifi Direct icon will appear on your Android status bar.
     *
     * Check mCreateGroupStatus to see current status.
     *  -2 if not ready yet.
     *  -1 if success.
     *  otherwise, failure.
     */
    public void createGroup()
    {
        mCreateGroupStatus = -2;

        mManager.createGroup(mChannel, new WifiP2pManager.ActionListener() {
            @Override
            public void onSuccess()
            {
                // PeerBroadcastReceiver will notify us. Ignore for now.
                Log.d(DEBUG_TAG, "createGroup() - success");
                mCreateGroupStatus = -1;
            }

            @Override
            public void onFailure(int reason)
            {
                Log.d(DEBUG_TAG, "createGroup() - failure: " + reason);
                mCreateGroupStatus = reason;
            }
        });
    }

    /* Disconnect from the current group.
     *
     * This is assuming that createGroup() or connectToGroup() was called beforehand.
     *
     * You will see the Wifi Direct icon in the android status bar disappear.
     *
     * Check mDisconnectFromGroupStatus to see current status.
     *  -2 if not ready yet.
     *  -1 if success.
     *  otherwise, failure.
     */
    public void disconnectFromGroup()
    {
        mDisconnectFromGroupStatus = -2;

        mManager.removeGroup(mChannel, new WifiP2pManager.ActionListener() {
            @Override
            public void onSuccess()
            {
                Log.d(DEBUG_TAG, "disconnectFromGroups() - success");
                mDisconnectFromGroupStatus = -1;

                //forceRemoveGroup();
            }

            @Override
            public void onFailure(int reason)
            {
                Log.d(DEBUG_TAG, "disconnectFromGroups() - failure: " + reason);
                mDisconnectFromGroupStatus = reason;
            }
        });
    }

    /* Debugging code. Do not use. 
     * http://stackoverflow.com/questions/16372724/wi-fi-direct-android
     */ 
    public void forceRemoveGroup() {
        try {
            Class persistentInterface = null;

            //Iterate and get class PersistentGroupInfoListener.
            for (Class<?> classR : WifiP2pManager.class.getDeclaredClasses()) {
                if (classR.getName().contains("PersistentGroupInfoListener")) {
                    persistentInterface = classR;
                    break;
                }
            }

            final Method deletePersistentGroupMethod = 
                WifiP2pManager.class.getDeclaredMethod("deletePersistentGroup",
                    new Class[]{WifiP2pManager.Channel.class, 
                                int.class,
                                WifiP2pManager.ActionListener.class});

            //Anonymous class to implement PersistentGroupInfoListener which 
            //has a method, onPersistentGroupInfoAvailable.
            Object persitentInterfaceObject =
                    java.lang.reflect.Proxy.newProxyInstance(persistentInterface.getClassLoader(),
                            new java.lang.Class[]{persistentInterface},
                            new java.lang.reflect.InvocationHandler() {
                                @Override
                                public Object invoke(Object proxy, java.lang.reflect.Method method,
                                    Object[] args) throws java.lang.Throwable {
                                    String method_name = method.getName();

                                    if (method_name.equals("onPersistentGroupInfoAvailable")) {
                                        Class wifiP2pGroupListClass =  Class.forName("android.net.wifi.p2p.WifiP2pGroupList");
                                        Object wifiP2pGroupListObject = wifiP2pGroupListClass.cast(args[0]);

                                        Collection<WifiP2pGroup> wifiP2pGroupList = (Collection<WifiP2pGroup>) wifiP2pGroupListClass.getMethod("getGroupList", null).invoke(wifiP2pGroupListObject, null);
                                        for (WifiP2pGroup group : wifiP2pGroupList) {
                                            Log.d(DEBUG_TAG, "forceRemoveGroup(): " + group.getNetworkName());
                                            deletePersistentGroupMethod.invoke(mManager, mChannel, (Integer) WifiP2pGroup.class.getMethod("getNetworkId").invoke(group, null), new WifiP2pManager.ActionListener() {
                                                @Override
                                                public void onSuccess()
                                                {
                                                    //All groups deleted
                                                    Log.d(DEBUG_TAG, "forceRemoveGroup() - success");
                                                }

                                                @Override
                                                public void onFailure(int i) 
                                                {
                                                    Log.d(DEBUG_TAG, "forceRemoveGroup() - failure");
                                                }
                                            });
                                        }
                                    }

                                    return null;
                                }
                            });

            Method requestPersistentGroupMethod =
                    WifiP2pManager.class.getDeclaredMethod("requestPersistentGroupInfo", new Class[]{WifiP2pManager.Channel.class, persistentInterface});

            requestPersistentGroupMethod.invoke(mManager, mChannel, persitentInterfaceObject);

        } catch (Exception ex) {
            ex.printStackTrace();
        }
    }


    /* Get the group owner IP address.
     *
     * Ensure createGroup() or connectToGroup() was called beforehand.
     */
    public String getGroupOwnerIpAddr()
    {
        return mReceiver.getGroupOwnerIpAddr();
    }


    /* Get the number of devices connected to the group, excluding the group owner, (and excluding
    *  client himself if not the group owner?).
    *
    *  Ensure createGroup() or connectToGroup() was called beforehand.
    *
    *  Call getNumGroupMembers() afterwards to retrieve the count.
    *  -2 if not ready yet.
    *  -3 if failure (i.e. not connect to group).
    *  Otherwise, the number is the number of the members.
    */
    public void countNumGroupMembers()
    {
        mNumGroupMembers = -2;

        mManager.requestGroupInfo(mChannel, new WifiP2pManager.GroupInfoListener() {
            @Override
            public void onGroupInfoAvailable(WifiP2pGroup group)
            {
                if (group == null)
                {
                    mNumGroupMembers = -3;
                    Log.d(DEBUG_TAG, "countNumGroupMembers(): Group not setup?");
                }
                else
                {
                    mNumGroupMembers = group.getClientList().size();
                    Log.d(DEBUG_TAG, "countNumGroupMembers(): Count is " + mNumGroupMembers);
                }
            }
        });
    }
    public int getNumGroupMembers()
    {
        return mNumGroupMembers;
    }


}
