using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class RBIngameUiManager : NetworkBehaviour
{
    public GameObject IngameUI;

    public Text TimeUI;
    public Text[] TeamScorePanels;

    private int[] _teamScore;

    [SyncVar]
    private float _timer;

    void Start()
    {

        NetworkManager.singleton.client.RegisterHandler((short)RBCustomMsgTypes.RBGameEventMessage, OnRecieveGameEventMessage);
        _teamScore = new int[4];

        if(isServer)
            _timer = 10 * 60;

        Array.ForEach(TeamScorePanels, tsp => tsp.text = "0");
    }

    private void OnRecieveGameEventMessage(NetworkMessage nwMessage)
    {
        RBGameEventMessage msg = nwMessage.ReadMessage<RBGameEventMessage>();
        switch (msg.TriggeredEventType)
        {
            case GameEvent.Goal:
                var goalTeam = int.Parse(msg.GameEventInfo) % 2;
                _teamScore[goalTeam] += 1;
                TeamScorePanels[goalTeam].text = _teamScore[goalTeam].ToString();
                break;
            case GameEvent.PowerUpCollected: break;
            default: break;
        }
    }

    void Update()
    {
        if(isServer)
            _timer -= Time.deltaTime;

        string minutes = ((int)(_timer / 60f)).ToString();
        string seconds = ((int)(_timer % 60)).ToString();
        if (seconds.Length == 1)
            seconds = "0" + seconds;
        TimeUI.text = minutes + ":" + seconds;
    }
}
