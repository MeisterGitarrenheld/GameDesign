using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBItemSlot : MonoBehaviour {

    public Text HotkeyText;
    public Image ActiveImage;

    private KeyCode _hotkey;

    void Awake()
    {
        RBPowerupActivityControl.UpdateUISlotsEvent += UpdateSlot; 
    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	public void UpdateSlot (KeyCode kc, RBPowerupActivityControl.PowerupSlot slot)
    {
        if (kc != _hotkey) return;

        switch(slot.State)
        {
            case RBPowerupActivityControl.SlotState.Active:
                ActiveImage.enabled = true;
                break;
            case RBPowerupActivityControl.SlotState.Empty:
                GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
                ActiveImage.enabled = false;
                // remove sprite
                break;
            case RBPowerupActivityControl.SlotState.ReadyForActivation:
                GetComponent<Image>().color = new Color(1, 1, 1, 0.75f);
                // apply sprite
                break;
        }
	}

    public void SetHotkey(KeyCode keycode)
    {
        _hotkey = keycode;
        HotkeyText.text = _hotkey.ToString();
    }
}
