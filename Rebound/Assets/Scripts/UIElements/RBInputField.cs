using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBInputField : ARBUIElement<InputField> {

    protected override void OnFadeOutEnded(RBCanvasNavigation navPage)
    {
        UIComponent.text = string.Empty;
    }    
}
