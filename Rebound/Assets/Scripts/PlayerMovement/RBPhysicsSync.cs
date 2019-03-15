using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPhysicsSync : NetworkBehaviour {

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (isServer)
            RegisterNetworkMessages();
    }

    private void RegisterNetworkMessages()
    {
        NetworkManager.singleton.client.RegisterHandler((short)RBCustomMsgTypes.RBPlayerPhysicsMessage, OnReceivePhysicsMessage);
    }

    private void OnReceivePhysicsMessage(NetworkMessage _message)
    {
        RBPlayerPhysicsMessage _msg = _message.ReadMessage<RBPlayerPhysicsMessage>();
        GetComponent<RBBall>().Player_LastHitID = _msg.PlayerHitID;
        GetComponent<RBBall>().Team_LastHit = _msg.PlayerTeamID;
        if (rb.velocity.magnitude < 0.1f)
            rb.velocity = _msg.objectHitDirection.normalized * 10;
        else
        {
            if (rb.velocity.magnitude < 10)
                rb.velocity = rb.velocity.normalized * 10;
            if (Vector3.Angle(rb.velocity, _msg.objectHitDirection) < 90)
            {
                rb.velocity = rb.velocity + _msg.objectHitDirection;
            }
            else
            {
                rb.velocity = Vector3.Reflect(rb.velocity, _msg.objectHitDirection).normalized *
                    (rb.velocity.magnitude + _msg.objectHitDirection.magnitude);
            }
        }

    }

}
