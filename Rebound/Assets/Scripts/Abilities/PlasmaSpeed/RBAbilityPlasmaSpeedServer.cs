using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBAbilityPlasmaSpeedServer : NetworkBehaviour
{
    [SerializeField]
    private GameObject _plasmaVisualEffect;
    private GameObject _spawnedEffect;

    public void SetSpeedEffect(bool active)
    {
        CmdSetSpeedEffect(RBMatch.Instance.GetLocalUser().Username, active);
    }

    [Command]
    void CmdSetSpeedEffect(string playerName, bool active)
    {
        RpcSetSpeedEffect(playerName, active);
    }

    [ClientRpc]
    void RpcSetSpeedEffect(string playerName, bool active)
    {
        var playerObject = gameObject.FindPlayerByName(playerName);
        var shieldScript = playerObject.GetComponentInChildren<RBShieldScript>();

        if (active)
        {
            _spawnedEffect = Instantiate(_plasmaVisualEffect);
            _spawnedEffect.transform.parent = shieldScript.gameObject.transform;
            _spawnedEffect.transform.localPosition = Vector3.zero;
            _spawnedEffect.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _spawnedEffect.transform.localScale = Vector3.one;
        }
        else
        {
            Destroy(_spawnedEffect);
        }
    }
}
