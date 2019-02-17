using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPlayerMovementMessage : MessageBase
{
    public const short movement_msg = (short)RBCustomMsgTypes.RBPlayerMovementMessage;
    public string objectTransformName;
    public Vector3 objectPosition;
    public Quaternion objectRotation;
    public float time;
}