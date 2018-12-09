using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class ARBArenaSetup : NetworkBehaviour
{
    public GameObject LocalPlayer = null;

    protected virtual void Awake()
    {
        FindLocalPlayer();
        SetupCamera();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    private void FindLocalPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
            if (player.GetComponent<NetworkBehaviour>().isLocalPlayer)
            {
                LocalPlayer = player;
                LocalPlayer.GetComponent<Rigidbody>().useGravity = true;
                LocalPlayer.GetComponent<RBPlayerMovement>().enabled = true;
            }
    }

    private void SetupCamera()
    {
        var cam = Camera.main;

        cam.transform.parent = LocalPlayer.transform;
        cam.transform.localPosition = new Vector3(0, 5, -10);
        cam.transform.LookAt(LocalPlayer.transform);
        cam.transform.localPosition = new Vector3(0, 9, -10);
    }
}
