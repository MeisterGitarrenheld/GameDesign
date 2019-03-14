using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShieldEffect : MonoBehaviour
{
    private float _effectTime;

    void Update()
    {
        if (_effectTime > 0)
            if (_effectTime < 450 && _effectTime > 400)
                GetComponent<Renderer>().sharedMaterial.SetVector("_ShieldColor", new Vector4(0.7f, 1, 1, 0));

        _effectTime -= Time.deltaTime * 1000;
        GetComponent<Renderer>().sharedMaterial.SetFloat("_EffectTime", _effectTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach(var contact in collision.contacts)
        {
            GetComponent<Renderer>().sharedMaterial.SetVector("_ShieldColor", new Vector4(0.7f, 1, 1, 0.05f));
            GetComponent<Renderer>().sharedMaterial.SetVector("_Position", transform.InverseTransformPoint(contact.point));

            _effectTime = 500;
        }
    }
}
