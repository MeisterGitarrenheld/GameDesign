using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class ARBArenaSetup : NetworkBehaviour
{
    public static ARBArenaSetup Instance;

    public GameObject LocalPlayer = null;

    public Transform BallStartPosition;
    public Transform[] PlayerStartPositions;
    public RBGoal[] Goals;

    public GameObject BallPrefab;
    public GameObject ShieldPrefab;

    [SyncVar]
    public bool GamePaused = true;

    [SyncVar]
    public bool GameDone = false;

    [SyncVar]
    public float RemainingTime = 0.0f;

    [SyncVar]
    public int Team1Points = 0;

    [SyncVar]
    public int Team2Points = 0;

    [SyncVar]
    public int WinnerTeam = 0;

    public const float MaxMatchDurationSeconds = 10 * 60;

    public const int MaxGoalCount = 11;

    [SyncVar]
    public bool PlayerMovementLocked = true;

    protected virtual void Awake()
    {
        GameDone = false;
        Instance = this;
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
                LocalPlayer.GetComponent<RBAbilityActivityControl>().enabled = true;

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

    void StartGame()
    {
        if (!isServer)
            return;

        // TODO enter logic ;)
        print("Game starting...");
        RBNetworkGameManager.Instance.RespawnBall();
        GamePaused = false;
    }

    public void PrepareForGameStart()
    {
        // enable movement
        if (isServer)
            PlayerMovementLocked = false;

        // enable UI
        RBIngameCanvas.Instance.GetComponent<RBCanvasNavigation>().Show();
        LocalPlayer.GetComponent<RBNetworkPlayer>().Shield.transform.rotation = Quaternion.Euler(0, 0, 0);

        if(isServer)
            StartCoroutine(GameCountdown());
    }

    IEnumerator GameCountdown()
    {
        int i = 5;
        while (i >= 0)
        {
            yield return new WaitForSeconds(1);
            if (i == 0)
                LocalPlayer.GetComponent<RBNetworkPlayer>().SetCountdown("Start", true);
            else
                LocalPlayer.GetComponent<RBNetworkPlayer>().SetCountdown(i.ToString());

            i--;
        }

        yield return new WaitForSeconds(1);
        StartGame();
    }

    public void EndGame(float remainingTime, int team1Points, int team2Points)
    {
        if (!isServer)
            return;

        GamePaused = true;
        GameDone = true;

        RemainingTime = remainingTime;
        Team1Points = team1Points;
        Team2Points = team2Points;

        if (Team1Points > Team2Points)
            WinnerTeam = 1;
        else if (Team2Points > Team1Points)
            WinnerTeam = 2;
        else WinnerTeam = 0;

        StartCoroutine(ReturnToLoginScene());
    }

    IEnumerator ReturnToLoginScene()
    {
        yield return new WaitUntil(RBGameCompleteScreen.Instance.WinScreenCompleted);
        //yield return new WaitForSeconds(5);
        RBNetworkManager.Instance.ServerChangeScene("LoginScene");
    }
}
