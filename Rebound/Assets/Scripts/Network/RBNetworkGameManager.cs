using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class RBNetworkGameManager : NetworkBehaviour {


    public static RBNetworkGameManager Instance;

    private Dictionary<int, int> Score;

    private ARBArenaSetup ArenaSetup;
    private Coroutine coroutine;

    private void Start()
    {
        Instance = this;

        if (isServer)
        {
            RegisterNetworkMessages();
        }
        Score = new Dictionary<int, int>();
        ArenaSetup = GameObject.Find("GameStateController").GetComponent<ARBArenaSetup>();
        RespawnBall();


    }

    private void Update()
    {
        if (!isServer)
            return;




    }

    /// <summary>
    /// Called only on the server when one player scored
    /// </summary>
    public void Goal(GameObject Ball, int goalID)
    {
        if (!isServer) return;

        // Amount of goals recieved
        // Score[goalID] += 1;

        
        if (coroutine == null)
            coroutine = StartCoroutine(GoalWorker(Ball));
    }

    IEnumerator GoalWorker(GameObject Ball)
    {
        yield return new WaitForSeconds(3);

        NetworkServer.Destroy(Ball);
        RespawnBall();

        yield return null;
        coroutine = null;
    }

    private void RespawnBall()
    {
        if (isServer)
        {
            var BallObject = Instantiate(ArenaSetup.BallPrefab, ArenaSetup.BallStartPosition.position, ArenaSetup.BallStartPosition.rotation);
            BallObject.GetComponent<Rigidbody>().velocity =
                new Vector3(
                    UnityEngine.Random.Range(-25, 25),
                    0,
                    UnityEngine.Random.Range(-25, 25));
            NetworkServer.Spawn(BallObject);
        }
    }

    private void RegisterNetworkMessages()
    {
        NetworkServer.RegisterHandler((short)RBCustomMsgTypes.RBPlayerMovementMessage, OnReceivePlayerMovementMessage);
        NetworkServer.RegisterHandler((short)RBCustomMsgTypes.RBPlayerPhysicsMessage, OnRecievePlayerPhysicsMessage);
        NetworkServer.RegisterHandler((short)RBCustomMsgTypes.RBGameEventMessage, OnRecieveGameEventMessage);

    }

    private void OnRecieveGameEventMessage(NetworkMessage _message)
    {
        RBGameEventMessage _msg = _message.ReadMessage<RBGameEventMessage>();
        switch(_msg.TriggeredEventType)
        {
            case GameEvent.GameOver: break;
            case GameEvent.Goal:
                print("Goal by: " + _msg.TriggeredPlayerID + "\nIn Goal of Team: " + _msg.TriggeredTeamID);
                NetworkServer.SendToAll((short)RBCustomMsgTypes.RBGameEventMessage, _msg);
                break;
            case GameEvent.PowerUpCollected:
                print("Ball Collected Powerup " + _msg.GameEventInfo + " for Player: " + _msg.TriggeredPlayerID + "\nIn Team: " + _msg.TriggeredTeamID);
                break;
            default: break;
        }
    }

    private void OnReceivePlayerMovementMessage(NetworkMessage _message)
    {
        RBPlayerMovementMessage _msg = _message.ReadMessage<RBPlayerMovementMessage>();
        NetworkServer.SendToAll((short)RBCustomMsgTypes.RBPlayerMovementMessage, _msg);
    }

    private void OnRecievePlayerPhysicsMessage(NetworkMessage _message)
    {
        RBPlayerPhysicsMessage _msg = _message.ReadMessage<RBPlayerPhysicsMessage>();
        NetworkServer.SendToAll((short)RBCustomMsgTypes.RBPlayerPhysicsMessage, _msg);
    }

}
