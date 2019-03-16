using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class RBIngameUiManager : MonoBehaviour {


    public GameObject IngameUI;

    public Text TimeUI;
    public Text[] TeamScorePanels;

    private int[] TeamScore;

    private float timer;
    
    void Start ()
    {

        NetworkManager.singleton.client.RegisterHandler((short)RBCustomMsgTypes.RBGameEventMessage, OnRecieveGameEventMessage);
        TeamScore = new int[4];
        timer = 15 * 60;

        Array.ForEach(TeamScorePanels, tsp => tsp.text = "0");
    }

    private void OnRecieveGameEventMessage(NetworkMessage _message)
    {
        RBGameEventMessage _msg = _message.ReadMessage<RBGameEventMessage>();
        switch (_msg.TriggeredEventType)
        {
            case GameEvent.Goal:
                if (_msg.TriggeredTeamID > 0)
                {
                    TeamScore[_msg.TriggeredTeamID - 1] += 1;
                    TeamScorePanels[_msg.TriggeredTeamID - 1].text = TeamScore[_msg.TriggeredTeamID - 1].ToString();
                }
                break;
            case GameEvent.PowerUpCollected: break;
            default: break;
        }
    }

    void Update () {
        timer -= Time.deltaTime;
        string minutes = ((int)(timer / 60f)).ToString();
        string seconds = ((int)(timer % 60)).ToString();
        if (seconds.Length == 1)
            seconds = "0" + seconds;
        TimeUI.text = minutes + ":" + seconds;
    }



}
