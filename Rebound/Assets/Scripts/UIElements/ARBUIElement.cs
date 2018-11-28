using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ARBUIElement<T> : MonoBehaviour {

    protected RBCanvasNavigation CanvasNav;
    [HideInInspector]
    public T UIComponent;

    /// <summary>
    /// This method should be used for initialization as derived classes will call this method
    /// instead of Awake.
    /// </summary>
    protected virtual void Awake()
    {
        CanvasNav = gameObject.FindComponentInObjectOrParent<RBCanvasNavigation>();

        UIComponent = GetComponent<T>();
        if (UIComponent == null)
            throw new System.Exception(this.GetType() + " needs a " + typeof(T).Name + " component to use the UI-Script.");

        // join events
        CanvasNav.OnFadeInStarted += OnFadeInStarted;
        CanvasNav.OnFadeInEnded += OnFadeInEnded;
        CanvasNav.OnFadeOutStarted += OnFadeOutStarted;
        CanvasNav.OnFadeOutEnded += OnFadeOutEnded;
    }
    
    protected virtual void OnFadeInStarted(RBCanvasNavigation navPage) { }
    protected virtual void OnFadeInEnded(RBCanvasNavigation navPage) { }
    protected virtual void OnFadeOutStarted(RBCanvasNavigation navPage) { }
    protected virtual void OnFadeOutEnded(RBCanvasNavigation navPage) { }
}
