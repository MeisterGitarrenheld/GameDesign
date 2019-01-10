using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBInputField : ARBUIElement<InputField> {

    public bool ContainsText { get { return !string.IsNullOrEmpty(UIComponent.text) && !string.IsNullOrWhiteSpace(UIComponent.text); } }

    protected override void OnFadeOutEnded(RBCanvasNavigation navPage)
    {
        UIComponent.text = string.Empty;
    }    
}
