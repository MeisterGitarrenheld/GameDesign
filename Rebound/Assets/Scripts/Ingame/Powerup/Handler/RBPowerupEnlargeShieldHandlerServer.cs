using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPowerupEnlargeShieldHandlerServer : NetworkBehaviour
{
    private Transform _shieldTransform;

    [SyncVar]
    public Vector3 ShieldScale = new Vector3(1, 1, 1);

    public float ShieldRescaleSpeed = 1.5f;
    public Vector3 ShieldDefaultScale = new Vector3(1, 1, 1);
    public Vector3 ShieldLargeScale = new Vector3(1.3f, 1.3f, 1.3f);

    private float _shieldTotalRescaleDuration = 3.0f;
    private float _shieldRescaleDuration = 0.0f;

    private float _minScaleMagnitude;
    private float _maxScaleMagnitude;

    [SyncVar]
    private float _targetScaleMagnitude;

    void Start()
    {
        _shieldTransform = gameObject.transform.Find("Shield");

        _minScaleMagnitude = ShieldDefaultScale.magnitude;
        _maxScaleMagnitude = ShieldLargeScale.magnitude;
    }

    /// <summary>
    /// Tracks the rescale timer on the server and adjusts the scale on every client.
    /// </summary>
    void Update()
    {
        if (isServer)
            UpdateShieldRescaleTimer();

        RescaleShieldIncremental();
    }

    /// <summary>
    /// Checks whether the maximum rescale duration has been reached.
    /// </summary>
    void UpdateShieldRescaleTimer()
    {
        if (_shieldRescaleDuration <= _shieldTotalRescaleDuration)
        {
            _shieldRescaleDuration += Time.deltaTime;

            if (_shieldRescaleDuration >= _shieldTotalRescaleDuration)
            {
                ShieldScale = ShieldDefaultScale;
                _targetScaleMagnitude = ShieldScale.magnitude;
            }
        }
    }

    /// <summary>
    /// Enlarges or shrinks the shield based on the passed time and the target scale.
    /// </summary>
    void RescaleShieldIncremental()
    {
        var curScaleMagnitude = _shieldTransform.localScale.magnitude;

        var isGrowing = _targetScaleMagnitude == _maxScaleMagnitude && _targetScaleMagnitude > curScaleMagnitude;
        var isShrinking = _targetScaleMagnitude == _minScaleMagnitude && _targetScaleMagnitude < curScaleMagnitude;

        if (isGrowing)
        {
            var newShieldScale = _shieldTransform.localScale + ShieldDefaultScale * ShieldRescaleSpeed * Time.deltaTime;

            if (newShieldScale.magnitude > _targetScaleMagnitude)
                newShieldScale = ShieldScale;

            _shieldTransform.localScale = newShieldScale;
        }
        else if (isShrinking)
        {
            var newShieldScale = _shieldTransform.localScale - ShieldDefaultScale * ShieldRescaleSpeed * Time.deltaTime;

            if (newShieldScale.magnitude < _targetScaleMagnitude)
                newShieldScale = ShieldScale;

            _shieldTransform.localScale = newShieldScale;
        }
    }

    /// <summary>
    /// Tells the server to set the trigger for the rescale of the shield.
    /// </summary>
    public void EnlargeShield()
    {
        CmdEnlargeShield();
    }

    /// <summary>
    /// Sets the trigger for shield rescaling for all clients
    /// and starts tracking the duration.
    /// </summary>
    [Command]
    void CmdEnlargeShield()
    {
        ShieldScale = ShieldLargeScale;
        _targetScaleMagnitude = ShieldScale.magnitude;
        _shieldRescaleDuration = 0.0f;
    }
}
