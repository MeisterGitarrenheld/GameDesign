using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBPlayerAnimator : MonoBehaviour {

    public static RBPlayerAnimator Instance;

    void Awake()
    {
        Instance = this;
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
}
