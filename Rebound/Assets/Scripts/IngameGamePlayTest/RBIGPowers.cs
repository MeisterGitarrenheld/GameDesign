using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBIGPowers : MonoBehaviour {


    public GameObject Plasma;
    public float PlasmaSpeed;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            ShootPlasma();    
    }

    void ShootPlasma()
    {
        var plasma = Instantiate(Plasma, transform.position + transform.forward * 10, transform.rotation).GetComponent<Rigidbody>();
        plasma.velocity = transform.GetChild(1).forward * PlasmaSpeed;
        plasma.transform.rotation = Quaternion.LookRotation(transform.GetChild(1).forward, transform.GetChild(1).up);
        Destroy(plasma, 20);
    }

}
