using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBColliderListener
{
    public Action<Collider> OnTriggerEnterAction;
    public Action<Collision> OnCollisionEnterAction;

    public void For(GameObject gameObject)
    {
        Collider collider = gameObject.GetComponentInChildren<Collider>();

        RBColliderBridge cb = collider.gameObject.AddComponent<RBColliderBridge>();
        cb.Initialize(this);
    }

    public void OnCollisionEnter(Collision collision)
    {
        OnCollisionEnterAction?.Invoke(collision);
    }

    public void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterAction?.Invoke(other);
    }
}
