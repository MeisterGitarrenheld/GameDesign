using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPowerupObject : NetworkBehaviour
{
    /// <summary>
    /// Detects when something hits the powerup.
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter(Collider collision)
    {
        var hitTag = collision.gameObject.tag;

        if (hitTag == "Player")
        { // The player walked through the powerup.
            var playerInfo = collision.gameObject.GetComponentInParent<RBCharacter>().PlayerInfo;

            if (playerInfo.IsLocalUser)
            {
                RBPowerupActivityControl.Instance.CollectPowerup(gameObject.GetComponent<RBPowerupBaseStatHolder>().BaseStats);
            }

            if (isServer)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
        else if (hitTag == "Ball")
        { // Someone has shot the ball on the powerup.
          // TODO determine who was the last player that hit the ball.

            RBBall ball = collision.gameObject.GetComponent<RBBall>();

            RBGameEventMessage msg = new RBGameEventMessage()
            {
                TriggeredEventType = GameEvent.PowerUpCollected,
                TriggeredPlayerID = ball.Player_LastHitID,
                TriggeredTeamID = ball.Team_LastHit,
                GameEventInfo = transform.name
            };
            NetworkManager.singleton.client.Send((short)RBCustomMsgTypes.RBGameEventMessage, msg);

            if (isServer)
            {
                NetworkServer.Destroy(gameObject);
            }

        }
    }
}
