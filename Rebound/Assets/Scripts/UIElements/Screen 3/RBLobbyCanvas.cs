using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    [SerializeField]
    private RBAnimatedButton _matchStartButton;

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
        RBCharacterPreview.Instance.SetPreview(RBCharacterInfo.Instance.GetDefaultCharacter());
        _matchStartButton.Disable();

        // TODO Is this the right way to hide the button?
        if (!RBMatch.Instance.IsLocalUserHost())
            _matchStartButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the player slots based on the current match data.
    /// </summary>
    private void Match_OnMatchChanged()
    {
        UpdatePlayerSlots();
        UpdateMatchStartButton();
    }

    /// <summary>
    /// Shows or hides the match start button, depending on the ready state and
    /// the host information.
    /// </summary>
    private void UpdateMatchStartButton()
    {
        if (RBMatch.Instance.IsLocalUserHost() && RBMatch.Instance.IsEveryoneReady())
        {   // show the start button
            _matchStartButton.Show();
        }
        else
        {
            // hide the start button
            _matchStartButton.Disable();
        }
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
    /// Searches the ui slot for the local player. Returns null, if not found.
    /// </summary>
    /// <returns></returns>
    public RBLobbyPlayerUISlot GetLocalPlayerUISlot()
    {
        return _playerSlots.Find(x => x.IsLocalPlayer);
    }

    /// <summary>
    /// Returns all ui player slots.
    /// </summary>
    /// <returns></returns>
    public ReadOnlyCollection<RBLobbyPlayerUISlot> GetAllPlayerUISlots()
    {
        return _playerSlots.AsReadOnly();
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

    protected override void OnFadeOutEnded(RBCanvasNavigation navPage)
    {
        base.OnFadeOutEnded(navPage);




        // TODO Show start button
        _matchStartButton.gameObject.SetActive(true);
        _matchStartButton.Disable();
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
