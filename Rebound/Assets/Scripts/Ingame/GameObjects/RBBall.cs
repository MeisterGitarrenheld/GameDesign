using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBBall : NetworkBehaviour
{
    public static Transform BallTransformInstance;

    public string LastHitPlayerName = null;
    public int Team_LastHit;

    public GameObject[] GoalEffects;

    [SyncVar]
    public bool InGoal;

    void Awake()
    {
        BallTransformInstance = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Goal" && !InGoal)
            SpawnGoalEffect(other);

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

    private void SpawnGoalEffect(Collider other)
    {
        Vector3 collPoint = Vector3.zero;

        if (other.transform.position.z == 0)
            collPoint = new Vector3(other.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        else if (other.transform.position.x == 0)
            collPoint = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, other.transform.position.z);

        var teamId = other.GetComponent<RBGoal>().OwningTeamID;
        GameObject obj = null;

        switch (teamId)
        {
            case 1:
                print("spawned effect 1");
                obj = Instantiate(GoalEffects[0]);
                break;
            case 2:
                print("spawned effect 2");
                obj = Instantiate(GoalEffects[1]);
                break;
        }

        obj.transform.position = collPoint;
        obj.transform.rotation = Quaternion.Euler(0, other.transform.position.x == 0 ? 0 : 90, 0);
    }
}
