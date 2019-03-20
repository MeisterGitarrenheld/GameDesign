using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBLoadingScreen : NetworkBehaviour
{
    public ARBArenaSetup ArenaSetup;
    public float SceneLoadDurationSeconds = 10.0f;


    void Update()
    {
        if(isServer)
        {
            SceneLoadDurationSeconds -= Time.deltaTime;

            if(SceneLoadDurationSeconds <= 0.0f)
            {
                ArenaSetup.PrepareForGameStart();
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
