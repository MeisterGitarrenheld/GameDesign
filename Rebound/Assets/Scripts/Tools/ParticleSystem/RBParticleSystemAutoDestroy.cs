using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBParticleSystemAutoDestroy : MonoBehaviour
{
    private ParticleSystem[] ps;


    public void Start()
    {
        ps = GetComponentsInChildren<ParticleSystem>();
    }

    public void Update()
    {

        if (ps != null)
        {
            foreach (var particleSystem in ps)
                if (particleSystem.IsAlive()) return;

            Destroy(gameObject);
        }
    }
}
