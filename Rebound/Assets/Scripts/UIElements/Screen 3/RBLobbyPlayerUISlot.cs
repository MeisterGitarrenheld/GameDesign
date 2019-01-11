using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private set
        {
            _readyPanel.gameObject.SetActive(value);
            _unreadyPanel.gameObject.SetActive(!value);
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

    public void ToggleIsReadyUI(bool ready)
    {
        IsReady = ready;
        Player?.SetIsReady(ready);
    }

    public void SwitchSelectedTeamUI(int team)
    {
        Debug.Log("switch team");
        Player?.SetTeam(team + 1);
    }

    /// <summary>
    /// Resets all properties to their default values.
    /// </summary>
    public void Reset()
    {
        Player = null;
        IsHost = RBNetworkManager.Instance.IsHost;
        IsReady = false;
        IsLocalPlayer = false;
        Username = string.Empty;
        _teamDropdown.value = 0;
        TeamCount = RBMatch.Instance.TeamCount;
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
        //Debug.Log(string.Join(" <- ", new List<System.Diagnostics.StackFrame>(new System.Diagnostics.StackTrace().GetFrames()).Select(f => f.GetMethod().Name).Take(10)));
        //SelectedTeam = player.Team;
        _teamDropdown.value = player.Team - 1;
        TeamCount = RBMatch.Instance.TeamCount;
        Player = player;
    }
}
