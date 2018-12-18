using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBInitPlayerMessage : MessageBase
{
    public static short MSG_TYPE = (short)RBCustomMsgTypes.RBInitPlayerMessage;
    public string Username = string.Empty;
}
