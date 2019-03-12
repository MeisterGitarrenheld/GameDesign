using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBNetworkMovementSync : NetworkBehaviour {

    public string objectName;
    public string playerID;

    public bool canSendNetworkMovement;
    public float speed;
    public float networkSendRate = 5;
    public float timeBetweenMovementStart;
    public float timeBetweenMovementEnd;

    public bool isLerpingPosition;
    public bool isLerpingRotation;
    public Vector3 realPosition;
    public Quaternion realRotation;
    public Vector3 lastRealPosition;
    public Quaternion lastRealRotation;
    public float timeStartedLerping;
    public float timeToLerp;

    private void OnEnable()
    {
        playerID = objectName + GetComponent<NetworkIdentity>().netId.ToString();
        transform.name = playerID;
        print(RBGameManager.Instance);
        print(RBGameManager.Instance.ConnectedPlayers);
        RBGameManager.Instance.AddPlayerToConnectedPlayers(playerID, gameObject);

        if (isLocalPlayer)
        {
            RBGameManager.Instance.SetLocalPlayerID(playerID);

            canSendNetworkMovement = false;
            RegisterNetworkMessages();
        }
        else
        {
            isLerpingPosition = false;
            isLerpingRotation = false;

            realPosition = transform.position;
            realRotation = transform.rotation;
        }
    }

    private void RegisterNetworkMessages()
    {
        NetworkManager.singleton.client.RegisterHandler((short)RBCustomMsgTypes.RBPlayerMovementMessage, OnReceiveMovementMessage);
    }

    private void OnReceiveMovementMessage(NetworkMessage _message)
    {
        RBPlayerMovementMessage _msg = _message.ReadMessage<RBPlayerMovementMessage>();

        if (_msg.objectTransformName != transform.name)
        {
            RBGameManager.Instance.ConnectedPlayers[_msg.objectTransformName].GetComponent<RBNetworkMovementSync>().ReceiveMovementMessage(_msg.objectPosition, _msg.objectRotation, _msg.time);
        }
    }

    public void ReceiveMovementMessage(Vector3 _position, Quaternion _rotation, float _timeToLerp)
    {
        lastRealPosition = realPosition;
        lastRealRotation = realRotation;
        realPosition = _position;
        realRotation = _rotation;
        timeToLerp = _timeToLerp;

        if (realPosition != transform.position)
        {
            isLerpingPosition = true;
        }

        if (realRotation.eulerAngles != transform.rotation.eulerAngles)
        {
            isLerpingRotation = true;
        }

        timeStartedLerping = Time.time;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            UpdatePlayerMovement();
        }
        else
        {
            return;
        }
    }
    

    private void UpdatePlayerMovement()
    {
        if (!canSendNetworkMovement)
        {
            canSendNetworkMovement = true;
            StartCoroutine(StartNetworkSendCooldown());
        }
    }

    private IEnumerator StartNetworkSendCooldown()
    {
        timeBetweenMovementStart = Time.time;
        yield return new WaitForSeconds((1 / networkSendRate));
        SendNetworkMovement();
    }

    private void SendNetworkMovement()
    {
        timeBetweenMovementEnd = Time.time;
        SendMovementMessage(playerID, transform.position, transform.rotation, (timeBetweenMovementEnd - timeBetweenMovementStart));
        canSendNetworkMovement = false;
    }

    public void SendMovementMessage(string _playerID, Vector3 _position, Quaternion _rotation, float _timeTolerp)
    {
        RBPlayerMovementMessage _msg = new RBPlayerMovementMessage()
        {
            objectPosition = _position,
            objectRotation = _rotation,
            objectTransformName = _playerID,
            time = _timeTolerp
        };

        NetworkManager.singleton.client.Send((short)RBCustomMsgTypes.RBPlayerMovementMessage, _msg);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            NetworkLerp();
        }
    }

    private void NetworkLerp()
    {
        if (isLerpingPosition)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            transform.position = Vector3.Lerp(lastRealPosition, realPosition, lerpPercentage);
        }

        if (isLerpingRotation)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            transform.rotation = Quaternion.Lerp(lastRealRotation, realRotation, lerpPercentage);
        }
    }
}
