using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPowerupSpeedupHandlerServer : NetworkBehaviour
{
    public ParticleSystem SpeedEffect;

    public void ShowSpeedEffect()
    {
        CmdShowSpeedEffect();
    }

    [Command]
    void CmdShowSpeedEffect()
    {
        RpcShowSpeedEffect();
    }

    [ClientRpc]
    void RpcShowSpeedEffect()
    {
        // Instantiate the Speed effect
        var psEffect = Instantiate(SpeedEffect, gameObject.transform);
        psEffect.transform.localPosition = new Vector3(0, 2.5f, 0);
    }
}
