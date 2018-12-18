using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBLobbyPlayerUISlot : MonoBehaviour
{
    [SerializeField]
    private Toggle _readyToggle = null;

    [SerializeField]
    private Text _usernameText = null;

    [SerializeField]
    private Dropdown _teamDropdown = null;

    [SerializeField]
    private RectTransform _readyPanel = null;

    [SerializeField]
    private RectTransform _unreadyPanel = null;

    /// <summary>
    /// True, if the local player is the host.
    /// </summary>
    public bool IsHost
    {
        get { return _teamDropdown.interactable; }
        set { _teamDropdown.interactable = value; }
    }

    /// <summary>
    /// True, if the slot corresponds to the local player.
    /// </summary>
    public bool IsLocalPlayer
    {
        get { return _readyToggle.gameObject.activeSelf; }
        set { _readyToggle.gameObject.SetActive(value); }
    }

    /// <summary>
    /// True, if the player that corresponds to the slot is ready.
    /// </summary>
    public bool IsReady
    {
        get { return _readyPanel.gameObject.activeSelf; }
        set
        {
            _readyPanel.gameObject.SetActive(value);
            _unreadyPanel.gameObject.SetActive(!value);

            if (Player != null)
                Player.IsReady = value;
        }
    }

    /// <summary>
    /// The username of the slots player.
    /// </summary>
    public string Username
    {
        get { return _usernameText.text; }
        set { _usernameText.text = value ?? string.Empty; }
    }

    /// <summary>
    /// The team number of the slots player starting with 1.
    /// </summary>
    public int SelectedTeam
    {
        get { return _teamDropdown.value + 1; }
        set
        {
            if (Player != null)
                Player.Team = value + 1;
        }
    }

    /// <summary>
    /// The corresponding player object.
    /// </summary>
    public RBPlayer Player { get; private set; }

    /// <summary>
    /// The number of available teams that should be displayed in the slot.
    /// </summary>
    public int TeamCount
    {
        get { return _teamDropdown.options.Count; }
        set
        {
            _teamDropdown.options.Clear();
            for (int i = 1; i <= value; i++)
                _teamDropdown.options.Add(new Dropdown.OptionData("Team " + i));
        }
    }

    /// <summary>
    /// Resets all properties to their default values.
    /// </summary>
    public void Reset()
    {
        IsHost = RBNetworkManager.Instance.IsHost;
        IsReady = false;
        IsLocalPlayer = false;
        Username = string.Empty;
        SelectedTeam = 1;
        TeamCount = RBMatch.Instance.TeamCount;
        Player = null;
    }

    /// <summary>
    /// Updates the slot data based on the given player object.
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayer(RBPlayer player)
    {
        IsHost = RBNetworkManager.Instance.IsHost;
        IsReady = player.IsReady;
        IsLocalPlayer = player.IsLocalUser;
        Username = player.Username;
        SelectedTeam = player.Team;
        TeamCount = RBMatch.Instance.TeamCount;
        Player = player;
    }
}
