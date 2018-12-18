using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class RBMatch
{
    public event Action OnMatchChanged;

    private static Lazy<RBMatch> _instance = new Lazy<RBMatch>(true);
    public static RBMatch Instance { get { return _instance.Value; } }

    private List<RBPlayer> _players = new List<RBPlayer>();
    public ReadOnlyCollection<RBPlayer> Players { get { return _players.AsReadOnly(); } }

    public int TeamCount { get; private set; } = 4;

    public int MaxPlayerCount { get; private set; } = 4;

    /// <summary>
    /// Resets all configurations of the match to their default values.
    /// </summary>
    public void Reset()
    {
        OnMatchChanged?.Invoke();
        _players.Clear();
    }


    /// <summary>
    /// Adds a new player to the current match.
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(RBPlayer player)
    {
        if (player.IsHost)
            _players.Insert(0, player);
        else
            _players.Add(player);
        OnMatchChanged?.Invoke();
    }


    /// <summary>
    /// Removes a player from the current match.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool RemovePlayer(RBPlayer player)
    {
        var success = _players.Remove(player);

        if (success)
            OnMatchChanged?.Invoke();

        return success;
    }


    /// <summary>
    /// Removes all players from the current match.
    /// </summary>
    /// <param name="silent"></param>
    public void RemoveAllPlayers(bool silent = false)
    {
        var cnt = _players.Count;

        _players.Clear();

        if (cnt > 0 && !silent)
            OnMatchChanged?.Invoke();
    }
}
