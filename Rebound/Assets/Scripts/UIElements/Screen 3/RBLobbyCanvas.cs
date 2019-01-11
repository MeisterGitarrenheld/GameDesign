using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RBLobbyCanvas : RBCanvas
{
    public RBNetworkDiscovery NetworkDiscovery;
    public Text UsernamePageHeader;
    public Text IpAddressPageHeader;

    [SerializeField]
    private List<RBLobbyPlayerUISlot> _playerSlots;

    /// <summary>
    /// - Updates the lobby header information and joins host events.
    /// - Updates the player slots based on the current match data.
    /// </summary>
    /// <param name="navPage"></param>
    protected override void OnFadeInStarted(RBCanvasNavigation navPage)
    {
        base.OnFadeInStarted(navPage);
        UsernamePageHeader.text = RBLocalUser.Instance.Username;
        IpAddressPageHeader.text = RBLocalUser.Instance.LocalIpAddress?.ToString(); // TODO change to host ip

        RBNetworkManager.Instance.OnPlayerAdded -= NetworkHost_OnAddPlayer;
        RBNetworkManager.Instance.OnPlayerAdded += NetworkHost_OnAddPlayer;
        RBNetworkManager.Instance.OnPlayerRemoved -= NetworkHost_OnRemovePlayer;
        RBNetworkManager.Instance.OnPlayerRemoved += NetworkHost_OnRemovePlayer;

        RBMatch.Instance.OnMatchChanged -= Match_OnMatchChanged;
        RBMatch.Instance.OnMatchChanged += Match_OnMatchChanged;

        UpdatePlayerSlots();
    }

    /// <summary>
    /// Updates the player slots based on the current match data.
    /// </summary>
    private void Match_OnMatchChanged()
    {
        UpdatePlayerSlots();
    }

    /// <summary>
    /// Updates the player slots based on the current match data.
    /// </summary>
    private void UpdatePlayerSlots()
    {
        for (int i = 0; i < RBMatch.Instance.Players.Count; i++)
            _playerSlots[i].SetPlayer(RBMatch.Instance.Players[i]);

        for (int i = RBMatch.Instance.Players.Count; i < _playerSlots.Count; i++)
            _playerSlots[i].Reset();
    }


    /// <summary>
    /// Called when the player decides to leave the lobby screen (back button) and does not
    /// start the game.
    /// </summary>
    /// <param name="navPage"></param>
    protected override void OnFadeOutStarted(RBCanvasNavigation navPage)
    {
        base.OnFadeOutEnded(navPage);
        NetworkDiscovery.StopSendingMulticasts();
        NetworkManager.singleton.StopHost();
    }


    /// <summary>
    /// Decreases the current player count for the match and updates the
    /// current match info.
    /// </summary>
    private void NetworkHost_OnRemovePlayer()
    {
        var matchInfo = NetworkDiscovery.CurrentHostingMatch;
        matchInfo.CurrentPlayerCount -= 1;
        NetworkDiscovery.CurrentHostingMatch = matchInfo;
    }


    /// <summary>
    /// Increases the current player count for the match and updates the
    /// current match info.
    /// </summary>
    private void NetworkHost_OnAddPlayer()
    {
        var matchInfo = NetworkDiscovery.CurrentHostingMatch;
        matchInfo.CurrentPlayerCount += 1;
        NetworkDiscovery.CurrentHostingMatch = matchInfo;
    }
}
