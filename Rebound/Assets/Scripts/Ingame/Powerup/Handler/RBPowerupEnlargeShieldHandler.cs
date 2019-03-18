using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBPowerupEnlargeShieldHandler : ARBPowerupActionHandler
{
    private RBPowerupEnlargeShieldHandlerServer _serverHandler;

    public override void DoAction(string playerName)
    {
        var playerObject = gameObject.FindPlayerByName(RBMatch.Instance.GetLocalUser().Username);
        _serverHandler = playerObject.GetComponent<RBPowerupEnlargeShieldHandlerServer>();
        _serverHandler.EnlargeShield();
        TriggerOnComplete();
    }
}