using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBNetworkManager : NetworkManager
{
    private short _clientCount = 0;

    public static RBNetworkManager Instance { get { return singleton as RBNetworkManager; } }

    public event Action OnAddPlayer;
    public event Action OnRemovePlayer;


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

    /// <summary>
    /// Called on the client when connected to a server.
    /// The default implementation of this function sets the client as ready and adds a player.
    /// </summary>
    /// <param name="connection"></param>
    public override void OnClientConnect(NetworkConnection connection)
    {
        Debug.Log("Client online: " + connection.address + "with id: " + connection.connectionId);
        ClientScene.AddPlayer(connection, 2);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        ClientScene.RemovePlayer(2);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        _clientCount++;
        OnAddPlayer?.Invoke();

        if(_clientCount == 3)
        {
            ServerChangeScene("VerticalPrototypeArena");
        }
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        _clientCount--;
        OnRemovePlayer?.Invoke();
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
