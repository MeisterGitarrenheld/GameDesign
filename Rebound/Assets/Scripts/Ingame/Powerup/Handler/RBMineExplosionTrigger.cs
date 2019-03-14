using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBMineExplosionTrigger : MonoBehaviour
{
    public GameObject ParticleSystemPrefab;

    void OnDisable()
    {
        var particleSystem = Instantiate(ParticleSystemPrefab);
        particleSystem.transform.position = gameObject.transform.position;
        particleSystem.transform.localScale = new Vector3(3, 3, 3);
    }
}
