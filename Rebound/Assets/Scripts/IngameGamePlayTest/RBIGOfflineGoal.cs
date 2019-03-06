using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBIGOfflineGoal : MonoBehaviour {

    public int PlayerNumber;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ball")
            RBIGPlayTest.Instance.Goal(PlayerNumber);
    }
}
