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

    public void TryThrowMine(Ray targetRay, Vector3 spawnPosition)
    {
        CmdTryThrowMine(targetRay.origin, targetRay.direction, spawnPosition);
    }

    [Command]
    void CmdTryThrowMine(Vector3 aimOrigin, Vector3 aimDirection, Vector3 spawnPosition)
    {
        // try to hit something with a raycast
        RaycastHit hitInfo;
        if (Physics.Raycast(aimOrigin, aimDirection, out hitInfo))
        {
            _aimTarget = hitInfo;
            AimTargetFound = true;

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
        CmdExplodeMine(stopCoroutine);
    }

    [Command]
    void CmdExplodeMine(bool stopCoroutine)
    {
        MineDestroyed = true;
        if (_mine == null)
            return;

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

        var explosionForce = 40.0f;
        var explosionRadius = 15.0f;
        var objectsInHitRange = Physics.OverlapSphere(_mine.transform.position, explosionRadius);

        foreach (var obj in objectsInHitRange)
        {
            switch (obj.tag)
            {
                case "Ball":
                    obj.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, _mine.transform.position, explosionRadius, 1.0f, ForceMode.Impulse);
                    break;
                case "Player":
                    if (obj.gameObject.name == "Shield")
                        break;

                    var connVtr = obj.transform.position - _mine.transform.position;
                    var distance = connVtr.magnitude;
                    var distFactor = (explosionRadius - Mathf.Min(connVtr.magnitude, explosionRadius)) / explosionRadius;
                    var dir = connVtr.normalized;
                    var forceVtr = dir * distFactor * explosionForce;

                    var playerObject = obj.gameObject;
                    while (playerObject.transform.parent != null)
                        playerObject = playerObject.transform.parent.gameObject;

                    var nwIdentity = playerObject.GetComponent<NetworkIdentity>();

                    TargetApplyPlayerExplosionForce(nwIdentity.connectionToClient, forceVtr * 3);
                    break;
            }
        }
    }

    [TargetRpc]
    void TargetApplyPlayerExplosionForce(NetworkConnection conn, Vector3 forceVtr)
    {
        RBPlayerMovement.Instance.ApplyExternalForce(forceVtr, .3f);
    }

    public void Reset()
    {
        CmdReset();
    }

    [Command]
    void CmdReset()
    {
        _mine = null;
        _targetTransformStartPos = Vector3.zero;
        TargetPosition = Vector3.zero;
        TargetReached = false;
        MineDestroyed = false;
        AimTargetFound = false;

        if (_explosionTimer != null)
            StopCoroutine(_explosionTimer);
        _explosionTimer = null;
    }
}
