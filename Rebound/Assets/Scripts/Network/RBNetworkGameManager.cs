using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBNetworkGameManager : NetworkBehaviour {


    public static RBNetworkGameManager Instance;

    public GameObject Ball;
    public Transform BallSpawnPosition;
    
    private GameObject spawnedBall;

    private void Start()
    {
        Instance = this;

        if (isServer)
        {
            RegisterNetworkMessages();
        }
    }

    private void Update()
    {
        if (!isServer)
            return;

        if(spawnedBall == null)
        {
            spawnedBall = Instantiate(Ball, BallSpawnPosition.position, Quaternion.identity);
            NetworkServer.Spawn(spawnedBall);
        }
    }

    private void RegisterNetworkMessages()
    {
        NetworkServer.RegisterHandler((short)RBCustomMsgTypes.RBPlayerMovementMessage, OnReceivePlayerMovementMessage);
        NetworkServer.RegisterHandler((short)RBCustomMsgTypes.RBPlayerPhysicsMessage, OnRecievePlayerPhysicsMessage);
    }

    private void OnReceivePlayerMovementMessage(NetworkMessage _message)
    {
        RBPlayerMovementMessage _msg = _message.ReadMessage<RBPlayerMovementMessage>();
        NetworkServer.SendToAll((short)RBCustomMsgTypes.RBPlayerMovementMessage, _msg);
    }

    private void OnRecievePlayerPhysicsMessage(NetworkMessage _message)
    {
        print("Server Recieve");
        RBPlayerPhysicsMessage _msg = _message.ReadMessage<RBPlayerPhysicsMessage>();
        NetworkServer.SendToAll((short)RBCustomMsgTypes.RBPlayerPhysicsMessage, _msg);
    }

}
