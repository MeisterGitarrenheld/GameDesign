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
    private float _maxBallShieldDistance = 15;

    [SerializeField]
    private float _maxSpeedBoostMultiplier = 5.0f;

    [SerializeField]
    private float _maxBallSpeed = 150.0f;

    [SerializeField]
    private GameObject BashParticleSystem;

    private float _minSpeedBoostFactor = 0.3f;
    private float _maxSpeedBoostCooldown = 0.5f;
    private float _speedBoostCooldown = 0.4f;

    private AudioSource _shootAudio;

    public bool PlasmaSpeedEffectActive = false;
    public bool IceSlowEffectActive = false;
    public RBAbilityIceSlow IceSlowAbility = null;

    void OnEnable()
    {
        _rotationPivotPoint = GameObject.Find("Camera Focus");
        _collider = GetComponent<BoxCollider>();
        _character = GetComponentInParent<RBCharacter>();
        _shootAudio = GetComponent<AudioSource>();
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
            // spawn particle effect
            SpawnShieldBashParticleAndAudio();

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
                        OnBallHit(ballTransform.gameObject, _maxSpeedBoostMultiplier * speedBoostFactor);
                    }
                }
            }
        }
    }

    private void SpawnShieldBashParticleAndAudio()
    {
        var particleSystem = Instantiate(BashParticleSystem);
        particleSystem.transform.parent = gameObject.transform;
        particleSystem.transform.localPosition = Vector3.zero;
        particleSystem.transform.localRotation = Quaternion.Euler(0, 0, 0);

        _shootAudio.Play();
    }

    void SendPhysicsUpdate(float speedMultiplier)
    {
        float curSpeed = RBPlayerController.CharController.velocity.magnitude;
        curSpeed = curSpeed > 1f ? curSpeed : 1f;

        RBPlayerPhysicsMessage msg = new RBPlayerPhysicsMessage()
        {
            objectHitDirection = transform.forward * Mathf.Min(_maxBallSpeed, curSpeed * speedMultiplier),
            PlayerHitName = _character.PlayerInfo.Username,
            PlayerTeamID = _character.PlayerInfo.Team
        };

        NetworkManager.singleton.client.Send((short)RBCustomMsgTypes.RBPlayerPhysicsMessage, msg);
    }

    void OnBallHit(GameObject ballObject, float speedMultiplier)
    {
        var speedMul = speedMultiplier;
        if (PlasmaSpeedEffectActive)
        {
            speedMul = _maxBallSpeed + 20;
        }

        if (IceSlowEffectActive)
        {
            IceSlowAbility.BallHitLocalShield();
        }

        var ballScript = ballObject.GetComponent<RBBall>();

        if (ballScript.IceEffectActive && ballScript.IceEffectSourceUsername != RBMatch.Instance.GetLocalUser().Username)
        {
            // apply slow
            RBPlayerController.Instance.SpeedBoost(0.5f, 3.0f);

            // remove ice effect from ball
            var playerObject = gameObject.FindPlayerByName(RBMatch.Instance.GetLocalUser().Username);
            var iceServerHandler = playerObject.GetComponent<RBAbilityIceSlowServer>();
            iceServerHandler.SetIceEffectOnBall(false);
        }

        SendPhysicsUpdate(speedMul);
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

        OnBallHit(other.gameObject, 1);
    }
}
