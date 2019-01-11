using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RBPlayer
{
    public event Action<RBPlayer, bool> OnReadyStateChanged;
    public event Action<RBPlayer, int> OnTeamChanged;

    public int ConnectionId { get; set; } = -1;

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
    /// Use <see cref="SetIsReady(bool, bool)"/> for setting.
    /// The public setter is only needed for serialization.
    /// </summary>
    public bool IsReady { get; set; } = false;

    /// <summary>
    /// True, if the object corresponds to the local player.
    /// </summary>
    public bool IsLocalUser { get { return Username == RBLocalUser.Instance.Username; } }

    /// <summary>
    /// The team number of the player.
    /// Use <see cref="SetTeam(int, bool)"/> for setting.
    /// The public setter is only needed for serialization.
    /// </summary>
    public int Team { get; set; } = 1;

    /// <summary>
    /// Updates the ready state of the player.
    /// Triggers the <see cref="OnReadyStateChanged"/> event if changed and not silent.
    /// </summary>
    /// <param name="ready"></param>
    /// <param name="silent"></param>
    public void SetIsReady(bool ready, bool silent = false)
    {
        var changed = IsReady != ready;
        IsReady = ready;

        if (!silent && changed)
            OnReadyStateChanged?.Invoke(this, ready);
    }

    /// <summary>
    /// Updates the team of the player.
    /// Triggers the <see cref="OnTeamChanged"/> event if changed and not silent.
    /// </summary>
    /// <param name="team"></param>
    /// <param name="silent"></param>
    public void SetTeam(int team, bool silent = false)
    {
        var changed = Team != team;
        Team = team;

        if (!silent && changed)
            OnTeamChanged?.Invoke(this, team);
    }
}
