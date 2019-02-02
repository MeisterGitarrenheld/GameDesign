using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBErrorMessage : RBMonoBehaviourSingleton<RBErrorMessage> {
    public enum ErrorType { None, Warning, Error }
    
    public Text Headline;
    public Text Message;
    public RBAnimatedButton Button;

    [SerializeField]
    private Animator _animator;

    private CanvasGroup _cvGroup;

    protected override void AwakeSingleton()
    {
        _cvGroup = gameObject.GetComponent<CanvasGroup>();
        _cvGroup.alpha = 0;
        _cvGroup.interactable = false;
        _cvGroup.blocksRaycasts = false;
    }

    public void ShowError(string text, ErrorType errType, string headlineExtension = null)
    {
        _cvGroup.alpha = 1;
        _cvGroup.interactable = true;
        _cvGroup.blocksRaycasts = true;

        _animator.SetBool("open", true);
        Button.Show();

        switch (errType)
        {
            case ErrorType.Warning:
                Headline.text = "WARNING";
                break;
            case ErrorType.Error:
                Headline.text = "ERROR";
                break;
        }

        if(!string.IsNullOrEmpty(headlineExtension) && !string.IsNullOrWhiteSpace(headlineExtension))
            Headline.text += " - " + headlineExtension;
        Message.text = text;
    }

    public void CloseError()
    {
        gameObject.GetComponent<RBCanvasNavigation>().FadeOut();
        _animator.SetBool("open", false);
    }
}
