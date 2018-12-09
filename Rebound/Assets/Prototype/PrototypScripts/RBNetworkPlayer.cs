using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBNetworkPlayer : NetworkBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
