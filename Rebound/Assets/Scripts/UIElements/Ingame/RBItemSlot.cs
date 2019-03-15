using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBItemSlot : MonoBehaviour {

    public Text HotkeyText;

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
                GetComponent<Image>().color = Color.red;
                break;
            case RBPowerupActivityControl.SlotState.Empty:
                GetComponent<Image>().color = Color.green;
                break;
            case RBPowerupActivityControl.SlotState.ReadyForActivation:
                GetComponent<Image>().color = Color.yellow;
                break;
        }
	}

    public void SetHotkey(KeyCode keycode)
    {
        _hotkey = keycode;
        HotkeyText.text = _hotkey.ToString();
    }
}
