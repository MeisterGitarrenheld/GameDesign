using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RBNetworkDiscovery : MonoBehaviour
{
    public enum ModifyState { None, Changed, Removed, Added }

    public event Action<RBLanConnectionInfo, ModifyState> OnUpdateMatchInfo;


    public RBMatchInfo CurrentHostingMatch
    {
        get { return _currentHostingMatch; }
        set
        {
            _currentHostingMatch = value;
            McMessage = RBSerializer.Serialize(value, false);
        }
    }
    private RBMatchInfo _currentHostingMatch;

    private float _timeOut = 5f;
    private Dictionary<RBLanConnectionInfo, float> _lanAddresses = new Dictionary<RBLanConnectionInfo, float>();


    #region Multicast-Config
    private RBTimedUdpMulticast _udpMulticast = new RBTimedUdpMulticast();

    [SerializeField]
    private int _port = RBTimedUdpMulticast.DEFAULT_PORT;
    public int Port
    {
        get { return _port; }
        set
        {
            _port = value;
            _udpMulticast.Port = value;
        }
    }

    [SerializeField]
    private int _mcInterval = RBTimedUdpMulticast.DEFAULT_MULTICAST_INTERVAL;
    public int McInterval
    {
        get { return _mcInterval; }
        set
        {
            _mcInterval = value;
            _udpMulticast.MulticastInterval = value;
        }
    }

    [SerializeField]
    private string _mcMessage = RBTimedUdpMulticast.DEFAULT_MESSAGE;
    public string McMessage
    {
        get { return _mcMessage; }
        set
        {
            _mcMessage = value ?? string.Empty;
            _udpMulticast.MulticastMessage = value ?? string.Empty;
        }
    }

    [SerializeField]
    private string _mcAddress = RBTimedUdpMulticast.DEFAULT_MULTICAST_ADDRESS;
    public string McAddress
    {
        get { return _mcAddress; }
        set
        {
            _mcAddress = value ?? string.Empty;
            _udpMulticast.MulticastMessage = value ?? string.Empty;
        }
    }
    #endregion


    void Awake()
    {
        _udpMulticast.Port = Port;
        _udpMulticast.MulticastMessage = McMessage;
        _udpMulticast.MulticastInterval = McInterval;
        _udpMulticast.MulticastAddress = McAddress;
        _udpMulticast.OnMulticastReceived += UdpBroadcast_OnMulticastReceived;
    }

    void Update()
    {
        _udpMulticast.CheckForMulticasts();
    }

    /// <summary>
    /// Starts listening for multicasts and checks for closed matches.
    /// </summary>
    public void StartListeningForMulticasts()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            RBErrorMessage.Instance.ShowError("There is no valid lan connection. Please check you connection and try again.", RBErrorMessage.ErrorType.Warning);
            return;
        }

        StartCoroutine(CleanupExpiredEntries());

        _udpMulticast.StartListeningForMulticasts();
    }

    /// <summary>
    /// Stops listening for multicasts and removes all match infos.
    /// </summary>
    public void StopListeningForMulticasts()
    {
        _udpMulticast.StopListeningForMulticasts();

        StopAllCoroutines();
        var keys = _lanAddresses.Keys.ToList();

        for (int i = keys.Count - 1; i >= 0; i--)
        {
            UpdateMatchInfos(keys[i], ModifyState.Removed);
        }

        _lanAddresses.Clear();
    }

    /// <summary>
    /// Starts a match as host and multicasts the match info periodically.
    /// </summary>
    public void StartSendingMulticasts(RBMatchInfo matchInfo)
    {
        matchInfo.HostPlayerName = RBLocalUser.Instance.Username;
        matchInfo.Port = RBNetworkManager.Instance.networkPort;

        StopListeningForMulticasts();
        _udpMulticast.StopMulticast();

        CurrentHostingMatch = matchInfo;

        _udpMulticast.StartMulticast();
    }

    /// <summary>
    /// <see cref="StartSendingMulticasts(RBMatchInfo)"/>
    /// </summary>
    public void StartSendingMulticasts()
    {
        RBMatchInfo matchInfo = new RBMatchInfo()
        {
            CurrentPlayerCount = 1,
            MaxPlayerCount = 4,
        };

        StartSendingMulticasts(matchInfo);
    }


    /// <summary>
    /// Stops the match hosting and multicasting of the match infos
    /// </summary>
    public void StopSendingMulticasts()
    {
        _udpMulticast.StopMulticast();
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
    /// <param name="fromPort">Source port of the broadcast</param>
    /// <param name="message">match info as json string. Example: {"Port":4321,"HostPlayerName":"Ho ho host","CurrentPlayerCount":2,"MaxPlayerCount":4}</param>
    private void UdpBroadcast_OnMulticastReceived(string fromAddress, int fromPort, string message)
    {
        RBLanConnectionInfo info = new RBLanConnectionInfo(fromAddress, message);

        if (string.Equals(info.MatchInfo.HostPlayerName, RBLocalUser.Instance.Username)) return;

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
