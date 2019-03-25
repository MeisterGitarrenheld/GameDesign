using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class RBNetworkGameManager : NetworkBehaviour
{
    public static RBNetworkGameManager Instance;

    private ARBArenaSetup _arenaSetup;
    private Coroutine _coroutine;

    RBAudience[] audience;

    private void Start()
    {
        Instance = this;

        if (isServer)
        {
            RegisterHostNetworkMessages();
        }

        _arenaSetup = GameObject.Find("GameStateController").GetComponent<ARBArenaSetup>();
        //RespawnBall();

        audience = FindObjectsOfType<RBAudience>();

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


        if (_coroutine == null)
            _coroutine = StartCoroutine(GoalWorker(Ball));
    }

    IEnumerator GoalWorker(GameObject ball)
    {
        yield return new WaitForSeconds(3);

        RespawnBall(ball);

        yield return null;
        _coroutine = null;
    }

    public void RespawnBall(GameObject ball = null)
    {
        if (isServer)
        {
            if(ball != null)
                NetworkServer.Destroy(ball);

            var BallObject = Instantiate(_arenaSetup.BallPrefab, _arenaSetup.BallStartPosition.position, _arenaSetup.BallStartPosition.rotation);
            BallObject.GetComponent<Rigidbody>().velocity =
                new Vector3(
                    UnityEngine.Random.Range(-25, 25),
                    0,
                    UnityEngine.Random.Range(-25, 25));
            NetworkServer.Spawn(BallObject);
        }
    }

    private void RegisterHostNetworkMessages()
    {
        NetworkServer.RegisterHandler((short)RBCustomMsgTypes.RBPlayerMovementMessage, OnReceivePlayerMovementMessage);
        NetworkServer.RegisterHandler((short)RBCustomMsgTypes.RBPlayerPhysicsMessage, OnRecievePlayerPhysicsMessage);
        NetworkServer.RegisterHandler((short)RBCustomMsgTypes.RBGameEventMessage, OnHostRecieveGameEventMessage);

    }

    private void OnHostRecieveGameEventMessage(NetworkMessage _message)
    {
        RBGameEventMessage _msg = _message.ReadMessage<RBGameEventMessage>();
        switch (_msg.TriggeredEventType)
        {
            case GameEvent.GameOver: break;
            case GameEvent.Goal:
                print("Goal by: " + _msg.TriggeredPlayerName + " for Team: " + _msg.TriggeredTeamID + " in Goal of Team: " + _msg.GameEventInfo);
                NetworkServer.SendToAll((short)RBCustomMsgTypes.RBGameEventMessage, _msg);
                Array.ForEach(audience, aud => { aud.Jump = true; aud.MinJumpTime = 0.1f; });
                break;
            case GameEvent.PowerUpCollected:
                print("Ball Collected Powerup " + _msg.GameEventInfo + " for Player: " + _msg.TriggeredPlayerName + " in Team: " + _msg.TriggeredTeamID);
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
