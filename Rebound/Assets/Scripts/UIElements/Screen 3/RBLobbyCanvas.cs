using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBLobbyCanvas : RBCanvas {

    public RBNetworkDiscovery NetworkDiscovery;
    public Text Username;
    public Text IpAddress;

    protected override void OnFadeInStarted(RBCanvasNavigation navPage)
    {
        base.OnFadeInStarted(navPage);
        Username.text = RBLocalUser.Instance.Username;
        IpAddress.text = RBLocalUser.Instance.LocalIpAddress?.ToString();
    }

    public void OnPlayerCountInc()
    {
        var matchInfo = NetworkDiscovery.CurrentHostingMatch;
        matchInfo.CurrentPlayerCount += 1;
        NetworkDiscovery.StartServer(matchInfo);
    }

    public void OnPlayerCountDec()
    {
        var matchInfo = NetworkDiscovery.CurrentHostingMatch;
        matchInfo.CurrentPlayerCount -= 1;
        NetworkDiscovery.StartServer(matchInfo);
    }

    public void OnLeaveLobby()
    {
        NetworkDiscovery.StopServer();
        GetComponent<RBCanvasNavigation>().PageBack();
    }
}
