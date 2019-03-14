using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPowerupMineHandlerServer : NetworkBehaviour
{
    public GameObject MinePrefab;
    public GameObject MineAttachedPrefab;

    public float MineFlightSpeed = 50.0f;
    public float MineLifeTime = 5.0f;

    private GameObject _mine = null;
    private RaycastHit _aimTarget;

    private Vector3 _targetTransformStartPos;

    [SyncVar]
    public Vector3 TargetPosition;

    [SyncVar]
    public bool TargetReached = false;

    [SyncVar]
    public bool MineDestroyed = false;

    [SyncVar]
    public bool AimTargetFound = false;

    private Coroutine _explosionTimer = null;

    public bool IsFlying { get { return _mine != null && !TargetReached; } }

    void Update()
    {
        if (!isServer) return;

        if (IsFlying)
        {
            UpdateMineFlightDirection();
            UpdateMineMovement();
        }
    }

    public bool TryGetAimTarget(Ray targetRay)
    {
        CmdTryGetAimTarget(targetRay.origin, targetRay.direction);
        return AimTargetFound;
    }

    [Command]
    void CmdTryGetAimTarget(Vector3 aimOrigin, Vector3 aimDirection)
    {
        // try to hit something with a raycast
        RaycastHit hitInfo;
        if (Physics.Raycast(aimOrigin, aimDirection, out hitInfo))
        {
            _aimTarget = hitInfo;
            AimTargetFound = true;
        }
    }

    public void ThrowMine(Vector3 spawnPosition)
    {
        CmdThrowMine(spawnPosition);
    }

    [Command]
    void CmdThrowMine(Vector3 spawnPosition)
    {
        Debug.Log("CmdThrowMine");
        // spawn the mine
        _mine = Instantiate(MinePrefab, spawnPosition, MinePrefab.transform.rotation);
        NetworkServer.Spawn(_mine);

        _targetTransformStartPos = _aimTarget.transform.position;
        TargetPosition = _aimTarget.point;

        RBColliderListener colliderListener = new RBColliderListener();
        colliderListener.For(_mine);
        colliderListener.OnCollisionEnterAction = OnMineCollisionEnter;
    }

    void OnMineCollisionEnter(Collision collision)
    {
        if (!isServer) return;

        var transform = collision.transform;
        var rbCharacter = transform.GetComponent<RBCharacter>();

        while (rbCharacter == null && (transform = transform.parent) != null)
            rbCharacter = transform.GetComponent<RBCharacter>();

        var currentPlayerName = gameObject.GetComponent<RBCharacter>().PlayerInfo.Username;
        if (rbCharacter != null && rbCharacter.PlayerInfo.Username == currentPlayerName) return;


        TargetReached = true;

        var tmpMine = Instantiate(MineAttachedPrefab, collision.contacts[0].point, MineAttachedPrefab.transform.rotation);
        NetworkServer.Spawn(tmpMine);

        tmpMine.transform.parent = collision.transform;

        NetworkServer.Destroy(_mine);

        _mine = tmpMine;

        _explosionTimer = StartCoroutine(DestroyAndExplodeMine());
    }

    void UpdateMineFlightDirection()
    {
        // if the target has not been destroyed
        if (_aimTarget.transform != null)
        {
            // if we have a moving target, we have to adjust the flight direction to ensure the hit
            if (_targetTransformStartPos != _aimTarget.transform.position)
            {
                TargetPosition = _aimTarget.transform.position;
            }
        }
    }

    void UpdateMineMovement()
    {
        var flightDirection = (TargetPosition - _mine.transform.position).normalized;

        if (flightDirection.magnitude > 0)
        {
            var rb = _mine.GetComponent<Rigidbody>();
            rb.velocity = flightDirection * MineFlightSpeed;
        }
    }

    IEnumerator DestroyAndExplodeMine()
    {
        yield return new WaitForSeconds(MineLifeTime);

        ExplodeMine(false);
    }

    public void ExplodeMine(bool stopCoroutine = true)
    {
        if (_mine == null)
        {
            MineDestroyed = true;
            return;
        }
        MineDestroyed = true;
        CmdExplodeMine(stopCoroutine);
    }

    [Command]
    void CmdExplodeMine(bool stopCoroutine)
    {
        NetworkServer.Destroy(_mine);
        TriggerBlastWave();

        if (stopCoroutine)
        {
            StopCoroutine(_explosionTimer);
        }
    }

    void TriggerBlastWave()
    {
        Debug.Log("Trigger blast wave");
    }

    public void Reset()
    {
        _mine = null;
        _targetTransformStartPos = Vector3.zero;
        TargetPosition = Vector3.zero;
        TargetReached = false;
        MineDestroyed = false;
        AimTargetFound = false;
    }
}
