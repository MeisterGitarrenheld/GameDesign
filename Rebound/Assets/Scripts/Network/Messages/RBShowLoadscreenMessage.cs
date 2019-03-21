using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBShowLoadscreenMessage : MessageBase {

    public static short MSG_TYPE = (short)RBCustomMsgTypes.RBShowLoadscreenMessage;
    public bool Show;
    public string SceneName;
}
