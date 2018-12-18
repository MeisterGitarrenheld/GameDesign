using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPlayer
{
    public event Action<RBPlayer, bool> OnReadyStateChanged;
    public event Action<RBPlayer, int> OnTeamChanged;

    /// <summary>
    /// The name of the player.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// True, if the object corresponds to the host player (does not have to be the local user).
    /// </summary>
    public bool IsHost { get; set; } = false;

    /// <summary>
    /// True, if the corresponding player has hit the ready button.
    /// </summary>
    public bool IsReady
    {
        get { return _isReady; }
        set
        {
            var changed = _isReady != value;
            _isReady = value;
            if (changed)
                OnReadyStateChanged?.Invoke(this, value);
        }
    }
    private bool _isReady = false;

    /// <summary>
    /// True, if the object corresponds to the local player.
    /// </summary>
    public bool IsLocalUser { get { return Username == RBLocalUser.Instance.Username; } }

    /// <summary>
    /// The team number of the player.
    /// </summary>
    public int Team
    {
        get { return _team; }
        set
        {
            var changed = _team != value;
            _team = value;
            if (changed)
                OnTeamChanged?.Invoke(this, value);
        }
    }
    private int _team = 1;
}
