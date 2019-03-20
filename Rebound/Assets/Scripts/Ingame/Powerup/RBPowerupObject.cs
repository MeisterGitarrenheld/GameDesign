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
            var localPlayerName = RBMatch.Instance.GetLocalUser().Username;

            RBBall ball = collision.gameObject.GetComponent<RBBall>();

            if (localPlayerName == ball.LastHitPlayerName)
            {
                RBPowerupActivityControl.Instance.CollectPowerup(gameObject.GetComponent<RBPowerupBaseStatHolder>().BaseStats);
            }

            if (isServer)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
