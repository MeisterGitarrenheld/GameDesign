using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RBMatchListItem : MonoBehaviour
{
    // TODO deactivate the join button if this is null
    public static RBMatchListItem SelectedItem = null;

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

    public void OnValueChanged(Toggle sender)
    {
        if (gameObject.GetComponent<Toggle>().isOn)
        {
            SelectedItem = this;
        }
        else
            SelectedItem = null;
    }

    public void JoinMatch()
    {
        RBNetworkManager.Instance.StartClient(ConnInfo);
    }
}
