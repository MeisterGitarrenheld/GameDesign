using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBBall : NetworkBehaviour {


    public int Player_LastHitID;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Goal")
        {
            RBNetworkGameManager.Instance.Goal(gameObject, other.GetComponent<RBGoal>().OwningTeamID);

            RBGameEventMessage msg = new RBGameEventMessage()
            {
                TriggeredEventType = GameEvent.Goal
            };
            NetworkManager.singleton.client.Send((short)RBCustomMsgTypes.RBGameEventMessage, msg);

        }
    }

}
