using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBBall : NetworkBehaviour {


    public int Player_LastHitID;
    public int Team_LastHit;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Goal")
        {
            RBNetworkGameManager.Instance.Goal(gameObject, other.GetComponent<RBGoal>().OwningTeamID);

            RBGameEventMessage msg = new RBGameEventMessage()
            {
                TriggeredEventType = GameEvent.Goal,
                TriggeredPlayerID = Player_LastHitID,
                TriggeredTeamID = other.GetComponent<RBGoal>().OwningTeamID
            };
            NetworkManager.singleton.client.Send((short)RBCustomMsgTypes.RBGameEventMessage, msg);
        }
    }

}
