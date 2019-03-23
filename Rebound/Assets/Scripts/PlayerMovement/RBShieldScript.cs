using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(BoxCollider))]
public class RBShieldScript : MonoBehaviour
{
    private GameObject _rotationPivotPoint;
    private BoxCollider _collider;

    private RBCharacter _character;
    private float _maxBallShieldDistance = 2;

    [SerializeField]
    private float _maxSpeedBoostMultiplier = 5.0f;

    private float _minSpeedBoostFactor = 0.3f;
    private float _maxSpeedBoostCooldown = 0.5f;
    private float _speedBoostCooldown = 0.0f;

    void OnEnable()
    {
        _rotationPivotPoint = GameObject.Find("Camera Focus");
        //transform.rotation = Quaternion.Euler(0, 0, 0);
        _collider = GetComponent<BoxCollider>();
        _character = GetComponentInParent<RBCharacter>();
    }

    void Update()
    {
        CheckForBallSpeedBoost();

        RotateShieldWithCamera();
    }

    void CheckForBallSpeedBoost()
    {
        _speedBoostCooldown = Mathf.Max(_speedBoostCooldown - Time.deltaTime, 0.0f);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (_speedBoostCooldown == 0.0f)
            {
                _speedBoostCooldown = _maxSpeedBoostCooldown;

                var ballTransform = RBBall.BallTransformInstance;
                if (ballTransform != null)
                {
                    var ballPosition = ballTransform.position;
                    var hitPoint = _collider.ClosestPointOnBounds(ballPosition);
                    var distance = (hitPoint - ballPosition).magnitude;

                    if (distance < _maxBallShieldDistance)
                    {
                        var speedBoostFactor = Mathf.Max(1 - (distance / _maxBallShieldDistance), _minSpeedBoostFactor);
                        SendPhysicsUpdate(_maxSpeedBoostMultiplier * speedBoostFactor);
                    }
                }
            }
        }
    }

    void SendPhysicsUpdate(float speedMultiplier)
    {
        float curSpeed = RBPlayerController.CharController.velocity.magnitude;
        RBPlayerPhysicsMessage msg = new RBPlayerPhysicsMessage()
        {
            objectHitDirection = (transform.forward * (curSpeed > 1f ? curSpeed : 1f)) * speedMultiplier,
            PlayerHitName = _character.PlayerInfo.Username,
            PlayerTeamID = _character.PlayerInfo.Team
        };
        NetworkManager.singleton.client.Send((short)RBCustomMsgTypes.RBPlayerPhysicsMessage, msg);
    }
    private void RotateShieldWithCamera()
    {
        var playerObject = gameObject.FindTagInParentsRecursively("Player");
        var nwIdentity = playerObject.GetComponent<NetworkIdentity>();
        if (!nwIdentity.isLocalPlayer) return;

        var rotationOffsetDegree = 20.0f;

        // the distance between player and shield
        var shieldDistance = 7f;

        // get the direction between camera and pivot and negate it, because the camera is behind the player and the
        // shield should be in front of him
        var shieldMoveDirection = (Camera.main.transform.position - _rotationPivotPoint.transform.position).normalized * -1;

        // apply the distance to the move vector
        var shieldMoveVector = shieldMoveDirection * shieldDistance;

        // move the shield position from the player in his look direction by the fixed distance
        var newShieldPos = _rotationPivotPoint.transform.position + shieldMoveVector;

        transform.position = new Vector3(newShieldPos.x, newShieldPos.y + 2, newShieldPos.z);

        // rotate the shield in the look position of the player
        transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x - rotationOffsetDegree, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!transform.parent.GetComponent<NetworkIdentity>().isLocalPlayer || other.tag != "Ball")
            return;
        
        SendPhysicsUpdate(1);
    }
}
