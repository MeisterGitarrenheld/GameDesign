using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBAnimatedButton : RBButton
{
    private Animator _animator;

    protected override void Awake()
    {
        base.Awake();
        _animator = gameObject.GetComponent<Animator>();
    }

    protected override void OnFadeOutEnded(RBCanvasNavigation navPage)
    {
        base.OnFadeOutEnded(navPage);
        _animator.SetTrigger("Normal");
    }

    public void Show()
    {
        /*
        gameObject.SetActive(true);
        _animator.SetTrigger("Normal");
        */
        Debug.Log("ENABLE");
        UIComponent.interactable = true;
    }

    public void Disable()
    {
        Debug.Log("DISABLE");
        UIComponent.interactable = false;
    }
}
