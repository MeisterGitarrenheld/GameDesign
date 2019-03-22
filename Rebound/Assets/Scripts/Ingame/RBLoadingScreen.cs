using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RBLoadingScreen : RBMonoBehaviourSingleton<RBLoadingScreen>
{
    private bool _isFadingOut = false;
    private string _targetScene = "";

    protected override void AwakeSingleton()
    {
        GetComponent<RBCanvasNavigation>().OnFadeOutEnded += RBLoadingScreen_OnFadeOutEnded;
    }

    public void ShowLoadingScreen(string targetScene)
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        _targetScene = targetScene;
    }

    public void HideLoadingScreen()
    {
        GetComponent<RBCanvasNavigation>().FadeOut();
    }

    private void RBLoadingScreen_OnFadeOutEnded(RBCanvasNavigation obj)
    {
        switch (_targetScene)
        {
            case "IngameScene":
                ARBArenaSetup.Instance?.PrepareForGameStart();
                break;
            case "LoginScene":
                PrepareLoginScene();
                break;
        }
    }

    private void PrepareLoginScene()
    {
        /*
        DestroyAllPlayerObjects();
        ResetNetworking();
        RBHideLockCursor.UnlockCursor();
        */

        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program
        Application.Quit(); //kill current process
    }

    private void DestroyAllPlayerObjects()
    {
        GameObject.FindGameObjectsWithTag("Player").ToList().ForEach(x => Destroy(x));
    }

    private void ResetNetworking()
    {
        RBNetworkManager.Instance.StopHost();
    }
}
