﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBRotateGO : MonoBehaviour
{

    public float RotationSpeed = 50.0f;
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
	}
}
