using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RBLanConnectionInfo
{
    public string IpAddress;
    public int Port => MatchInfo.Port;
    public RBMatchInfo MatchInfo;

    public RBLanConnectionInfo(string fromAddress, string data)
    {
        var addressParts = fromAddress.Split(':');
        IpAddress = addressParts[addressParts.Length - 1];
        Debug.Log(data);
        MatchInfo = RBSerializer.Deserialize<RBMatchInfo>(data);
    }

    /// <summary>
    /// Checks if ip address and port are equal.
    /// </summary>
    /// <param name="lanInfo"></param>
    /// <returns></returns>
    public bool EqualsHost(RBLanConnectionInfo lanInfo)
    {
        return IpAddress == lanInfo.IpAddress && Port == lanInfo.Port;
    }
}
