using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBLobbyPlayerUISlot : MonoBehaviour
{
    /// <summary>
    /// Called when the slot corresponds to the local player and
    /// he toggles the ready button.
    /// </summary>
    public event Action<bool> OnReadyStateChanged;

    /// <summary>
    /// Called when the local player is the host and he
    /// changes the slots team.
    /// </summary>
    public event Action<int> OnTeamChanged;

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

            OnReadyStateChanged?.Invoke(value);
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
            _teamDropdown.value = value - 1;

            OnTeamChanged?.Invoke(value - 1);
        }
    }

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
}
