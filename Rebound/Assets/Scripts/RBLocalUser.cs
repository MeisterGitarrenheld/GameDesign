using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a singleton that persists between sceene changes.
/// It should be used to store information about the local user.
/// </summary>
public class RBLocalUser : RBMonoBehaviourSingleton<RBLocalUser>
{
    public RBUserProfileType ProfileType { get; set; } = RBUserProfileType.None;
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// <see cref="RBMonoBehaviourSingleton{T}.AwakeSingleton"/>
    /// </summary>
    protected override void AwakeSingleton()
    {
        
    }

    /// <summary>
    /// <see cref="RBMonoBehaviourSingleton{T}.OnDestroyUnityObject"/>
    /// </summary>
    protected override void OnDestroyUnityObject()
    {
        
    }
}
