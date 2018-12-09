using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBShield : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ball"))
        {
            Rigidbody oRB = other.GetComponent<Rigidbody>();
            //oRB.velocity = Vector3.Reflect(oRB.velocity, transform.forward);
            oRB.velocity = transform.forward * oRB.velocity.magnitude * 20;
        }
    }
}
