using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class RBExtensions {

	public static T FindComponentInObjectOrParent<T>(this GameObject gobj)
    {
        Transform t = gobj.transform;
        while(t != null)
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
}
