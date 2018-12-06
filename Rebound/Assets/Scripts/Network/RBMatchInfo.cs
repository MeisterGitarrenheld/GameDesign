using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct RBMatchInfo
{
    public bool IsNull => HostPlayerName != "";
    public int Port;
    public string HostPlayerName;
    public int CurrentPlayerCount;
    public int MaxPlayerCount;
}
