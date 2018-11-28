using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RBUserProfileType { None, Permanent, Temporary }

public abstract class ARBUserProfileValidator : MonoBehaviour, IRBValidator
{
    public abstract RBUserProfileType ProfileType { get; }
    public abstract string Username { get; }

    public virtual bool Validate()
    {
        return ProfileType != RBUserProfileType.None && !string.IsNullOrWhiteSpace(Username);
    }
}
