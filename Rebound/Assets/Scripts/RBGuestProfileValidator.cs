using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBGuestProfileValidator : ARBUserProfileValidator {
    public override RBUserProfileType ProfileType { get; } = RBUserProfileType.Temporary;

    public RBInputField UsernameInput = null;

    public override string Username { get { return UsernameInput?.UIComponent.text; }  }
}
