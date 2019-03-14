using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class ARBPowerupActionHandler : NetworkBehaviour
{
    /// <summary>
    /// Should be triggered when the power up action has been finished.
    /// </summary>
    public event Action OnComplete;

    /// <summary>
    /// This action is called when the powerup is executed.
    /// </summary>
    /// <param name="playerName"></param>
    public abstract void DoAction(string playerName);

    protected void TriggerOnComplete()
    {
        OnComplete?.Invoke();
    }
}
