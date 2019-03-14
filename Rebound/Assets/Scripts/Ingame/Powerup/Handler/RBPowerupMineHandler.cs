using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class RBPowerupMineHandler : ARBPowerupActionHandler
{
    public GameObject MineCanvasPrefab;
    
    public KeyCode ThrowKey = KeyCode.LeftShift;
    
    private GameObject _mineCanvas = null;

    private RBPowerupMineHandlerServer _serverHandler;


    public override void DoAction(string playerName)
    {
        // create the crosshair and let the player select the target
        _mineCanvas = Instantiate(MineCanvasPrefab);
    }
        
    void Start()
    {
        var playerObject = gameObject.FindPlayerByName(RBMatch.Instance.GetLocalUser().Username);
        _serverHandler = playerObject.GetComponent<RBPowerupMineHandlerServer>();

        //gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(playerObject.GetComponent<NetworkIdentity>().connectionToClient);
    }
    
    void Update()
    {
        if (_mineCanvas != null)
        {
            // the player is aiming
            if (Input.GetKeyDown(ThrowKey))
            {
                var targetRay = GetTargetRay();

                // aiming complete
                if (_serverHandler.TryGetAimTarget(targetRay))
                {
                    // remove the crosshair
                    Destroy(_mineCanvas.gameObject);

                    // throw the mine
                    //ThrowMine();
                }
            }
        }
        /*
        else if (!_targetReached)
        {
            UpdateMineFlightDirection();
            UpdateMineMovement();
        }
        */
    }

    Ray GetTargetRay()
    {
        // get target
        var crossHair = _mineCanvas.GetComponent<RectTransform>().Find("Crosshair").GetComponent<RectTransform>();

        // the anchor is the center of the crosshair
        var xAnchor = crossHair.anchorMin.x;
        var yAnchor = crossHair.anchorMin.y;

        // get the anchor position in screen space
        var xScreen = Screen.width * xAnchor;
        var yScreen = Screen.height * yAnchor;

        // get the ray for the crosshair point
        return Camera.main.ScreenPointToRay(new Vector3(xScreen, yScreen, 0));
    }


    /*
    void ThrowMine()
    {
        Debug.Log("ThrowMine");
        var throwAnchor = GetMineSpawnPosition();

        CmdThrowMine(throwAnchor.position);
    }

    [Command]
    void CmdThrowMine(Vector3 spawnPosition)
    {
        Debug.Log("CmdThrowMine");
        // spawn the mine
        _mine = Instantiate(MinePrefab, spawnPosition, MinePrefab.transform.rotation);

        _targetTransformStartPos = _aimTarget.transform.position;
        _targetPosition = _aimTarget.point;

        RBColliderListener colliderListener = new RBColliderListener();
        colliderListener.For(_mine);
        colliderListener.OnCollisionEnterAction = OnCollisionEnter;

        RpcThrowMine(spawnPosition);
    }

    [ClientRpc]
    void RpcThrowMine(Vector3 spawnPosition)
    {
        Debug.Log("RpcThrowMine");
        // spawn the mine
        _mine = Instantiate(MinePrefab, spawnPosition, MinePrefab.transform.rotation);
    }
    
    void UpdateMineFlightDirection()
    {
        // if the target has not been destroyed
        if (_aimTarget.transform != null)
        {
            // if we have a moving target, we have to adjust the flight direction to ensure the hit
            if (_targetTransformStartPos != _aimTarget.transform.position)
            {
                _targetPosition = _aimTarget.transform.position;
            }
        }

        Debug.DrawLine(GetMineSpawnPosition().position, _targetPosition, Color.blue, .1f);
    }

    void UpdateMineMovement()
    {
        var flightDirection = (_targetPosition - _mine.transform.position).normalized;

        if (flightDirection.magnitude > 0)
        {
            var rb = _mine.GetComponent<Rigidbody>();
            rb.velocity = flightDirection * MineFlightSpeed;
        }
    }

    Transform GetMineSpawnPosition()
    {
        var localPlayerName = RBMatch.Instance.GetLocalUser().Username;
        var localPlayerObject = gameObject.FindPlayerByName(localPlayerName);
        var throwAnchor = localPlayerObject.transform.Find("ThrowAnchor");
        return throwAnchor.transform;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit " + collision.gameObject.name);

        if (collision.gameObject.name == "Shield") return;

        _targetReached = true;

        var tmpMine = Instantiate(MineAttachedPrefab, collision.contacts[0].point, MineAttachedPrefab.transform.rotation);

        tmpMine.transform.parent = collision.transform;

        Destroy(_mine);

        _mine = tmpMine;
    }
    */
}
