using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBBall : NetworkBehaviour
{
    public static Transform BallTransformInstance;

    public string LastHitPlayerName = null;
    public int Team_LastHit;

    [SyncVar]
    public bool InGoal;

    void Awake()
    {
        BallTransformInstance = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.tag == "Goal" && !InGoal)
        {
            RBNetworkGameManager.Instance.Goal(gameObject, other.GetComponent<RBGoal>().OwningTeamID);

            RBGameEventMessage msg = new RBGameEventMessage()
            {
                TriggeredEventType = GameEvent.Goal,
                TriggeredPlayerName = LastHitPlayerName,
                TriggeredTeamID = Team_LastHit,
                GameEventInfo = other.GetComponent<RBGoal>().OwningTeamID.ToString()
            };
            NetworkManager.singleton.client.Send((short)RBCustomMsgTypes.RBGameEventMessage, msg);
            InGoal = true;
        }
    }
}
