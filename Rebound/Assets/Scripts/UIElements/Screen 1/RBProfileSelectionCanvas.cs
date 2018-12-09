using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBProfileSelectionCanvas : RBCanvas
{
    public RBCanvasNavigation NewProfileScreen = null;
    public RBCanvasNavigation ExistingProfileScreen = null;
    public RBCanvasNavigation GuestProfileScreen = null;
    public RBCanvasNavigation MatchFinderScreen = null;

    public List<Toggle> NavToggles;

    private RBCanvasNavigation _activeProfileScreen = null;


    protected override void Awake()
    {
        base.Awake();

        NewProfileScreen.OnFadeInStarted += ProfileScreen_OnFadeInStarted;
        ExistingProfileScreen.OnFadeInStarted += ProfileScreen_OnFadeInStarted;
        GuestProfileScreen.OnFadeInStarted += ProfileScreen_OnFadeInStarted;
        GuestProfileScreen.Show();
    }

    /// <summary>
    /// Sets the active profile selection page when it is shown.
    /// </summary>
    /// <param name="navPage"></param>
    private void ProfileScreen_OnFadeInStarted(RBCanvasNavigation navPage)
    {
        _activeProfileScreen = navPage;
    }

    public void Submit()
    {
        var profileValidator = _activeProfileScreen.GetComponent<ARBUserProfileValidator>();

        if (profileValidator == null)
            throw new NotImplementedException("The requested profile selector is not implemented.");

        if (profileValidator.Validate())
        {
            RBLocalUser.Instance.ProfileType = profileValidator.ProfileType;
            RBLocalUser.Instance.Username = profileValidator.Username;
            MatchFinderScreen.Show();
        }
    }
}
