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

        RBNetworkManager.Instance.OnAddPlayer += Instance_OnAddPlayer;
        RBNetworkManager.Instance.OnRemovePlayer += Instance_OnRemovePlayer;
    }

    private void Instance_OnRemovePlayer()
    {
        var matchInfo = NetworkDiscovery.CurrentHostingMatch;
        matchInfo.CurrentPlayerCount -= 1;
        NetworkDiscovery.CurrentHostingMatch = matchInfo;
    }

    private void Instance_OnAddPlayer()
    {
        var matchInfo = NetworkDiscovery.CurrentHostingMatch;
        matchInfo.CurrentPlayerCount += 1;
        NetworkDiscovery.CurrentHostingMatch = matchInfo;
    }

    protected override void OnFadeOutStarted(RBCanvasNavigation navPage)
    {
        base.OnFadeOutEnded(navPage);
        NetworkDiscovery.StopSendingMulticasts();
    }
}
