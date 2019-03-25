using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TrailRenderer))]

public class ToggleTrail : MonoBehaviour
{
    public float MinimumVelocity;

    private Rigidbody _ballRigidBody;
    private TrailRenderer _renderer;

    // Use this for initialization
    void Start () {
        _ballRigidBody = this.GetComponentInParent<Rigidbody>();
        _renderer = this.GetComponent<TrailRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(_ballRigidBody.velocity.magnitude > MinimumVelocity && !_renderer.enabled )
        {
            _renderer.enabled = true;
        }else if(_ballRigidBody.velocity.magnitude < MinimumVelocity && _renderer.enabled)
        {
            _renderer.enabled = false;
        }
	}
}
