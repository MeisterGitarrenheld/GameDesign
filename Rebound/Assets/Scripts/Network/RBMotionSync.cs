using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBMotionSync : NetworkBehaviour
{
    [SyncVar]
    private Vector3 _syncPos;

    [SyncVar]
    private Vector3 _syncRot;

    private Vector3 _lastPos;
    private Quaternion _lastRot;
    private Transform _myTransform;

    [SerializeField]
    private float _lerpRate = 10;

    [SerializeField]
    private float _posThreshold = 0.5f;

    [SerializeField]
    private float _rotThreshold = 0.0f;

    [SerializeField]
    private float _snapThreshold = 1.0f;

    void Start()
    {
        _myTransform = transform;
    }

    void Update()
    {
        TransmitMotion();
        LerpMotion();
    }

    void TransmitMotion()
    {
        if (!isServer)
        {
            return;
        }

        if (Vector3.Distance(_myTransform.position, _lastPos) > _posThreshold || Quaternion.Angle(_myTransform.rotation, _lastRot) > _rotThreshold)
        {
            _lastPos = _myTransform.position;
            _lastRot = _myTransform.rotation;

            _syncPos = _myTransform.position;
            _syncRot = _myTransform.localEulerAngles;
        }
    }

    void LerpMotion()
    {
        if (isServer)
        {
            return;
        }

        if (Vector3.Distance(_myTransform.position, _syncPos) > _snapThreshold)
        {
            _myTransform.position = _syncPos;
            _myTransform.rotation = Quaternion.Euler(_syncRot);
        }
        else
        {
            _myTransform.position = Vector3.Lerp(_myTransform.position, _syncPos, Time.deltaTime * _lerpRate);
            _myTransform.rotation = Quaternion.Lerp(_myTransform.rotation, Quaternion.Euler(_syncRot), Time.deltaTime * _lerpRate);
        }
    }


}
