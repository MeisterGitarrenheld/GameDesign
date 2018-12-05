using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBMatchListItem : MonoBehaviour
{
    private RBLanConnectionInfo _connInfo;
    public RBLanConnectionInfo ConnInfo { get { return _connInfo; } set { SetConnInfo(value); } }

    [SerializeField]
    private Text _hostname;
    [SerializeField]
    private Text _ipAddress;
    [SerializeField]
    private Text _playerCount;

    /// <summary>
    /// Sets the <see cref="ConnInfo"/> backingfield and applies the values to the UI.
    /// </summary>
    /// <param name="connInfo">Data of the match.</param>
    private void SetConnInfo(RBLanConnectionInfo connInfo)
    {
        _connInfo = connInfo;
        _hostname.text = connInfo.MatchInfo.HostPlayerName;
        _ipAddress.text = connInfo.IpAddress;
        _playerCount.text = connInfo.MatchInfo.CurrentPlayerCount + "/" + connInfo.MatchInfo.MaxPlayerCount;
    }
}
