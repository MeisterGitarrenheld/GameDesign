using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBColliderBridge : MonoBehaviour
{
    private RBColliderListener _listener;

    public void Initialize(RBColliderListener l)
    {
        _listener = l;
    }
    void OnCollisionEnter(Collision collision)
    {
        _listener.OnCollisionEnter(collision);
    }
    void OnTriggerEnter(Collider other)
    {
        _listener.OnTriggerEnter(other);
    }
}
