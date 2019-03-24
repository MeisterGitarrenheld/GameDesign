using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBAbilityPlasmaSpeed : ARBAbility
{
    public int Duration = 10;

    private bool _active;

    private GameObject _playerObject;

    private RBAbilityPlasmaSpeedServer _serverHandler;
    private RBShieldScript _shieldScript;

    protected override void OnTrigger()
    {
        if (!_active)
        {
            _active = PauseCooldown = true;

            if (_playerObject == null)
                _playerObject = gameObject.FindPlayerByName(RBMatch.Instance.GetLocalUser().Username);

            if (_serverHandler == null)
                _serverHandler = _playerObject.GetComponent<RBAbilityPlasmaSpeedServer>();

            if (_shieldScript == null)
                _shieldScript = _playerObject.GetComponentInChildren<RBShieldScript>();


            _serverHandler.SetSpeedEffect(true);
            _shieldScript.PlasmaSpeedEffectActive = true;

            StartCoroutine(WaitUnitlEnd());
        }
    }

    IEnumerator WaitUnitlEnd()
    {
        yield return new WaitForSeconds(Duration);

        _active = PauseCooldown = false;

        _shieldScript.PlasmaSpeedEffectActive = false;
        _serverHandler.SetSpeedEffect(false);
    }
}
