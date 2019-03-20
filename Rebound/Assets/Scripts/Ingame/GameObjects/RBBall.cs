using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBBall : NetworkBehaviour
{
    public string LastHitPlayerName = null;
    public int Team_LastHit;

    [SyncVar]
    private bool _inGoal;

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.tag == "Goal" && !_inGoal)
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
            _inGoal = true;
        }
    }
}
