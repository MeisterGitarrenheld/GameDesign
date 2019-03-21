using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RBLoadingScreen : NetworkBehaviour
{
    public ARBArenaSetup ArenaSetup;
    public GameObject LoadingAnim;
    public float LoadingAnimDelayFrames = 5;

    private Animation[] _animations;
    private int _currFrameCount = 0;
    private int _maxAnimations;

    [SyncVar]
    public float SceneLoadDurationSeconds = 10.0f;

    private bool _isFadingOut = false;

    void Awake()
    {
        GetComponent<RBCanvasNavigation>().OnFadeOutEnded += RBLoadingScreen_OnFadeOutEnded;
        _animations = gameObject.GetComponentsInChildren<Animation>();
        _maxAnimations = _animations.Length;
    }

    void FixedUpdate()
    {
        if (_currFrameCount / 5 >= _maxAnimations) return;

        if(_currFrameCount % LoadingAnimDelayFrames == 0)
        {
            _animations[_currFrameCount / 5].Play();
        }

        _currFrameCount++;
    }

    private void RBLoadingScreen_OnFadeOutEnded(RBCanvasNavigation obj)
    {
        ArenaSetup.PrepareForGameStart();
        Destroy(gameObject);
    }

    void Update()
    {
        if(isServer)
        {
            SceneLoadDurationSeconds -= Time.deltaTime;
        }

        if (SceneLoadDurationSeconds <= 0.0f && !_isFadingOut)
        {
            _isFadingOut = true;
            GetComponent<RBCanvasNavigation>().FadeOut();
        }
    }
}
