using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBApplicationStartup : MonoBehaviour
{
    public RBCanvasNavigation NewProfileScreen = null;
    public RBCanvasNavigation ExistingProfileScreen = null;
    public RBCanvasNavigation GuestProfileScreen = null;

    void Awake()
    {
        ExistingProfileScreen.Show();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
