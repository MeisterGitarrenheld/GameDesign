using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPowerupMineHandlerServer : NetworkBehaviour
{
    public GameObject MinePrefab;
    public GameObject MineAttachedPrefab;

    public float MineFlightSpeed = 50.0f;

    private GameObject _mine = null;
    private RaycastHit _aimTarget;

    private Vector3 _targetTransformStartPos;

    private Vector3 _targetPosition;

    private bool _targetReached = false;

    [SyncVar]
    public bool AimTargetFound = false;

    public bool TryGetAimTarget(Ray targetRay)
    {
        CmdTryGetAimTarget(targetRay);
        return AimTargetFound;
    }

    [Command]
    void CmdTryGetAimTarget(Ray targetRay)
    {
        // try to hit something with a raycast
        RaycastHit hitInfo;
        if (Physics.Raycast(targetRay, out hitInfo))
        {
            _aimTarget = hitInfo;
            AimTargetFound = true;
        }
    }
}
