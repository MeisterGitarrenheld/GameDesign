using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBRobotColliderListener : RBColliderListener
{
    public Action<GameObject> OnHitGameObjectAction;


    public void OnHitGameObject(GameObject other)
    {
        OnHitGameObjectAction?.Invoke(other);
    }
}
