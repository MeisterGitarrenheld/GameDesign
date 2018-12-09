using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawn : MonoBehaviour
{

    public GameObject Ball;
    public Transform Player;
    public float SpawnRate;
    public float MaxForce;
    public float MinForce;

    private float _spawnTimer;
    private BoxCollider col;
    private float _spawnWidth;
    private float _spawnHeight;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        _spawnWidth = col.bounds.size.x;
        _spawnHeight = col.bounds.size.y;
    }
    
    void Update()
    {
        if(_spawnTimer > SpawnRate)
        {
            Vector3 spawnPosition = new Vector3(0, Random.Range(0, _spawnHeight), Random.Range(0, _spawnWidth));
            var ball = Instantiate(Ball, spawnPosition + col.center, Quaternion.identity).GetComponent<Rigidbody>();
            ball.AddForce((Player.position - ball.position).normalized * Random.Range(MinForce, MaxForce));
            _spawnTimer = 0;
        }
        _spawnTimer += Time.deltaTime;
    }
}
