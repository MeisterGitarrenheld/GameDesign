using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBColliderBridge : MonoBehaviour
{
    protected RBColliderListener listener;

    public void Initialize(RBColliderListener l)
    {
        listener = l;
    }
    void OnCollisionEnter(Collision collision)
    {
        listener.OnCollisionEnter(collision);
    }
    void OnTriggerEnter(Collider other)
    {
        listener.OnTriggerEnter(other);
    }
}
