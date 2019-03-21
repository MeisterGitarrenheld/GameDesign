using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RBLoadingAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        var rect = GetComponent<RectTransform>();
        //rect.Rotate(rect.rotation.x, rect.rotation.y, rect.rotation.z + Time.deltaTime * 100);
	}
}
