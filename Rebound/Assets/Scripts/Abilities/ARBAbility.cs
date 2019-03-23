using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ARBAbility : MonoBehaviour
{
    public float CooldownTime;

    public float CurrCooldownTime { get; private set; }

    public bool IsOnCooldown
    {
        get { return CurrCooldownTime != 0; }   
    }

    protected void Update()
    {
        if(CurrCooldownTime > 0)
            CurrCooldownTime = Mathf.Max(0, CurrCooldownTime -= Time.deltaTime);

        UpdateAbility();
    }

    protected virtual void UpdateAbility() { }

    public bool Trigger()
    {
        if (IsOnCooldown) return false;

        CurrCooldownTime = CooldownTime;
        OnTrigger();

        return true;
    }

    protected abstract void OnTrigger();
}
