using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class ensures that a MonoBehaviour is maximum once in the scene.
/// It does not guarantee that there is an instance at all.
/// 
/// Usage:
/// - T should be the type that inherits from RBMonoBehaviourSingleton.
///   For example "public class MyClass : RBMonoBehaviourSingleton<MyClass>".
/// - Use AwakeSingleton and OnDestroyUnityObject instead of Awake and OnDestroy
/// </summary>
/// <typeparam name="T"></typeparam>
public class RBMonoBehaviourSingleton<T> : MonoBehaviour where T : RBMonoBehaviourSingleton<T>
{
    public static bool InstanceSet { get; private set; } = false;
    private static RBMonoBehaviourSingleton<T> _instance;
    public static T Instance { get { return _instance as T; } private set { _instance = value; } }

    /// <summary>
    /// Called on object creation.
    /// </summary>
    protected void Awake()
    {
        if (InstanceSet)
            Destroy(gameObject);
        else
        {
            Instance = this as T;
            InstanceSet = true;
            AwakeSingleton();
        }
    }

    /// <summary>
    /// Called after the Awake-function, if the object has not been destroyed.
    /// </summary>
    protected virtual void AwakeSingleton() { }

    /// <summary>
    /// Called by Unity when the object is being destroyed.
    /// </summary>
    protected void OnDestroy()
    {
        if (InstanceSet && GetInstanceID() == Instance.GetInstanceID())
        {
            Instance = null;
            InstanceSet = false;
        }
        OnDestroyUnityObject();
    }

    /// <summary>
    /// Called after the Unity-OnDestroy-function in any case.
    /// Even if the object was not the singleton, but another object of the
    /// same type that has been destroyed by the awake function.
    /// </summary>
    protected virtual void OnDestroyUnityObject() { }
}