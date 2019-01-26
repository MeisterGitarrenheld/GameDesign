using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RBButton : ARBUIElement<Button> {

    protected override void OnFadeOutEnded(RBCanvasNavigation navPage)
    {
        gameObject.GetComponent<Button>().interactable = false;
    }

    protected override void OnFadeInStarted(RBCanvasNavigation navPage)
    {
        gameObject.GetComponent<Button>().interactable = true;
    }
}
