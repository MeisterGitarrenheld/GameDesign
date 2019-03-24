using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBAbilityIceSlow : ARBAbility
{
    private GameObject _playerObject;
    private RBAbilityIceSlowServer _serverHandler;
    private RBShieldScript _shieldScript;


    protected override void OnTrigger()
    {
        if (PauseCooldown) return;

        PauseCooldown = true;

        if (_playerObject == null)
            _playerObject = gameObject.FindPlayerByName(RBMatch.Instance.GetLocalUser().Username);

        if (_serverHandler == null)
            _serverHandler = _playerObject.GetComponent<RBAbilityIceSlowServer>();

        if (_shieldScript == null)
            _shieldScript = _playerObject.GetComponentInChildren<RBShieldScript>();

        _serverHandler.SetIceEffectOnLocalPlayer(true);

        _shieldScript.IceSlowEffectActive = true;
        _shieldScript.IceSlowAbility = this;
    }

    public void BallHitLocalShield()
    {
        _shieldScript.IceSlowEffectActive = false;
        _shieldScript.IceSlowAbility = null;

        _serverHandler.SetIceEffectOnLocalPlayer(false);
        _serverHandler.SetIceEffectOnBall(true);

        PauseCooldown = false;
    }
}
