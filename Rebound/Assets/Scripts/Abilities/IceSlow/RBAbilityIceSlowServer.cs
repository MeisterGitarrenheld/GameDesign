using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBAbilityIceSlowServer : NetworkBehaviour
{
    [SerializeField]
    private GameObject _shieldIceEffectPrefab;
    private GameObject _shieldIceEffectObject;

    [SerializeField]
    private GameObject _ballIceEffectPrefab;
    private GameObject _ballIceEffectObject;


    public void SetIceEffectOnLocalPlayer(bool active)
    {
        CmdSetIceEffectOnLocalPlayer(RBMatch.Instance.GetLocalUser().Username, active);
    }

    [Command]
    void CmdSetIceEffectOnLocalPlayer(string playerName, bool active)
    {
        RpcSetIceEffectOnLocalPlayer(playerName, active);
    }

    [ClientRpc]
    void RpcSetIceEffectOnLocalPlayer(string playerName, bool active)
    {
        var playerObject = gameObject.FindPlayerByName(playerName);
        var shieldScript = playerObject.GetComponentInChildren<RBShieldScript>();

        if (active)
        {
            // show ice effect on shield
            if (_shieldIceEffectObject == null)
            {
                _shieldIceEffectObject = Instantiate(_shieldIceEffectPrefab);
                _shieldIceEffectObject.transform.parent = shieldScript.gameObject.transform;
                _shieldIceEffectObject.transform.localPosition = Vector3.zero;
                _shieldIceEffectObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                _shieldIceEffectObject.transform.localScale = Vector3.one;
            }
        }
        else
        {
            // hide ice effect on shield
            if (_shieldIceEffectObject != null)
            {
                Destroy(_shieldIceEffectObject);
                _shieldIceEffectObject = null;
            }
        }
    }

    public void SetIceEffectOnBall(bool active)
    {
        CmdSetIceEffectOnBall(RBMatch.Instance.GetLocalUser().Username, active);
    }

    [Command]
    void CmdSetIceEffectOnBall(string sourcePlayer, bool active)
    {
        RpcSetIceEffectOnBall(sourcePlayer, active);
    }

    [ClientRpc]
    void RpcSetIceEffectOnBall(string sourcePlayer, bool active)
    {
        var ballObject = RBBall.BallTransformInstance.gameObject;
        var ballScript = ballObject.GetComponent<RBBall>();

        if (active)
        {
            ballScript.IceEffectActive = true;
            ballScript.IceEffectSourceUsername = sourcePlayer;

            // apply ice style to ball
            if (_ballIceEffectObject == null)
            {
                _ballIceEffectObject = Instantiate(_ballIceEffectPrefab);
                _ballIceEffectObject.transform.parent = ballObject.transform;
                _ballIceEffectObject.transform.localPosition = Vector3.zero;
                _ballIceEffectObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                _ballIceEffectObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            ballScript.IceEffectActive = false;
            ballScript.IceEffectSourceUsername = null;

            // remove ice style from ball
            if (_ballIceEffectObject != null)
            {
                Destroy(_ballIceEffectObject);
                _ballIceEffectObject = null;
            }
        }
    }
}
