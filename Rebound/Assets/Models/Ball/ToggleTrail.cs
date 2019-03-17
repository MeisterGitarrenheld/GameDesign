using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TrailRenderer))]

public class ToggleTrail : MonoBehaviour {
    public float minimumVelocity;

    private Rigidbody BallRigidBody;
    private TrailRenderer renderer;
    // Use this for initialization
    void Start () {
        BallRigidBody = this.GetComponentInParent<Rigidbody>();
        renderer = this.GetComponent<TrailRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(BallRigidBody.velocity.magnitude > minimumVelocity && !renderer.enabled )
        {
            renderer.enabled = true;
        }else if(BallRigidBody.velocity.magnitude < minimumVelocity && renderer.enabled)
        {
            renderer.enabled = false;
        }
	}
}
