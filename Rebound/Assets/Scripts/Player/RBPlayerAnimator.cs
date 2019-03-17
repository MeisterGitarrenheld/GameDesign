using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBPlayerAnimator : MonoBehaviour {

    public static RBPlayerAnimator Instance;

    private Animator _animator; 

    void Awake()
    {
        Instance = this;
        _animator = GetComponentInChildren<Animator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        DetermineCurrentState();
	}

    /// <summary>
    /// Determine the current move direction of the player
    /// </summary>
    public void DetermineCurrentMoveDirection()
    {
        var forward = false;
        var backward = false;
        var left = false;
        var right = false;

        if (RBPlayerMovement.Instance.MoveVector.z > 0)
            forward = true;
        if (RBPlayerMovement.Instance.MoveVector.z < 0)
            backward = true;
        if (RBPlayerMovement.Instance.MoveVector.x > 0)
            right = true;
        if (RBPlayerMovement.Instance.MoveVector.x < 0)
            left = true;

        // sets the direction where the player is moving
        if (forward)
        {
            if (left)
                RBPlayerController.Instance.MoveDirection = RBPlayerController.Direction.LeftForward;
            else if (right)
                RBPlayerController.Instance.MoveDirection = RBPlayerController.Direction.RightForward;
            else
                RBPlayerController.Instance.MoveDirection = RBPlayerController.Direction.Forward;
        }
        else if (backward)
        {
            if (left)
                RBPlayerController.Instance.MoveDirection = RBPlayerController.Direction.LeftBackward;
            else if (right)
                RBPlayerController.Instance.MoveDirection = RBPlayerController.Direction.RightBackward;
            else
                RBPlayerController.Instance.MoveDirection = RBPlayerController.Direction.Backward;
        }
        else if (left)
            RBPlayerController.Instance.MoveDirection = RBPlayerController.Direction.Left;
        else if (right)
            RBPlayerController.Instance.MoveDirection = RBPlayerController.Direction.Right;
        else
            RBPlayerController.Instance.MoveDirection = RBPlayerController.Direction.Stationary;
    }

    public void DetermineCurrentState()
    {
        var state = RBPlayerController.Instance.State;

        if(!RBPlayerController.CharController.isGrounded &&
            state != RBPlayerController.CharacterState.Falling &&
            state != RBPlayerController.CharacterState.Jumping)
        {
            state = RBPlayerController.CharacterState.Falling;
        }

        // moving
        if(state != RBPlayerController.CharacterState.Falling &&
            state != RBPlayerController.CharacterState.Jumping)
        {
            switch(RBPlayerController.Instance.MoveDirection)
            {
                case RBPlayerController.Direction.Stationary:
                    RBPlayerController.Instance.State = RBPlayerController.CharacterState.Idle;
                    break;

                case RBPlayerController.Direction.Forward:
                case RBPlayerController.Direction.Left:
                case RBPlayerController.Direction.Right:
                case RBPlayerController.Direction.LeftForward:
                case RBPlayerController.Direction.RightForward:
                    RBPlayerController.Instance.State = RBPlayerController.CharacterState.Running;
                    break;

                case RBPlayerController.Direction.Backward:
                case RBPlayerController.Direction.LeftBackward:
                case RBPlayerController.Direction.RightBackward:
                    RBPlayerController.Instance.State = RBPlayerController.CharacterState.WalkingBackwards;
                    break;
            }
        }
    }

    private RBPlayerController.Direction _lastDir = RBPlayerController.Direction.Stationary;
    public void UpdateAnimation(RBPlayerController.Direction moveDir)
    {
        if (moveDir == _lastDir || _animator == null) return;

        switch(_lastDir = moveDir)
        {
            case RBPlayerController.Direction.Stationary:
                _animator.SetTrigger("Idle");
                break;
            case RBPlayerController.Direction.Forward:
                _animator.SetTrigger("Forward");
                break;
            case RBPlayerController.Direction.LeftForward:
                _animator.SetTrigger("LeftForward");
                break;
            case RBPlayerController.Direction.RightForward:
                _animator.SetTrigger("RightForward");
                break;
            case RBPlayerController.Direction.Left:
                _animator.SetTrigger("Left");
                break;
            case RBPlayerController.Direction.Right:
                _animator.SetTrigger("Right");
                break;
            case RBPlayerController.Direction.Backward:
                _animator.SetTrigger("Backward");
                break;
            case RBPlayerController.Direction.LeftBackward:
                _animator.SetTrigger("LeftBackward");
                break;
            case RBPlayerController.Direction.RightBackward:
                _animator.SetTrigger("RightBackward");
                break;
        }
    }
}
