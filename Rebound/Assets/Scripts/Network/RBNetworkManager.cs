using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBNetworkManager : NetworkManager {

    public static RBNetworkManager Instance { get { return singleton as RBNetworkManager; } }

    public override NetworkClient StartHost()
    {
        return base.StartHost();
    }

    public NetworkClient StartClient(RBLanConnectionInfo connInfo)
    {
        networkAddress = connInfo.IpAddress;
        networkPort = connInfo.Port;
        
        return base.StartClient();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("New client connected: " + conn.address);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("Client disconnected: " + conn.address);
    }
}
