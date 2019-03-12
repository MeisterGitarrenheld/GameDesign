using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBIGOfflineBall : MonoBehaviour {

    private Rigidbody rb;
    
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
	}
	
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Shield")
        {
            if (rb.velocity.magnitude < 0.1f)
                rb.velocity = other.transform.forward * 10;
            else
            {
                rb.velocity = 
                    Vector3.Reflect(rb.velocity, other.transform.forward).normalized * 
                    (rb.velocity.magnitude + other.GetComponentInParent<Rigidbody>().velocity.magnitude);
            }
        }
        else if(other.tag == "PlasmaShot")
        {
            rb.AddForce(other.GetComponent<Rigidbody>().velocity * 1500 * Time.deltaTime);
            Destroy(other.gameObject);
        }
    }
}
