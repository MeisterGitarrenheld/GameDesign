using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RBRawImageGradient : MonoBehaviour {

    [SerializeField]
    public RBCustomGradient customGradient;

    void Update()
    {
        gameObject.GetComponent<RawImage>().texture = customGradient.GetTexture(400);
    }
}
