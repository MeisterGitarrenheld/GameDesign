using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBCameraController : MonoBehaviour
{

    /// <summary>
    /// holds a reference to itself
    /// </summary>
    public static RBCameraController Instance;

    public Transform TargetLookAt;

    public float Distance = 20f;
    public float DistanceMin = 10f;
    public float DistanceMax = 30f;
    public float DistanceSmooth = 0.05f;
    public float DistanceResumeSmooth = 0.3f;
    public float X_MouseSensitivity = 5f;
    public float Y_MouseSensitivity = 5f;
    public float MouseWheelSensitivity = 50f;
    public float X_Smooth = 0.05f;
    public float Y_Smooth = 0.1f;
    public float Y_MinLimit = -20f;
    public float Y_MaxLimit = 80f;
    public float OcclusionDistanceStep = 0.5f;
    public int MaxOcclusionChecks = 10;

    private float mouseX = 0f;
    private float mouseY = 0f;
    private float velX = 0f;
    private float velY = 0f;
    private float velZ = 0f;
    private float velDistance = 0f;
    private float startDistance = 0f;
    private Vector3 position = Vector3.zero;
    private Vector3 desiredPosition = Vector3.zero;
    private float desiredDistance = 0f;
    private float distanceSmooth = 0f;
    private float preOccludedDistance = 0f;

    /// <summary>
    /// Used to Initialize
    /// </summary>
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// is called on startup
    /// </summary>
    void Start()
    {
        Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
        startDistance = Distance;
        Reset();
    }

    /// <summary>
    /// Is called after everything else is updated
    /// </summary>
    void LateUpdate()
    {
        if (TargetLookAt == null)
            return;

        HandlePlayerInput();

        int count = 0;
        do
        {
            CalculateDesiredPosition();
            count++;
        } while (CheckIfOccluded(count));


        UpdatePosition();
    }

    /// <summary>
    /// Handles the Userinput and convert it to fit into their min and max values
    /// </summary>
    void HandlePlayerInput()
    {
        var deadZone = 0.01f;


        mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;


        // clamp the mouseY rotation
        mouseY = RBCameraHelper.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);

        // clamps the distance of the camera to the target
        if (Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
        {
            desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity, DistanceMin, DistanceMax);
            preOccludedDistance = desiredDistance;
            distanceSmooth = DistanceSmooth;
        }

    }

    /// <summary>
    /// Calcs the Position of the Camera and moves to it smoothly
    /// </summary>
    void CalculateDesiredPosition()
    {
        // eval distance
        ResetDesiredDistance();
        Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, distanceSmooth);

        // calc desired position
        desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
    }

    /// <summary>
    /// Calcs the Position of the Camera
    /// </summary>
    /// <param name="rotationX"></param>
    /// <param name="rotationY"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        return TargetLookAt.position + rotation * direction;
    }

    /// <summary>
    /// Checks if there is anything between the camera and the player
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    bool CheckIfOccluded(int count)
    {
        bool isOccluded = false;

        float nearestDistance = CheckCameraPoints(TargetLookAt.position, desiredPosition);

        if (nearestDistance != -1)
        {
            if (count < MaxOcclusionChecks)
            {
                isOccluded = true;
                Distance -= OcclusionDistanceStep;

                if (Distance < 1f)
                    Distance = 1f;
            }
            else
            {
                //Distance = nearestDistance - Camera.main.nearClipPlane;
            }

            desiredDistance = Distance;
            distanceSmooth = DistanceResumeSmooth;
        }

        return isOccluded;
    }

    /// <summary>
    /// Check if one of the camera points collide with something then the player
    /// and return the nearst point to the player
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    float CheckCameraPoints(Vector3 from, Vector3 to)
    {
        float nearestDistance = -1;

        RaycastHit hitInfo;

        RBCameraHelper.ClipPlanePoints clipPlanePoints = RBCameraHelper.ClipPlaneAtNear(to);

        if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player")
            nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, to + transform.forward * -(GetComponent<Camera>()).nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;


        return nearestDistance;
    }

    /// <summary>
    /// Resets the Camera to the position it was before it was occluded
    /// </summary>
    void ResetDesiredDistance()
    {
        if (desiredDistance < preOccludedDistance)
        {
            Vector3 pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);
            float nearestDistance = CheckCameraPoints(TargetLookAt.position, pos);
            if (nearestDistance == -1 || nearestDistance > preOccludedDistance)
            {
                desiredDistance = preOccludedDistance;
            }
        }
    }

    /// <summary>
    /// Sets the calculated position of the camera
    /// </summary>
    void UpdatePosition()
    {
        var posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
        var posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
        var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);
        position = new Vector3(posX, posY, posZ);

        transform.position = position;

        transform.LookAt(TargetLookAt);
    }

    /// <summary>
    /// Resets the Camera
    /// </summary>
    public void Reset()
    {
        mouseX = 0;
        mouseY = -10;
        Distance = startDistance;
        desiredDistance = startDistance;
        preOccludedDistance = startDistance;
    }

    /// <summary>
    /// Setup the Camera to focus the CameraFocus object
    /// </summary>
    public static void SetupCamera(GameObject cameraFocus)
    {
        GameObject tempCamera;
        RBCameraController myCamera;

        // checks if there is a camera and set it as tempCamera
        if (Camera.main != null)
        {
            tempCamera = Camera.main.gameObject;
        }
        // if there is none => create a new one
        else
        {
            tempCamera = new GameObject("Main Camera");
            tempCamera.AddComponent<Camera>();
            tempCamera.tag = "MainCamera";
        }
        // apply Script to camera
        tempCamera.AddComponent<RBCameraController>();
        // set myCamera to the applied script of tempCamera
        myCamera = tempCamera.GetComponent<RBCameraController>();
        
        myCamera.TargetLookAt = cameraFocus.transform;
    }
}
