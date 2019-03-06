using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPlayerPhysicsMessage : MessageBase
{
    public const short physics_msg = (short)RBCustomMsgTypes.RBPlayerPhysicsMessage;
    /// <summary>
    /// Direction and speed.
    /// Not normalized, Velocity is length of vector.
    /// </summary>
    public Vector3 objectHitDirection;
    public string PlayerHitName;
}
