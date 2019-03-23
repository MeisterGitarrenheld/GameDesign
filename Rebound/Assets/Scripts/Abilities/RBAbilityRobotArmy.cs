using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBAbilityRobotArmy : ARBAbility
{
    protected override void OnTrigger()
    {
        print("activated " + this.ToString());
    }
}
