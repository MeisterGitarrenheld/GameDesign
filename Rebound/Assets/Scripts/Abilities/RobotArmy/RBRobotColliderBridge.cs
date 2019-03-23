using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBRobotColliderBridge : RBColliderBridge
{
    void OnHitGameObject(GameObject other)
    {
        ((RBRobotColliderListener)listener).OnHitGameObject(other);
    }
}
