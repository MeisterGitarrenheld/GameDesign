using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBPlayerController : MonoBehaviour
{
    public enum Direction
    {
        Stationary, Forward, Backward, Left, Right, LeftForward, RightForward, LeftBackward, RightBackward
    }

    public enum CharacterState
    {
        None, Idle, Running, WalkingBackwards, Jumping, Falling, ActionLocked
    }

    /// <summary>
    /// Reference to the CharacterController
    /// </summary>
    public static CharacterController CharController;

    public static RBPlayerController Instance;

    public Direction MoveDirection { get; set; }
    public CharacterState State { get; set; }

    public void Awake()
    {
        CharController = GetComponent<CharacterController>();
        Instance = this;
        // todo setup camera
    }
    	
	// Update is called once per frame
	void Update ()
    {
        if (Camera.main == null)
            return;

        if (!RBHideLockCursor.CursorLocked)
            return;

        if (ARBArenaSetup.Instance != null && ARBArenaSetup.Instance.PlayerMovementLocked)
            return;

        GetUserInput();
        HandleActionInput();
                
        RBPlayerMovement.Instance.UpdateMovement();
	}

    private void GetUserInput()
    {
        var deadZone = .1f;

        RBPlayerMovement.Instance.VerticalVelocity = RBPlayerMovement.Instance.MoveVector.y;
        RBPlayerMovement.Instance.MoveVector = Vector3.zero;

        var vertInput = Input.GetAxis("Vertical");
        var horInput = Input.GetAxis("Horizontal");

        if(CharController.isGrounded)

        // Adds the vertical movement to the vector
        if (vertInput > deadZone || vertInput < -deadZone)
        {
            RBPlayerMovement.Instance.MoveVector += new Vector3(0, 0, vertInput);
        }
        // Adds the horizontal movement to the vector
        if (horInput > deadZone || horInput < -deadZone)
        {
            RBPlayerMovement.Instance.MoveVector += new Vector3(horInput, 0, 0);
        }

        RBPlayerAnimator.Instance.DetermineCurrentMoveDirection();
        RBPlayerAnimator.Instance.UpdateAnimation(MoveDirection);
    }

    private void HandleActionInput()
    {
        if(Input.GetButton("Jump"))
        {
            Jump();
        }
    }

    private void Jump()
    {
        RBPlayerMovement.Instance.Jump();
    }

    private Coroutine _speedBoost;
    public void SpeedBoost(float multiplier, float duration)
    {
        if (RBPlayerMovement.Instance.SpeedMultiplier > multiplier) return;

        RBPlayerMovement.Instance.SpeedMultiplier = multiplier;

        if (_speedBoost != null)
        {
            StopCoroutine(_speedBoost);
        }
        _speedBoost = StartCoroutine(SpeedBoostWaitForEnd(duration));
    }

    private IEnumerator SpeedBoostWaitForEnd(float duration)
    {
        yield return new WaitForSeconds(duration);

        foreach(var ps in gameObject.GetComponentsInChildren<ParticleSystem>())
            Destroy(ps);

        RBPlayerMovement.Instance.SpeedMultiplier = 1;
    }
}
