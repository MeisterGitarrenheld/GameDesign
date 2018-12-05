using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RBNetworkDiscovery : NetworkDiscovery
{
    public enum ModifyState { None, Changed, Removed, Added }

    public event Action<RBLanConnectionInfo, ModifyState> OnUpdateMatchInfo;

    private float _timeOut = 5f;
    private Dictionary<RBLanConnectionInfo, float> _lanAddresses = new Dictionary<RBLanConnectionInfo, float>();

    /// <summary>
    /// Starts listening for broadcasts and checks for closed matches.
    /// </summary>
    public void StartClient()
    {
        base.Initialize();
        base.StartAsClient();

        StartCoroutine(CleanupExpiredEntries());
    }

    /// <summary>
    /// Stops listening for broadcasts and removes all match infos.
    /// </summary>
    public void StopClient()
    {
        StopBroadcast();

        StopAllCoroutines();
        var keys = _lanAddresses.Keys.ToList();

        for(int i = keys.Count-1; i >= 0 ; i--)
        {
            UpdateMatchInfos(keys[i], ModifyState.Removed);
        }

        _lanAddresses.Clear();
    }

    /// <summary>
    /// Starts a match as host and broadcasts the match info periodically.
    /// </summary>
    public void StartServer()
    {
        StopBroadcast();

        // TODO: wtf is this?
        RBMatchInfo matchInfo = new RBMatchInfo()
        {
            CurrentPlayerCount = 2,
            MaxPlayerCount = 4,
            HostPlayerName = "Ho ho host",
            Port = 4321
        };

        broadcastData = JsonUtility.ToJson(matchInfo, false);

        base.Initialize();
        base.StartAsServer();
    }

    /// <summary>
    /// Removes outdated connection infos.
    /// </summary>
    private IEnumerator CleanupExpiredEntries()
    {
        while (true)
        {
            foreach (var key in _lanAddresses.Keys.ToList())
            {
                if (_lanAddresses[key] <= Time.time)
                {
                    _lanAddresses.Remove(key);
                    UpdateMatchInfos(key, ModifyState.Removed);
                }
            }

            yield return new WaitForSeconds(_timeOut);
        }
    }

    /// <summary>
    /// Creates or updates the match infos depending on the broadcast message.
    /// </summary>
    /// <param name="fromAddress">Server ip address</param>
    /// <param name="data">match info as json string. Example: {"Port":4321,"HostPlayerName":"Ho ho host","CurrentPlayerCount":2,"MaxPlayerCount":4}</param>
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);

        RBLanConnectionInfo info = new RBLanConnectionInfo(fromAddress, data);

        ModifyState state = ModifyState.None;

        var existingInfo = _lanAddresses.Select(x => x.Key).Where(x => x.EqualsHost(info)).ToList();
        if (existingInfo.Any())
        {
            if (!existingInfo[0].Equals(info))
            {
                _lanAddresses.Remove(existingInfo[0]);
                state = ModifyState.Changed;
            }

        }
        else
            state = ModifyState.Added;

        _lanAddresses[info] = Time.time + _timeOut;

        if (state != ModifyState.None)
            UpdateMatchInfos(info, state);
    }

    /// <summary>
    /// Invokes the <see cref="OnUpdateMatchInfo"/> event.
    /// </summary>
    /// <param name="connInfo">The object with all information.</param>
    /// <param name="state">State which describes the action which is done on the list.</param>
    private void UpdateMatchInfos(RBLanConnectionInfo connInfo, ModifyState state)
    {
        OnUpdateMatchInfo?.Invoke(connInfo, state);
    }
}
