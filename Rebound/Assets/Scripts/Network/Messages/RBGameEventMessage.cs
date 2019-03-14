using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum GameEvent
{
    GameOver,
    Goal,
    PowerUpCollected
    //TeamChanged
    //...
}

public class RBGameEventMessage : MessageBase
{
    public const short game_event_message = (short)RBCustomMsgTypes.RBGameEventMessage;
    public GameEvent TriggeredEventType;
    public int TriggeredPlayerID;
    public int TriggeredTeamID;
}
