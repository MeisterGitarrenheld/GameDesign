using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBAbilityWallServer : NetworkBehaviour {

    [SerializeField]
    private GameObject _wallPrefab;
    
    public void SpawnWall(Vector3 pos, Quaternion rot, int duration)
    {
        CmdSpawnWall(pos, rot, duration);
    }

    [Command]
    private void CmdSpawnWall(Vector3 pos, Quaternion rot, int duration)
    {
        var obj = Instantiate(_wallPrefab, pos, rot);
        NetworkServer.Spawn(obj);
        StartCoroutine(DestroyAfterTime(obj, duration));
    }

    IEnumerator DestroyAfterTime(GameObject obj, int duration)
    {
        yield return new WaitForSeconds(duration);
        NetworkServer.Destroy(obj);
    }
}
