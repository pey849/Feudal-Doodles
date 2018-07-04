package com.example.wifip2p;

import android.app.Activity;
import android.content.Context;
import android.net.wifi.WpsInfo;
import android.net.wifi.p2p.WifiP2pConfig;
import android.net.wifi.p2p.WifiP2pDevice;
import android.net.wifi.p2p.WifiP2pManager;
import android.net.wifi.p2p.nsd.WifiP2pDnsSdServiceInfo;
import android.net.wifi.p2p.nsd.WifiP2pDnsSdServiceRequest;
import android.util.Log;

import java.io.IOException;
import java.net.ServerSocket;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.regex.Pattern;



public class ServiceDiscovery
{
    public static final String DEBUG_TAG = "ServiceDiscovery";

    protected Activity mActivity;
    protected WifiP2pManager mManager;
    protected WifiP2pManager.Channel mChannel;

    protected Map<String,Service> mDiscoveredServices;

    /* Hold reference to a service request (i.e. service discovery) so can disable later */
    protected WifiP2pDnsSdServiceRequest mServiceRequest;

    /* Use these public members below to query status (i.e. success or failure) of their
     * corresponding methods.
     *
     *  -2 = method hasn't been called or completed yet.
     *  -1   = success
     */

    public int mAdvertiseServiceStatus;

    public int mDiscoverServicesStatus_addServiceRequest;
    public int mDiscoverServicesStatus_discoverServices;

    public int mConnectToServiceStatus;
    public int mDisconnectFromServiceStatus;

    public ServiceDiscovery(Activity activity)
    {
        mActivity = activity;
        mManager = (WifiP2pManager) mActivity.getSystemService(Context.WIFI_P2P_SERVICE);
        mChannel = mManager.initialize(mActivity, mActivity.getMainLooper(), null);

        mDiscoveredServices = new HashMap<String,Service>();

        mAdvertiseServiceStatus = -2;
        mDiscoverServicesStatus_addServiceRequest = -2;
        mDiscoverServicesStatus_discoverServices = -2;
        mConnectToServiceStatus = -2;
        mDisconnectFromServiceStatus = -2;
    }

    /* Advertise a service in the public wifi space.
     *
     * Remote devices will be able to see it by calling discoverServices().
     *
     * TODO:  This methods automatically calls discoverServices() on success. Investigate why
     *        this is needed for remote devices to discover this service.
     *
     * @param buddyName :: Name of the service (e.g. "billy" or "colorprinter")
     * @param serviceType :: Type of service. (e.g. "_dragongame")
     * @param listenPort :: Port that clients should use to connect to your listen socket.
     *                      (e.g. "5000")
     *
     * WARNING: Use lowercase for the above arguments.
     *
     * "_tcp" will be used as the transfer protocol of the service.
     *
     * Check mAdvertiseServiceStatus to see current status.
     *  -2 if not ready yet.
     *  -1 if success.
     *  otherwise, failure.
     */
    public void advertiseService(String buddyName, final String serviceType, String listenPort)
    {
        mAdvertiseServiceStatus = -2;

        /* Create info about my new service that client should see after they connect */
        Map serviceRecord = new HashMap<String,String>();
        serviceRecord.put("buddyName", buddyName);
        serviceRecord.put("listenPort", listenPort);

        final WifiP2pDnsSdServiceInfo serviceInfo =
                WifiP2pDnsSdServiceInfo.newInstance(buddyName, serviceType+"._tcp", serviceRecord);

        mManager.clearLocalServices(mChannel, new WifiP2pManager.ActionListener() {
            @Override
            public void onSuccess()
            {
                Log.d(DEBUG_TAG, "clearLocalServices() - success!");

                mManager.addLocalService(mChannel, serviceInfo, new WifiP2pManager.ActionListener() {
                    @Override
                    public void onSuccess() {
                        Log.d(DEBUG_TAG, "startService() - success!");
                        mAdvertiseServiceStatus = -1;
                        discoverServices(serviceType);
                    }

                    @Override
                    public void onFailure(int reason) {
                        Log.d(DEBUG_TAG, "startService() - failure!");
                        mAdvertiseServiceStatus = reason;
                    }
                });
            }

            @Override
            public void onFailure(int reason)
            {
                Log.d(DEBUG_TAG, "clearLocalServices() - failure: " + reason);
                mAdvertiseServiceStatus = reason;
            }
        });
    }

    /* Find all services in the public wifi space.
     *
     * Use getDiscoveredServices() afterwards to retrieve queried discovered services.
     *
     * IMPORTANT: You may need to call this more than once to get a response.
     *
     * @param filterInServiceType :: type of service to discover (e.g. "_airplaneGame")
     *
     * Check mDiscoverServicesStatus_addServiceRequest and mDiscoverServicesStatus_discoverServices
     * to see current status.
     *  -2 if not ready yet.
     *  -1 if success.
     *  otherwise, failure.
     */
    public void discoverServices(final String filterInServiceType)
    {
        mDiscoveredServices.clear();
        mDiscoverServicesStatus_addServiceRequest = -2;
        mDiscoverServicesStatus_discoverServices = -2;

        /* Invoked when discovered a service domain
         * TODO: UNUSED. SUPERSEDED BY onDnsSdTxtRecordAvailable() */
        WifiP2pManager.DnsSdServiceResponseListener serviceListener =
                new WifiP2pManager.DnsSdServiceResponseListener() {
                    @Override
                    public void onDnsSdServiceAvailable(String serviceInstanceName,
                                                        String serviceType,
                                                        WifiP2pDevice serviceDevice)
                    {
                        Log.d(DEBUG_TAG, "DnsSdService available - " +
                                serviceInstanceName + "." + serviceType);

                        if (serviceType.contains(filterInServiceType))
                        {
                            Log.d(DEBUG_TAG, "Filtered in DnsSDService: " +
                                    serviceInstanceName + "." + serviceType);
                        }
                    }
                };

        /* Invoke when discovered a service record */
        WifiP2pManager.DnsSdTxtRecordListener txtListener = new WifiP2pManager.DnsSdTxtRecordListener() {
            @Override
            public void onDnsSdTxtRecordAvailable(String serviceDomain,
                                                  Map<String, String> serviceRecord,
                                                  WifiP2pDevice serviceDevice)
            {
                Log.d(DEBUG_TAG, "DnsSdTxtRecord serviceDomain - " + serviceDomain);
                Log.d(DEBUG_TAG, "DnsSdTxtRecord record - " + serviceRecord.toString());

                /* Add to list of discovered services but only if the service type is expected */
                if (Pattern.compile(Pattern.quote(filterInServiceType), Pattern.CASE_INSENSITIVE).matcher(serviceDomain).find())
                {
                    Log.d(DEBUG_TAG, "Filtered in DnsSdTxtRecord: " + serviceDomain);

                    Service incomingService = new Service();
                    incomingService.mBuddyName = serviceRecord.get("buddyName");
                    incomingService.mListenPort = serviceRecord.get("listenPort");
                    incomingService.mMacAddress = serviceDevice.deviceAddress;

                    mDiscoveredServices.put(incomingService.mBuddyName, incomingService);
                }
            }
        };

        /* Attach both listeners to wifi manager */
        mManager.setDnsSdResponseListeners(mChannel, serviceListener, txtListener);

        /* Keep reference of our service request (i.e. discovery) so we can disable it later when
         * not needed. */
        mServiceRequest = WifiP2pDnsSdServiceRequest.newInstance();

        mManager.clearServiceRequests(mChannel, new WifiP2pManager.ActionListener() {
            @Override
            public void onSuccess()
            {
                Log.d(DEBUG_TAG, "clearServiceRequests(): Success");

                mManager.addServiceRequest(mChannel, mServiceRequest, new WifiP2pManager.ActionListener() {
                    @Override
                    public void onSuccess()
                    {
                        Log.d(DEBUG_TAG, "addServiceRequest(): onSuccess");
                        mDiscoverServicesStatus_addServiceRequest = -1;
                    }

                    @Override
                    public void onFailure(int reason)
                    {
                        Log.d(DEBUG_TAG, "addServiceRequest(): onFailure. Reason: " + reason);
                        mDiscoverServicesStatus_addServiceRequest = reason;
                    }
                });

                 /* Start finding new services to connect to */
                mManager.discoverServices(mChannel, new WifiP2pManager.ActionListener() {
                    @Override
                    public void onSuccess()
                    {
                        Log.d(DEBUG_TAG, "discoverServices(): success.");
                        mDiscoverServicesStatus_discoverServices = -1;
                    }

                    @Override
                    public void onFailure(int reason)
                    {
                        Log.d(DEBUG_TAG, "discoverServices(): failure.");
                        mDiscoverServicesStatus_discoverServices = reason;
                    }
                });
            }

            @Override
            public void onFailure(int reason)
            {
                Log.d(DEBUG_TAG, "clearServiceRequests(): Failure");
                mAdvertiseServiceStatus = reason;
            }
        });
    }

    /* Connect to a discovered service. You will become a peer in the service's local network.
     *
     * The Wifi Direct icon in the android status bar will appear upon successful connect.
     *
     * @param macAddress :: the MAC address of the service owner. This can be retrieved from
     *                      getDiscoveredServices()
     *
     * Check mConnectToServiceStatus to see current status.
     *  -2 if not ready yet.
     *  -1 if success.
     *  otherwise, failure.
     */
    public void connectToService(String macAddress)
    {
        mConnectToServiceStatus = -2;

        WifiP2pConfig cfg = new WifiP2pConfig();
        cfg.deviceAddress = macAddress;
        cfg.wps.setup = WpsInfo.PBC;

        mManager.connect(mChannel, cfg, new WifiP2pManager.ActionListener() {
            @Override
            public void onSuccess()
            {
                Log.d(DEBUG_TAG, "connectToService() - success");
                mConnectToServiceStatus = -1;
            }

            @Override
            public void onFailure(int reason)
            {
                Log.d(DEBUG_TAG, "connectToService() - failure: " + reason);
                mConnectToServiceStatus = reason;
            }
        });
    }

    /* Disconnect from the current service. You will no longer become a peer of the service's
     * local network.
     *
     * The Wifi Direct icon in the android status bar will disappear upon successful disconnect.
     *
     * Check mDisconnectFromServiceStatus to see current status.
     *  -2 if not ready yet.
     *  -1 if success.
     *  otherwise, failure.
     */
    public void disconnectFromService()
    {
        mDisconnectFromServiceStatus = -2;
        mManager.removeGroup(mChannel, new WifiP2pManager.ActionListener() {
            @Override
            public void onSuccess() {
                Log.d(DEBUG_TAG, "disconnectFromService() - success");
                mDisconnectFromServiceStatus = -1;
            }

            @Override
            public void onFailure(int reason) {
                Log.d(DEBUG_TAG, "disconnectFromService() - failure");
                mDisconnectFromServiceStatus = reason;
            }
        });
    }

    /* Get discovered services.
     *
     * @precond discoverServices() should have been called beforehand.
     *
     * Return: Each entry in the HashMap will contain:
     *      Key:  The buddy name of the service. (e.g. "Billy's Lobby")
     *      Value:  A service object containing details about the service:
     *                  mListenPort :: The port that the service owner will be listening on
     *                                 to accept client connections.
     *                  mMacAddress :: The MAC address to be used to join the service's local
     *                                 network. Pass this connectToService().
     */
    public List<Service> getDiscoveredServices()
    {
        /* Make a copy of the list of discovered services*/
        List<Service> rv = new ArrayList<Service>();

        for (Map.Entry<String,Service> entry : mDiscoveredServices.entrySet())
        {
            String buddyName = entry.getKey();
            Service service = entry.getValue();

            Log.d(DEBUG_TAG, "getDiscoveredServices() - buddyName: " + buddyName);
            Log.d(DEBUG_TAG, "getDiscoveredServices() - listenPort: " + service.mListenPort);
            Log.d(DEBUG_TAG, "getDiscoveredServices() - MAC: " + service.mMacAddress);
            rv.add(service.clone());
        }

        return rv;
    }

    /* Stop the search for services.
     *
     * Note that this doesn't disconnect from any connected service local network. If needed,
     * call disconnectFromService() instead.
     *
     * @precond discoverServices() was called beforehand.
     */
    public void cancelDiscoverServices()
    {
        mManager.clearServiceRequests(mChannel, new WifiP2pManager.ActionListener() {
            @Override
            public void onSuccess() {
                Log.d(DEBUG_TAG, "cancelDiscoverServices() - success");
            }

            @Override
            public void onFailure(int reason) {
                Log.d(DEBUG_TAG, "cancelDiscoverServices() - failure");
            }
        });
    }

    /* Stop advertising the service.
     *
     * Note that this doesn't disconnect from any connected service local network. If needed,
     * call disconnectFromService() instead.
     *
     * @precond advertiseService() was called beforehand.
     */
    public void cancelAdvertiseService()
    {
        mManager.clearLocalServices(mChannel, new WifiP2pManager.ActionListener() {
            @Override
            public void onSuccess() {
                Log.d(DEBUG_TAG, "cancelAdvertiseService() - success");
            }

            @Override
            public void onFailure(int reason) {
                Log.d(DEBUG_TAG, "cancelAdvertiseService() - failure");
            }
        });
    }

    public int getOpenPort()
    {
        ServerSocket s = null;
        try
        {
            s = new ServerSocket(0);
            int port = s.getLocalPort();
            Log.d(DEBUG_TAG, "getOpenPort(): " + port);
            return port;
        }
        catch (IOException e)
        {
            e.printStackTrace();
            Log.d(DEBUG_TAG, "getOpenPort(): failure");
            return -1;
        }
    }

    public boolean closePort(int port)
    {
        ServerSocket s = null;
        try
        {
            s = new ServerSocket(port);
            s.close();
            Log.d(DEBUG_TAG, "closePort(): success");
            return true;
        }
        catch (IOException e)
        {
            e.printStackTrace();
            Log.d(DEBUG_TAG, "closePort(): failure");
            return false;
        }
    }
}
