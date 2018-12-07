using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBErrorMessage : RBMonoBehaviourSingleton<RBErrorMessage> {
    public enum ErrorType { None, Warning, Error }
    
    public Text Headline;
    public Text Message;

    private CanvasGroup _cvGroup;

    protected override void AwakeSingleton()
    {
        _cvGroup = gameObject.GetComponent<CanvasGroup>();
        _cvGroup.alpha = 0;
        _cvGroup.interactable = false;
        _cvGroup.blocksRaycasts = false;
    }

    public void ShowError(string text, ErrorType errType)
    {
        _cvGroup.alpha = 1;
        _cvGroup.interactable = true;
        _cvGroup.blocksRaycasts = true;

        switch (errType)
        {
            case ErrorType.Warning:
                Headline.text = "WARNING";
                break;
            case ErrorType.Error:
                Headline.text = "ERROR";
                break;
        }

        Message.text = text;
    }

    public void CloseError()
    { 
        _cvGroup.alpha = 0;
        _cvGroup.interactable = false;
        _cvGroup.blocksRaycasts = false;
    }
}
