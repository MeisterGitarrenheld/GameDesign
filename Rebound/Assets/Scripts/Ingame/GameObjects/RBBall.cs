using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBBall : NetworkBehaviour
{
    public static Transform BallTransformInstance;

    public string LastHitPlayerName = null;
    public int Team_LastHit;
    public bool IceEffectActive = false;
    public string IceEffectSourceUsername = null;

    public GameObject[] GoalEffects;

    [SyncVar]
    public bool InGoal;

    void Awake()
    {
        BallTransformInstance = transform;
    }

    void Update()
    {
        if (Vector3.Distance(gameObject.transform.position, Vector3.zero) > 300)
            RBNetworkGameManager.Instance.RespawnBall(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        //if (other.tag == "Goal" && !InGoal)
            

        if (!isServer)
            return;

        if (other.tag == "Goal" && !InGoal)
        {
            RpcSpawnGoalEffect(other.transform.position, other.GetComponent<RBGoal>().OwningTeamID);
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

    [ClientRpc]
    private void RpcSpawnGoalEffect(Vector3 goalPos, int owningTeamId)
    {
        Vector3 collPoint = Vector3.zero;

        if (goalPos.z == 0)
            collPoint = new Vector3(goalPos.x, gameObject.transform.position.y, gameObject.transform.position.z);
        else if (goalPos.x == 0)
            collPoint = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, goalPos.z);

        GameObject obj = null;

        switch (owningTeamId)
        {
            case 1:
                obj = Instantiate(GoalEffects[0]);
                break;
            case 2:
                obj = Instantiate(GoalEffects[1]);
                break;
        }

        obj.transform.position = collPoint;
        obj.transform.rotation = Quaternion.Euler(0, goalPos.x == 0 ? 0 : 90, 0);
    }
}
