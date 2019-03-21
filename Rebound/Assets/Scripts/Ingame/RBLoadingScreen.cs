using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBLoadingScreen : MonoBehaviour
{
    public static RBLoadingScreen Instance;

    private bool _isFadingOut = false;

    void Awake()
    {
        Instance = this;
        GetComponent<RBCanvasNavigation>().OnFadeOutEnded += RBLoadingScreen_OnFadeOutEnded;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowLoadingScreen()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void HideLoadingScreen()
    {
        GetComponent<RBCanvasNavigation>().FadeOut();
    }

    private void RBLoadingScreen_OnFadeOutEnded(RBCanvasNavigation obj)
    {
        ARBArenaSetup.Instance?.PrepareForGameStart();
    }
}
