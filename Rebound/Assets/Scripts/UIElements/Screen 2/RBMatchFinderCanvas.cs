using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBMatchFinderCanvas : RBCanvas
{
    public Text UsernameDisp;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnFadeInStarted(RBCanvasNavigation navPage)
    {
        base.OnFadeInStarted(navPage);

        UsernameDisp.text = RBLocalUser.Instance.Username;
    }
}
