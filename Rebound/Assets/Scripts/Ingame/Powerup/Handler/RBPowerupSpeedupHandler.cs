using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPowerupSpeedupHandler : ARBPowerupActionHandler
{
    public ParticleSystem SpeedEffect;
    public float SpeedDuration;

    private Coroutine _coroutine;

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

            if (rbPlayer.Username == playerName && _coroutine == null)
            {
                // Instantiate the Speed effect
                var psEffect = Instantiate(SpeedEffect, player.gameObject.transform);
                psEffect.transform.localPosition = new Vector3(0, 2.5f, 0);

                var playerController = player.GetComponent<RBPlayerController>();
                playerController.SpeedBoost(3.0f, SpeedDuration);
                
                break;
            }
        }

        Destroy(gameObject);
    }
}
