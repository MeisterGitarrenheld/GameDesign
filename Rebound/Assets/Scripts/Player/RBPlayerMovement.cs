using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBPlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Holds reference to itself
    /// </summary>
    public static RBPlayerMovement Instance;

    /// <summary>
    /// Public Variables
    /// </summary>
    public float ForwardSpeed = 20.0f;
    public float BackwardSpeed = 15.0f;
    public float StrafingSpeed = 20.0f;
    public float JumpSpeed = 25.0f;
    public float Gravity = 35.0f;
    public float TerminalVelocity = 35.0f;

    /// <summary>
    /// Vector for the durection of the movement
    /// </summary>
    public Vector3 MoveVector { get; set; }

    public float VerticalVelocity { get; set; }

    private bool isJumping;
    private Vector3 prevMoveVector;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Update method for the movement
    /// </summary>
    public void UpdateMovement()
    {
        RotateCharacterWithCamera();
        ProcessInput();
    }

    /// <summary>
    /// Converts the input to units per seconds and moves the player
    /// </summary>
    void ProcessInput()
    {
        // checks if the player has finished jumping
        if (RBPlayerController.CharController.isGrounded)
            isJumping = false;

        // transform move vector to world space
        MoveVector = transform.TransformDirection(MoveVector);

        // normalize move vector
        if (MoveVector.magnitude > 1)
            MoveVector = Vector3.Normalize(MoveVector);

        // multiplay movespeed
        MoveVector *= MoveSpeed();

        // reapply vertical velocity (y direction) to move vectory
        MoveVector = new Vector3(MoveVector.x, VerticalVelocity, MoveVector.z);

        // apply gravity
        ApplyGravity();

        // move character in world space
        RBPlayerController.CharController.Move((isJumping ? new Vector3(prevMoveVector.x, MoveVector.y, prevMoveVector.z) : MoveVector) * Time.deltaTime);

        // checks if the player starts jumping and stores the inital move vector
        if (!RBPlayerController.CharController.isGrounded && !isJumping)
        {
            prevMoveVector = MoveVector;
            isJumping = true;
        }
    }

    private void ApplyGravity()
    {
        if (MoveVector.y > -TerminalVelocity)
            MoveVector = new Vector3(MoveVector.x, MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);

        if (RBPlayerController.CharController.isGrounded && MoveVector.y < -1)
            MoveVector = new Vector3(MoveVector.x, MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);
    }

    public void Jump()
    {
        if (RBPlayerController.CharController.isGrounded)
            VerticalVelocity = JumpSpeed;
    }

    private void RotateCharacterWithCamera()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
    }

    /// <summary>
    /// Calculates the movespeed for the move vector
    /// </summary>
    /// <returns></returns>
    private float MoveSpeed()
    {
        float moveSpeed = 0;

        switch (RBPlayerController.Instance.MoveDirection)
        {
            case RBPlayerController.Direction.Stationary:
                moveSpeed = 0;
                break;

            case RBPlayerController.Direction.Forward:
            case RBPlayerController.Direction.LeftForward:
            case RBPlayerController.Direction.RightForward:
                moveSpeed = ForwardSpeed;
                break;

            case RBPlayerController.Direction.Backward:
            case RBPlayerController.Direction.LeftBackward:
            case RBPlayerController.Direction.RightBackward:
                moveSpeed = BackwardSpeed;
                break;

            case RBPlayerController.Direction.Left:
                moveSpeed = StrafingSpeed;
                break;
            case RBPlayerController.Direction.Right:
                moveSpeed = StrafingSpeed;
                break;
        }

        return moveSpeed;
    }
}
