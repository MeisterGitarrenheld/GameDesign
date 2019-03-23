using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBAbilityWallRagdollActivator : MonoBehaviour
{
    [SerializeField]
    private GameObject _ragdollPrefab;

    [SerializeField]
    private float _ragdollLifeTime = 3.0f;

    void OnDisable()
    {
        var ragdoll = Instantiate(_ragdollPrefab, transform.position, transform.rotation);
        Destroy(ragdoll, _ragdollLifeTime);
    }
}
