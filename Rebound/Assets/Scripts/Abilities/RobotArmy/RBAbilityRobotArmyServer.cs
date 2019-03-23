using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBAbilityRobotArmyServer : NetworkBehaviour
{
    [SerializeField]
    private GameObject _robotPrefab;

    public float RobotSpeed = 5.0f;

    public void SpawnRobot(Vector3 position, Quaternion rotation, Vector3 forwardDirection)
    {
        CmdSpawnRobot(position, rotation, forwardDirection);
    }

    [Command]
    void CmdSpawnRobot(Vector3 position, Quaternion rotation, Vector3 forwardDirection)
    {
        var robot = Instantiate(_robotPrefab, position, rotation);
        NetworkServer.Spawn(robot);

        var colliderListener = new RBRobotColliderListener();
        colliderListener.For<RBRobotColliderBridge>(robot);
        colliderListener.OnHitGameObjectAction = other => OnRobotHitSomething(other, robot);
        colliderListener.OnCollisionEnterAction = collision => OnRobotHitSomething(collision.gameObject, robot);
    }

    void OnRobotHitSomething(GameObject other, GameObject robot)
    {
        if (!isServer) return;
        if (Terrain.activeTerrain.name == other.name) return;

        Debug.Log("Robot collision with: " + other.name);

        if (other.tag == "Player")
        {
            var playerObject = other;
            while (playerObject.transform.parent != null)
                playerObject = playerObject.transform.parent.gameObject;

            var nwIdentity = playerObject.GetComponent<NetworkIdentity>();

            TargetSlowPlayer(nwIdentity.connectionToClient);
        }

        NetworkServer.Destroy(robot);
    }

    [TargetRpc]
    void TargetSlowPlayer(NetworkConnection conn)
    {

    }
}
