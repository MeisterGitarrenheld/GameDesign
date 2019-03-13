using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RBShieldScript : MonoBehaviour
{
    private GameObject _rotationPivotPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!transform.parent.GetComponent<NetworkIdentity>().isLocalPlayer || other.tag != "Ball")
            return;
        print("Hit!");
        RBPlayerPhysicsMessage msg = new RBPlayerPhysicsMessage()
        {
            objectHitDirection = transform.forward * RBPlayerController.CharController.velocity.magnitude
        };
        NetworkManager.singleton.client.Send((short)RBCustomMsgTypes.RBPlayerPhysicsMessage, msg);
    }

    void Update()
    {
        RotateShieldWithCamera();
    }

    void OnEnable()
    {
        _rotationPivotPoint = GameObject.Find("Camera Focus");
    }

    private void RotateShieldWithCamera()
    {
        var playerObject = gameObject.FindTagInParentsRecursively("Player");
        var nwIdentity = playerObject.GetComponent<NetworkIdentity>();
        if (!nwIdentity.isLocalPlayer) return;

        // the distance between player and shield
        var shieldDistance = 7f;

        // get the direction between camera and pivot and negate it, because the camera is behind the player and the
        // shield should be in front of him
        var shieldMoveDirection = (Camera.main.transform.position - _rotationPivotPoint.transform.position).normalized * -1;

        // apply the distance to the move vector
        var shieldMoveVector = shieldMoveDirection * shieldDistance;

        // move the shield position from the player in his look direction by the fixed distance
        transform.position = _rotationPivotPoint.transform.position + shieldMoveVector;

        // rotate the shield in the look position of the player
        transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        
    }
}
