using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class RBPowerupMineHandler : ARBPowerupActionHandler
{
    public GameObject MineCanvasPrefab;

    public KeyCode ActionKey = KeyCode.LeftShift;

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
            if(_serverHandler.AimTargetFound)
            {
                // remove the crosshair
                Destroy(_mineCanvas.gameObject);
            }
            // the player is aiming
            else if (Input.GetKeyDown(ActionKey))
            {
                var targetRay = GetTargetRay();

                // aiming complete try to throw
                _serverHandler.TryThrowMine(targetRay);
            }
        }
        else if (_serverHandler.TargetReached)
        {
            if (_serverHandler.MineDestroyed)
            {
                Debug.Log("Complete");
                _serverHandler.Reset();
                TriggerOnComplete();
            }
            else if (Input.GetKeyDown(ActionKey))
            {
                _serverHandler.ExplodeMine();
            }
        }
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
}
