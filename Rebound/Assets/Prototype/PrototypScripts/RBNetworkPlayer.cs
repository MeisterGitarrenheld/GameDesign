using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RBNetworkPlayer : NetworkBehaviour
{
    [SyncVar]
    public bool ShieldEnabled = false;

    private bool _prevShieldEnabled = false;

    public GameObject Shield;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (_prevShieldEnabled != ShieldEnabled)
        {
            Shield.SetActive(ShieldEnabled);
            CmdEnabledShield(gameObject.GetComponent<RBCharacter>().PlayerInfo.Username);
            _prevShieldEnabled = ShieldEnabled;
        }
    }

    public void EnableShield()
    {
        ShieldEnabled = true;
    }

    [Command]
    void CmdEnabledShield(string playerName)
    {
        // Change it on the server
        EnableShieldByPlayerName(playerName);

        // Change it on the other clients
        RpcEnableShieldByPlayerName(playerName);
    }

    void EnableShieldByPlayerName(string playerName)
    {
        var playerObject = gameObject.FindPlayerByName(playerName);
        playerObject.GetComponent<RBNetworkPlayer>().Shield.SetActive(true);
    }

    [ClientRpc]
    void RpcEnableShieldByPlayerName(string playerName)
    {
        // Enable it on all other clients
        EnableShieldByPlayerName(playerName);
    }

    public void SetCountdown(int number)
    {
        RpcSetCountdown(number);
    }

    [ClientRpc]
    private void RpcSetCountdown(int number)
    {
        print("Game Starts in " + number);
    }
}
