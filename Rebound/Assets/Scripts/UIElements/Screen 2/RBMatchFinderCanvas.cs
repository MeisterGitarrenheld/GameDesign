using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RBMatchFinderCanvas : RBCanvas
{
    public Text UsernameDisp;
    public RBNetworkDiscovery NetworkDiscovery;
    public GameObject MatchListItemPrefab;
    public GameObject MatchList;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnFadeInStarted(RBCanvasNavigation navPage)
    {
        base.OnFadeInStarted(navPage);

        UsernameDisp.text = RBLocalUser.Instance.Username;
        RBLocalUser.Instance.IsHost = false;

        NetworkDiscovery.OnUpdateMatchInfo -= NetworkDiscovery_OnUpdateMatchInfo;
        NetworkDiscovery.OnUpdateMatchInfo += NetworkDiscovery_OnUpdateMatchInfo;
        NetworkDiscovery.StartListeningForMulticasts();

    }

    protected override void OnFadeOutEnded(RBCanvasNavigation navPage)
    {
        base.OnFadeOutEnded(navPage);

        NetworkDiscovery.StopListeningForMulticasts();
    }

    /// <summary>
    /// Updates the match list depending on the given match infos.
    /// </summary>
    /// <param name="connInfo">Data of the match.</param>
    /// <param name="state">State which describes the action which is done on the list.</param>
    private void NetworkDiscovery_OnUpdateMatchInfo(RBLanConnectionInfo connInfo, RBNetworkDiscovery.ModifyState state)
    {
        switch(state)
        {
            case RBNetworkDiscovery.ModifyState.Added:
                Instantiate(MatchListItemPrefab, MatchList.transform).GetComponent<RBMatchListItem>().ConnInfo = connInfo;
                break;
            case RBNetworkDiscovery.ModifyState.Changed:
                var items = MatchList.gameObject.GetComponentsInChildren<RBMatchListItem>();
                for (int i = 0; i < items.Length; i++)
                    if (items[i].ConnInfo.EqualsHost(connInfo))
                    {
                        items[0].ConnInfo = connInfo;
                        break;
                    }
                break;
            case RBNetworkDiscovery.ModifyState.Removed:
                items = MatchList.gameObject.GetComponentsInChildren<RBMatchListItem>();
                for (int i = 0; i < items.Length; i++)
                    if (items[i].ConnInfo.EqualsHost(connInfo))
                    {
                        Destroy(items[0].gameObject);
                        break;
                    }
                break;
        }
    }

    /// <summary>
    /// Starts a match as host and switches to the lobby screen.
    /// </summary>
    public void OnHostButtonClick(RBCanvasNavigation pageToSwitch)
    {
        pageToSwitch.Show();
        NetworkDiscovery.StartSendingMulticasts();
        RBLocalUser.Instance.IsHost = true;

        RBNetworkManager.Instance.StartHost();
    }
}
