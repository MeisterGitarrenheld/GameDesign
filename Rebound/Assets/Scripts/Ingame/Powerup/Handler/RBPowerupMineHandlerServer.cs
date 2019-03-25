using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RBPowerupMineHandlerServer : NetworkBehaviour
{
    public GameObject MinePrefab;
    public GameObject MineAttachedPrefab;

    public float MineFlightSpeed = 50.0f;
    public float MineLifeTime = 5.0f;

    private GameObject _mine = null;
    private Transform _aimTarget;

    private Vector3 _targetTransformStartPos;

    private Vector3 _lastFlightDirection = Vector3.forward;

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

    public void TryThrowMine(Ray targetRay)
    {
        CmdTryThrowMine(targetRay.origin, targetRay.direction, RBMatch.Instance.GetLocalUser().Username);
    }

    [Command]
    void CmdTryThrowMine(Vector3 aimOrigin, Vector3 aimDirection, string sourceUsername)
    {
        // try to hit something with a raycast
        RaycastHit[] hitInfos = Physics.RaycastAll(aimOrigin, aimDirection, 500.0f);
        if (hitInfos.Length > 0)
        {
            var spawnPosition = aimOrigin;
            var hitInfoList = hitInfos.ToList();
            foreach (var hitInfo in hitInfos)
            {
                var targetObject = hitInfo.transform.gameObject;
                /*
                Debug.Log("Hit: " +
                    targetObject.name + ", " +
                    targetObject.tag + ", " +
                    LayerMask.LayerToName(targetObject.layer));
                */
                if (targetObject.tag == "Player")
                {
                    var targetPlayer = targetObject.FindComponentInObjectOrParent<RBCharacter>().PlayerInfo;

                    if (sourceUsername == targetPlayer.Username)
                    {
                        hitInfoList.Remove(hitInfo);

                        if (hitInfo.collider.transform.name == "ThrowArea")
                            spawnPosition = hitInfo.point;
                    }
                }
            }

            if (hitInfoList.Count > 0)
            {
                var tmpTarget = hitInfoList.Last();

                //Debug.Log("Mine target is: " + tmpTarget.transform.gameObject.name);

                _aimTarget = tmpTarget.transform;
                _lastFlightDirection = aimDirection;
                AimTargetFound = true;

                // spawn the mine
                _mine = Instantiate(MinePrefab, spawnPosition, MinePrefab.transform.rotation);
                NetworkServer.Spawn(_mine);

                _targetTransformStartPos = tmpTarget.transform.position;
                TargetPosition = tmpTarget.point;

                RBColliderListener colliderListener = new RBColliderListener();
                colliderListener.For(_mine);
                colliderListener.OnCollisionEnterAction = OnMineCollisionEnter;
            }
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
        if (_aimTarget != null)
        {
            // if we have a moving target, we have to adjust the flight direction to ensure the hit
            if (_targetTransformStartPos != _aimTarget.position)
            {
                TargetPosition = _aimTarget.position;
            }
        }
    }

    void UpdateMineMovement()
    {
        Vector3 flightDirection;

        if (_aimTarget == null)
        {   // target destroyed, simply move in the last valid direction
            flightDirection = _lastFlightDirection;
        }
        else
        {   // target still exists, move towards the target
            flightDirection = (TargetPosition - _mine.transform.position).normalized;


            if (flightDirection.magnitude == 0.0f)
            {   // target reached, start moving in the last valid direction
                _aimTarget = null;
                flightDirection = _lastFlightDirection;
            }
        }

        var rb = _mine.GetComponent<Rigidbody>();
        rb.velocity = flightDirection * MineFlightSpeed;
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
        _lastFlightDirection = Vector3.forward;
        TargetPosition = Vector3.zero;
        TargetReached = false;
        MineDestroyed = false;
        AimTargetFound = false;

        if (_explosionTimer != null)
            StopCoroutine(_explosionTimer);
        _explosionTimer = null;
    }
}
