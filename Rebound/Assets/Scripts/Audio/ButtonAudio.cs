using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour {

    public AudioClip ClickSound;
    public AudioClip HoverSound;

    private Button _button { get { return GetComponent<Button>(); } }
    private AudioSource _source;

	// Use this for initialization
	void Start ()
    {
        _source = gameObject.AddComponent<AudioSource>();
        _source.clip = ClickSound;
        _source.playOnAwake = false;

        _button.onClick.AddListener(() => PlaySound(ClickSound));

        var trigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => PlaySound(HoverSound));
        trigger.triggers.Add(entry);

        // audio settings
        _source.pitch = 1.4f;
	}
	
	private void PlaySound(AudioClip clip)
    {
        if (_button.interactable == false) return;
        _source.PlayOneShot(clip);
    }
}
