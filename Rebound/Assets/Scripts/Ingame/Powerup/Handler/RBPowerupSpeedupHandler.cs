using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPowerupSpeedupHandler : ARBPowerupActionHandler
{
    public float SpeedDuration;

    private RBPowerupSpeedupHandlerServer _serverHandler;

    /// <summary>
    /// Increases the player speed for the player that triggered the powerup.
    /// </summary>
    /// <param name="playerName"></param>
    public override void DoAction(string playerName)
    {
        var playerObject = gameObject.FindPlayerByName(playerName);
        _serverHandler = playerObject.GetComponent<RBPowerupSpeedupHandlerServer>();

        var playerController = playerObject.GetComponent<RBPlayerController>();
        playerController.SpeedBoost(3.0f, SpeedDuration);

        _serverHandler.ShowSpeedEffect();

        TriggerOnComplete();
    }
}
