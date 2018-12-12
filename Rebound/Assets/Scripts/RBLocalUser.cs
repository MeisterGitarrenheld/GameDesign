﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// This class is a singleton that persists between sceene changes.
/// It should be used to store information about the local user.
/// </summary>
public class RBLocalUser : RBMonoBehaviourSingleton<RBLocalUser>
{
    public RBUserProfileType ProfileType { get; set; } = RBUserProfileType.None;
    public string Username { get; set; } = string.Empty;
    public IPAddress LocalIpAddress { get { return GetLocalIp(); } }
    public bool IsHost { get; set; } = false;

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

    private IPAddress GetLocalIp()
    {
        IPHostEntry host;
        host = Dns.GetHostEntry(Dns.GetHostName());

        foreach(var ip in host.AddressList)
        {
            if(ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }
        }

        return null;
    }
}
