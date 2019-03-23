using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBAbilityWall : ARBAbility {
    protected override void OnTrigger()
    {
        print("activated " + this.ToString());
    }
}
