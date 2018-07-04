package com.example.wifip2p;

import java.util.HashMap;
import java.util.Map;


public class Service
{
    public String mInstanceName;
    public String mIpAddress;
    public String mMacAddress;
    public String mListenPort;
    public String mBuddyName;
    public String mServiceType;
    public String mTransportLayer;

    public Service() {}

    public Service(Map<String,String> hashMapForm)
    {
        mInstanceName = hashMapForm.get("instanceName");
        mIpAddress = hashMapForm.get("ipAddress");
        mMacAddress = hashMapForm.get("macAddress");
        mListenPort = hashMapForm.get("listenPort");
        mBuddyName = hashMapForm.get("buddyName");
        mServiceType = hashMapForm.get("serviceType");
        mTransportLayer = hashMapForm.get("transportLayer");
    }

    public HashMap<String,String> convertToHashMap()
    {
        HashMap<String,String> service = new HashMap<String,String>();
        service.put("instanceName", mInstanceName);
        service.put("ipAddress",mIpAddress);
        service.put("macAddress",mMacAddress);
        service.put("listenPort",mListenPort);
        service.put("buddyName",mBuddyName);
        service.put("serviceType",mServiceType);
        service.put("transportLayer",mTransportLayer);

        return service;
    }

    public Service clone()
    {
        Service clonedService = new Service();
        clonedService.mInstanceName = mInstanceName;
        clonedService.mIpAddress = mIpAddress;
        clonedService.mMacAddress = mMacAddress;
        clonedService.mListenPort = mListenPort;
        clonedService.mBuddyName = mBuddyName;
        clonedService.mServiceType = mServiceType;
        clonedService.mTransportLayer = mTransportLayer;
        return clonedService;
    }
}
