using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBColliderListener
{
    public Action<Collider> OnTriggerEnterAction;
    public Action<Collision> OnCollisionEnterAction;

    public void For<T>(GameObject gameObject) where T : RBColliderBridge
    {
        Collider collider = gameObject.GetComponentInChildren<Collider>();

        T cb = collider.gameObject.AddComponent<T>();
        cb.Initialize(this);
    }

    public void For(GameObject gameObject)
    {
        For<RBColliderBridge>(gameObject);
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
