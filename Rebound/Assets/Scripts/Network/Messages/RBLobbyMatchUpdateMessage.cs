using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBLobbyMatchUpdateMessage : MessageBase
{
    public static short MSG_TYPE = (short)RBCustomMsgTypes.RBLobbyMatchUpdateMessage;
    public string[] Players = null;
    public bool FromHost = false;
}
