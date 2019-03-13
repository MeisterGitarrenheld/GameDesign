using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Linq;

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
        throw new Exception(gobj.GetType() + " has no parent with component " + typeof(T));
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

    public static void SetLayerRecursively(this GameObject obj, int layer)
    {
        if (obj == null) return;

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            child.gameObject.SetLayerRecursively(layer);
        }
    }

    public static void SetTagRecursively(this GameObject obj, string tag)
    {
        if (obj == null) return;
        obj.tag = tag;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            child.gameObject.SetTagRecursively(tag);
        }
    }

    /// <summary>
    /// Performs a depth first search in the children of <see cref="GameObject.transform"/>.
    /// Returns the first result with the given tag.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindTagInChildrenRecursively(this GameObject obj, string tag)
    {
        if (obj == null) return null;

        var transform = obj.transform;

        foreach (Transform child in transform)
        {
            if (child == null) continue;
            if (child.tag == tag) return child.gameObject;

            // depth first search
            var result = child.gameObject.FindTagInChildrenRecursively(tag);
            if (result != null) return result;
        }

        return null;
    }

    /// <summary>
    /// Searches a tag in the parents recursively and returns the corresponding game object.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindTagInParentsRecursively(this GameObject obj, string tag)
    {
        if (obj == null) return null;

        var parent = obj.transform.parent;

        while (parent != null)
        {
            if (parent.tag == tag)
                return parent.gameObject;
            else
                parent = parent.parent;
        }

        return null;
    }

    /// <summary>
    /// Searches all players and returns the one with the given player name.
    /// </summary>
    /// <param name=""></param>
    /// <param name="playerName"></param>
    /// <returns></returns>
    public static GameObject FindPlayerByName(this GameObject obj, string playerName)
    {
        return GameObject.FindGameObjectsWithTag("Player").ToList().Find(x => x.GetComponent<RBCharacter>().PlayerInfo.Username == playerName);
    }
}
