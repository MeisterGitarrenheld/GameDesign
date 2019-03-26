using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RBAudioAutoDestroy : MonoBehaviour {

    private AudioSource _audioSource;

	void Awake()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        if(!_audioSource.isPlaying)
            _audioSource.Play();
    }   
	
	// Update is called once per frame
	void Update ()
    {
        if (!_audioSource.isPlaying)
            Destroy(gameObject);
	}
}
