using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class ARBArenaSetup : NetworkBehaviour
{
    public GameObject LocalPlayer = null;

    public Transform BallStartPosition;
    public Transform[] PlayerStartPositions;

    public GameObject BallPrefab;

    protected virtual void Awake()
    {
        SetupPlayers();
        SetupCamera();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    private void SetupPlayers()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");


        // attach the player info to the player objects
        List<RBCharacter> playerObjects = new List<RBCharacter>();
        playerObjects.AddRange(players.Select(x => x.GetComponent<RBCharacter>()));

        List<RBPlayer> playerScripts = new List<RBPlayer>();
        playerScripts.AddRange(RBMatch.Instance.Players);

        RBCharacter localPlayerObject = playerObjects.Find(x => x.gameObject.GetComponent<NetworkBehaviour>().isLocalPlayer);
        playerObjects.Remove(localPlayerObject);

        RBPlayer localPlayerScript = playerScripts.Find(x => x.IsLocalUser);
        playerScripts.Remove(localPlayerScript);

        localPlayerObject.PlayerInfo = localPlayerScript;

        foreach (var playerScript in playerScripts)
        {
            var playerObject = playerObjects.Find(x => x.ID == playerScript.CharacterId);
            playerObjects.Remove(playerObject);

            playerObject.PlayerInfo = playerScript;
        }



        // setup the players
        var spawnIndex = 0;
        foreach (var player in players)
        {
            // add or enable components
            var rigidBody = player.AddComponent<Rigidbody>();

            if (player.GetComponent<NetworkBehaviour>().isLocalPlayer)
            {
                LocalPlayer = player;
                rigidBody.useGravity = true;
                LocalPlayer.GetComponent<RBPlayerMovement>().enabled = true;
            }

            // place the character at the correct spawn position
            var targetPosition = PlayerStartPositions[spawnIndex++];
            player.transform.position = targetPosition.position;
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
