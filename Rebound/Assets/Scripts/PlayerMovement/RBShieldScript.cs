using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBShieldScript : MonoBehaviour {

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

    void Start()
    {
        _rotationPivotPoint = GameObject.Find("Camera Focus");
    }

    void Update()
    {
        RotateShieldWithCamera();
    }

    private void RotateShieldWithCamera()
    {
        var dist = 2.0f;
        var vec = _rotationPivotPoint.transform.position - Camera.main.transform.position;
        

        transform.localPosition = _rotationPivotPoint.transform.position + (Vector3.Normalize(vec) * dist);
        transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
