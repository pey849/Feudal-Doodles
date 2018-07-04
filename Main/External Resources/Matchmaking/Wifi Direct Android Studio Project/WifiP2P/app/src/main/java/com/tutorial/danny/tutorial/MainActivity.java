package com.tutorial.danny.tutorial;

import android.Manifest;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.View;
import android.widget.EditText;

import com.example.wifip2p.PeerConnection;
import com.example.wifip2p.ServiceDiscovery;

import java.util.Date;

public class MainActivity extends AppCompatActivity {

    public final static String DEBUG_TAG = "MainActivity";

    private final String[] PERMISSIONS = new String[]{Manifest.permission.INTERNET,
            Manifest.permission.ACCESS_WIFI_STATE, Manifest.permission.CHANGE_WIFI_STATE,
            Manifest.permission.CHANGE_NETWORK_STATE, Manifest.permission.ACCESS_NETWORK_STATE};

    private static final String TAG = "MainActivity";
    private static final int PERMISSION_REQUEST_ID = 1;

    protected PeerConnection mPeerConn;
    protected ServiceDiscovery mServiceDiscovery;

    protected Integer mPort = -2;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        //if (!hasPermissions())
            requestPermissions();

        Date d = new Date();

        mPeerConn = new PeerConnection(this);
        mServiceDiscovery = new ServiceDiscovery(this);
    }

    public boolean hasPermissions()
    {
        for (String perm : PERMISSIONS)
        {
            if (ContextCompat.checkSelfPermission(this, perm) != PackageManager.PERMISSION_GRANTED)
            {
                Log.d(TAG, perm + " :: Didn't have permission.");
                return false;
            }
        }

        Log.d(TAG, "I have all permissions!");
        return true;
    }

    public void requestPermissions()
    {
        /* Don't forget to add permission in manifest file */
        ActivityCompat.requestPermissions(this, PERMISSIONS, PERMISSION_REQUEST_ID);
    }

    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           String permissions[], int[] grantResults)
    {
        switch (requestCode) {
            case PERMISSION_REQUEST_ID:
            {
                int nPerms = permissions.length;
                for (int i = 0; i < nPerms; i++)
                {
                    String perm = permissions[i];
                    int result = grantResults[i];

                    if (result == PackageManager.PERMISSION_GRANTED)
                        Log.d(TAG, perm + " :: Permission granted");
                    else
                        Log.d(TAG, perm + " :: Permission denied");
                }
            }
        }
    }

    public void onClick_createGroup(View view)
    {
        mPeerConn.createGroup();
    }

    public void onClick_getGroupIP(View view)
    {
        EditText editText = (EditText) findViewById(R.id.getGroupIP);

        String addr = mPeerConn.getGroupOwnerIpAddr();
        editText.setText(addr);
    }

    public void onClick_advertiseService(View view)
    {
        mServiceDiscovery.advertiseService("billy", "_dragongame", "5000");
    }

    public void onClick_discoverServices(View view)
    {
        mServiceDiscovery.discoverServices("_dragongame");
    }

    public void onClick_getDiscoveredServices(View view)
    {
        mServiceDiscovery.getDiscoveredServices();
    }

    public void onClick_connectToService(View view)
    {
        EditText editText = (EditText) findViewById(R.id.connectToService);
        String addr = editText.getText().toString();

        Log.d(DEBUG_TAG, "Service device to connect: " + addr);

        mServiceDiscovery.connectToService(addr);
    }

    public void onClick_disconnectFromService(View view)
    {
        //mServiceDiscovery.disconnectFromService();
        mPeerConn.disconnectFromGroup();
    }

    public void onClick_forceDisconnectFromService(View view)
    {
        //mServiceDiscovery.disconnectFromService();
        mPeerConn.forceRemoveGroup();
    }

    public void onClick_cancelDiscoverServices(View view)
    {
        mServiceDiscovery.cancelDiscoverServices();
    }

    public void onClick_cancelAdvertiseService(View view)
    {
        mServiceDiscovery.cancelAdvertiseService();
    }

    public void onClick_countNumGroupMembers(View view)
    {
        mPeerConn.countNumGroupMembers();
    }
}
