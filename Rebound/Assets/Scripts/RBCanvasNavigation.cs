﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RBCanvasNavigation : MonoBehaviour
{
    public event Action OnFadeInStarted;
    public event Action OnFadeInEnded;
    private static event Action _onFadeInStarted;
    private static event Action _onFadeOutStarted;

    public static RBCanvasNavigation CurrentPage;
    private static RBCanvasNavigation _nextPage = null;

    public RBCanvasNavigation Parent = null;
    public RBCanvasNavigation Previous = null;
    private List<RBCanvasNavigation> _children = new List<RBCanvasNavigation>();

    public const float FADE_DURATION = .3f;

    private CanvasGroup _cvGroup;
    private float _fadeInState = 0.0f;
    private float _fadeOutState = 0.0f;

    /// <summary>
    /// Registers the page as child, if there is a parent
    /// and joins the static fade events.
    /// </summary>
    void Awake()
    {
        _cvGroup = gameObject.GetComponent<CanvasGroup>();

        if (Parent != null)
        {
            Parent._children.Add(this);
        }

        _onFadeOutStarted += RBCanvasNavigation_OnFadeOutStarted;
        _onFadeInStarted += RBCanvasNavigation__onFadeInStarted;
    }

    /// <summary>
    /// Called when a fade out prcess has finished and the next
    /// fade in process should start.
    /// </summary>
    private void RBCanvasNavigation__onFadeInStarted()
    {
        if (_fadeInState == 0.0f)
        {
            if (_children.Contains(CurrentPage))
            {
                FadeIn();
            }
        }
    }

    /// <summary>
    /// Called when any page is shown.
    /// </summary>
    private void RBCanvasNavigation_OnFadeOutStarted()
    {
        if (_fadeOutState == 0.0f)
        {
            if (!_children.Contains(_nextPage) && _nextPage != this)
            {
                FadeOut();
            }
        }
    }

    /// <summary>
    /// Processes fading effects
    /// </summary>
    void Update()
    {
        if (_fadeOutState > 0.0f)
        {
            _cvGroup.alpha -= Time.deltaTime / FADE_DURATION;
            _fadeOutState -= Time.deltaTime;

            // show the next page, if the previous page is faded out
            if (_fadeOutState <= 0.0f)
            {
                if (_nextPage != null)
                {   // show all pages that are part of the next screen
                    CurrentPage = _nextPage;
                    _nextPage.FadeIn();
                    _nextPage = null;
                    _onFadeInStarted?.Invoke();
                }

                _fadeOutState = 0.0f;

                _cvGroup.alpha = 0.0f;
                _cvGroup.blocksRaycasts = false;
                _cvGroup.interactable = false;
            }
        }
        else if (_fadeInState > 0.0f)
        {   // prevent spamming the buttons
            if (CurrentPage == this || _children.Contains(CurrentPage))
            {
                _cvGroup.alpha += Time.deltaTime / FADE_DURATION;
                _fadeInState -= Time.deltaTime;

                if (_fadeInState <= 0.0f)
                {
                    _fadeInState = 0.0f;

                    _cvGroup.alpha = 1.0f;
                    _cvGroup.blocksRaycasts = true;
                    _cvGroup.interactable = true;
                    OnFadeInEnded?.Invoke();
                }
            }
            else _fadeInState = 0.0f;
        }
    }

    /// <summary>
    /// Shows the page using an fade in effect
    /// </summary>
    public void FadeIn()
    {
        _fadeInState = FADE_DURATION;
        OnFadeInStarted?.Invoke();
    }

    /// <summary>
    /// Hides the page using an fade out effect
    /// </summary>
    public void FadeOut()
    {
        _fadeOutState = FADE_DURATION;
    }

    /// <summary>
    /// Switches two menu pages. Hides the current one and displays the next one.
    /// </summary>
    /// <param name="next"></param>
    public void Show()
    {
        if (CurrentPage == null)
        {
            CurrentPage = this;
            CurrentPage.FadeIn();
        }
        else
        {
            _nextPage = this;
            // hide all pages that are not part of the next screen
            _onFadeOutStarted?.Invoke();
            CurrentPage.FadeOut();
        }
    }

    /// <summary>
    /// Returns to the previous page.
    /// </summary>
    public void PageBack()
    {
        _nextPage = CurrentPage?.Previous;
        CurrentPage?.FadeOut();
    }
}
