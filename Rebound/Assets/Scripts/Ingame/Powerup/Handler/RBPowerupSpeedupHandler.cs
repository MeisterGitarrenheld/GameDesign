using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPowerupSpeedupHandler : ARBPowerupActionHandler
{
    /// <summary>
    /// Increases the player speed for the player that triggered the powerup.
    /// </summary>
    /// <param name="playerName"></param>
    public override void DoAction(string playerName)
    {
        Debug.Log("Doing the action for " + playerName);

        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            var character = player.GetComponent<RBCharacter>();
            var rbPlayer = character.PlayerInfo;

            if (rbPlayer.Username == playerName)
            {
                var movementScript = player.GetComponent<RBPlayerMovement>();
                movementScript.MaxMoveSpeed = 100;
                break;
            }
        }

        Destroy(gameObject);
    }
}
