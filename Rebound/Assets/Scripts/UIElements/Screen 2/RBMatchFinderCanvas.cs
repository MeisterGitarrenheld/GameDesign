﻿using System.Collections;
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
    public Text MatchListPlaceholder;
    public RBInputField ConcretIPAddress;
    public Button JoinButton;

    [SerializeField]
    private RBCanvasNavigation _lobbyPage;

    private int MatchCount = 0;

    protected override void Awake()
    {
        base.Awake();
        RBNetworkManager.Instance.OnClientStarted += NetworkManager_OnClientStarted;
    }

    /// <summary>
    /// Switches to the lobby screen when the client is connected to the host.
    /// </summary>
    private void NetworkManager_OnClientStarted()
    {
        _lobbyPage?.Show();
    }

    protected override void OnFadeInStarted(RBCanvasNavigation navPage)
    {
        base.OnFadeInStarted(navPage);

        UsernameDisp.text = RBLocalUser.Instance.Username;
        RBMatchListItem.SelectedItem = null;

        NetworkDiscovery.OnUpdateMatchInfo -= NetworkDiscovery_OnUpdateMatchInfo;
        NetworkDiscovery.OnUpdateMatchInfo += NetworkDiscovery_OnUpdateMatchInfo;
        NetworkDiscovery.StartListeningForMulticasts();
    }

    protected override void OnFadeInEnded(RBCanvasNavigation navPage)
    {
        base.OnFadeInEnded(navPage);

        JoinButton.interactable = false;
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
        switch (state)
        {
            case RBNetworkDiscovery.ModifyState.Added:
                var obj = Instantiate(MatchListItemPrefab, MatchList.transform);
                obj.GetComponent<Toggle>().group = MatchList.GetComponent<ToggleGroup>();
                obj.GetComponent<Toggle>().onValueChanged.AddListener(delegate { UpdateJoinButtonState(); });
                obj.GetComponent<RBMatchListItem>().ConnInfo = connInfo;
                MatchCount++;
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
                        MatchCount--;
                        break;
                    }
                break;
        }

        UpdateMatchListPlaceholder();
    }

    private void UpdateMatchListPlaceholder()
    {
        if (MatchCount == 0)
            MatchListPlaceholder.gameObject.SetActive(true);
        else
            MatchListPlaceholder.gameObject.SetActive(false);
    }

    public void OnJoinButtonClick()
    {
        if(ConcretIPAddress.ContainsText)
        {
            // TODO do join to explicit ip address
            RBErrorMessage.Instance.ShowError("Join to specific IPs are not implemented yet.", RBErrorMessage.ErrorType.Error, "Not Implemented");
        }
        else
            RBMatchListItem.SelectedItem?.JoinMatch();
    }

    public void UpdateJoinButtonState()
    {
        if (ConcretIPAddress.ContainsText || RBMatchListItem.SelectedItem != null)
            JoinButton.interactable = true;
        else
            JoinButton.interactable = false;
    }

    /// <summary>
    /// Starts a match as host and switches to the lobby screen.
    /// </summary>
    public void OnHostButtonClick()
    {
        NetworkDiscovery.StartSendingMulticasts();

        RBNetworkManager.Instance.StartHost();
    }
}
