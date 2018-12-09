using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPlayerMovement : NetworkBehaviour
{
    public float MaxMoveSpeed;
    public float RotationSpeed;
    public float ShieldMoveSpeed;
    public Vector3 RotationPivot;
    public float MaxShieldHeight;
    public float MinShieldHeight;
    public float ShieldRotationRate;

    private Rigidbody _rb;
    private Vector3 _movementVector;

    //Shield Variables
    private Transform _shieldTransform;
    private Vector3 _shieldOffset;
    private float _initYOffset;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (!isLocalPlayer) return;
        
        Cursor.lockState = CursorLockMode.Locked;

        _rb = GetComponent<Rigidbody>();
        _shieldTransform = transform.Find("Shield");
        _shieldOffset = _shieldTransform.localPosition;
        _initYOffset = _shieldOffset.y;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        float xInput = Input.GetAxis("Horizontal") * MaxMoveSpeed;
        float zInput = Input.GetAxis("Vertical") * MaxMoveSpeed;
        float xMouseDelta = Input.GetAxis("Mouse X") * RotationSpeed;
        float yMouseDelta = Input.GetAxis("Mouse Y") * ShieldMoveSpeed;

        _movementVector = transform.right * xInput;
        _movementVector += transform.forward * zInput;

        _shieldOffset.y = Mathf.Clamp(_shieldOffset.y + yMouseDelta, MinShieldHeight, MaxShieldHeight);
        _shieldTransform.localPosition = _shieldOffset;
        //_shieldTransform.LookAt(_shieldTransform.position + (_shieldTransform.position - (transform.position + RotationPivot)));
        _shieldTransform.localRotation = Quaternion.Euler((_shieldOffset.y - _initYOffset) * -ShieldRotationRate, 0, 0);

        _rb.velocity = _movementVector;
        _rb.angularVelocity = new Vector3(0, xMouseDelta, 0);


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState != CursorLockMode.None ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
