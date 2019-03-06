using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBIGPlayTest : MonoBehaviour {

    public static RBIGPlayTest Instance;

    public Transform BallStartPosition;
    public Transform[] PlayerStartPositions;

    public GameObject PlayerPrefab;
    public GameObject BallPrefab;

    public GameObject Ball;
    public Dictionary<int, GameObject> Players;

    private float BallImpulseTimer = 50;

    public Transform Camera;
    private void Awake()
    {
        Instance = this;
    }

    void Start ()
    {
        Players = new Dictionary<int, GameObject>();
        for (int i = 0; i < 4; i++)
            SpawnPlayer(i == 0, PlayerStartPositions[i], i);
        SpawnBall();
	}
	
	void Update ()
    {
        BallImpulseTimer += Time.deltaTime;
        if(BallImpulseTimer > 20)
        {
            BallImpulseTimer = 0;
            Ball.GetComponent<Rigidbody>().AddForce((Players[0].transform.position - Ball.transform.position).normalized * 1000 + Vector3.up * 500);
        }
    }

    void SpawnBall()
    {
        if (Ball != null)
            Destroy(Ball);
        Ball = Instantiate(BallPrefab, BallStartPosition.position, BallStartPosition.rotation);
        BallImpulseTimer = 50;
    }

    void SpawnPlayer(bool controllable, Transform startPosition, int id)
    {
        var Player = Instantiate(PlayerPrefab, startPosition.position, startPosition.rotation);
        Player.GetComponent<RBNetworkMovementSync>().enabled = false;
        if (!controllable)
            Player.GetComponent<RBPlayerMove>().enabled = false;
        else
        {
            Camera.parent = Player.transform;
            Camera.localPosition = Vector3.back * 20 + Vector3.up * 10;
            Camera.localRotation = Quaternion.identity;
        }
        Player.GetComponent<RBPlayerMove>().Offline = true;
        Player.GetComponent<RBPlayerMove>().PlayerStartPos = startPosition;
        Players.Add(id, Player);
    }


    void SpawnPowerUp()
    {

    }

    public void Goal(int goalID)
    {
        SpawnBall();
    }
}
