using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class ARBPowerupActionHandler : NetworkBehaviour
{
    /// <summary>
    /// This action is called when the powerup is executed.
    /// </summary>
    /// <param name="playerName"></param>
    public abstract void DoAction(string playerName);
}
