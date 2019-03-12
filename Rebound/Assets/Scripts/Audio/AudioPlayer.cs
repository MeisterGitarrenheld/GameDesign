using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SfxType
{
    Goal,
    BallHit,
    Explosion,
    PlayerEnter,
    PlayerLeave
}
[ExecuteInEditMode]
public class AudioPlayer : MonoBehaviour {

    public static AudioPlayer Instance;

    private AudioSource[] AvailableSources;

    public AudioClip[] Sfx;

    [Range(0, 1)]
    public float Volume;

    private void Update()
    {
        System.Array.ForEach(AvailableSources, source => source.volume = Volume);
    }

    void Start ()
    {
		if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        AudioSource[] temp = GetComponents<AudioSource>(); //Remove Music Player
        AvailableSources = new AudioSource[temp.Length - 1];
        for(int i = 1;  i < temp.Length; i++)
        {
            AvailableSources[i - 1] = temp[i];
        }
	}

    public void PlaySound(SfxType toPlay)
    {
        AudioSource freeAudioSource = AvailableSources[Random.Range(0, AvailableSources.Length)];
        foreach (AudioSource aus in AvailableSources)
        {
            if (!aus.isPlaying)
            {
                freeAudioSource = aus;
                break;
            }
        }
        if (freeAudioSource.isPlaying)
        {
            freeAudioSource.Stop();

        }
        freeAudioSource.clip = Sfx[(int)toPlay];
        freeAudioSource.Play();
    }
    
}
