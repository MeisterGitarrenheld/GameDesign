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
    public RBGoal[] Goals;

    public GameObject BallPrefab;
    public GameObject ShieldPrefab;

    protected virtual void Awake()
    {
        SetupPlayers();
        //SetupCamera();
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
        var team1SpawnIndex = 0;
        var team2SpawnIndex = 2;
        foreach (var player in players)
        {
            if (player.GetComponent<NetworkBehaviour>().isLocalPlayer)
            {
                LocalPlayer = player;

                SetupCamera();
                
                LocalPlayer.AddComponent<RBPlayerController>();
                LocalPlayer.AddComponent<RBPlayerAnimator>();
                LocalPlayer.AddComponent<RBPlayerMovement>();
                LocalPlayer.SetTagRecursively("Player");

                LocalPlayer.GetComponent<RBNetworkPlayer>().EnableShield();
            }

            // place the character at the correct spawn position
            var playerTeam = player.GetComponent<RBCharacter>().PlayerInfo.Team;
            var spawnIndex = playerTeam == 1 ? team1SpawnIndex++ : team2SpawnIndex++;

            Goals[spawnIndex].OwningTeamID = playerTeam;

            var targetPosition = PlayerStartPositions[spawnIndex];
            player.transform.position = targetPosition.position;
        }
    }


    private void SetupCamera()
    {
        var targetLookAt = new GameObject("Camera Focus");
        targetLookAt.transform.parent = LocalPlayer.transform;
        targetLookAt.transform.localPosition = new Vector3(0, 5, 0);

        RBCameraController.SetupCamera(targetLookAt);
    }
}
