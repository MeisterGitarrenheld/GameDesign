using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public static class RBExtensions
{

    public static T FindComponentInObjectOrParent<T>(this GameObject gobj)
    {
        Transform t = gobj.transform;
        while (t != null)
        {
            var component = t.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            t = t.parent?.transform;
        }
        throw new Exception(gobj.GetType() + "has no parent with component " + typeof(T));
    }

    /// <summary>
    /// Returns the connection id which is unique and between <see cref="Int16.MinValue"/> and <see cref="Int16.MaxValue"/>.
    /// </summary>
    /// <param name="conn"></param>
    /// <returns></returns>
    public static short GetPlayerId(this NetworkConnection conn)
    {
        return (short)conn.connectionId;
    }
}
