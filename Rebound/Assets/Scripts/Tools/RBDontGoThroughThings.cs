using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class RBDontGoThroughThings : MonoBehaviour {

    // Careful when settings this to true - it might cause double
    // event to be fired - but it won't pass through the trigger
    public bool SendTriggerMessage = false;

    public LayerMask LayerMask = -1; // make sure we aren't in this layer
    public float SkinWidth = 0.1f; // probalby doesn't need to be changed

    private float _minimumExtent;
    private float _partialExtent;
    private float _sqrMinimumExtent;
    private Vector3 _previousPosition;
    private Rigidbody _myRigidbody;
    private Collider _myCollider;

	// Use this for initialization
	void Start ()
    {
        _myRigidbody = GetComponent<Rigidbody>();
        _myCollider = GetComponent<Collider>();
        _previousPosition = _myRigidbody.position;
        _minimumExtent = Mathf.Min(Mathf.Min(_myCollider.bounds.extents.x, _myCollider.bounds.extents.y), _myCollider.bounds.extents.z);
        _partialExtent = _minimumExtent * (1.0f - SkinWidth);
        _sqrMinimumExtent = _minimumExtent * _minimumExtent;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // have we moved more than our minimum extend?
        Vector3 movementThisStep = _myRigidbody.position - _previousPosition;
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if(movementSqrMagnitude > _sqrMinimumExtent)
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            RaycastHit hitInfo;

            // check for obstuctions we might have missed
            if(Physics.Raycast(_previousPosition, movementThisStep, out hitInfo, movementMagnitude, LayerMask.value))
            {
                if (!hitInfo.collider)
                    return;

                if (hitInfo.collider.isTrigger)
                    hitInfo.collider.SendMessage("OnTriggerEnter", _myCollider, SendMessageOptions.DontRequireReceiver);

                if (!hitInfo.collider.isTrigger)
                    _myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * _partialExtent;
            }
        }

        _previousPosition = _myRigidbody.position;
	}
}
