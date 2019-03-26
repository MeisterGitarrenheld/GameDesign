using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBItemSlot : MonoBehaviour {

    public Text HotkeyText;
    public Image ActiveImage;
    public Image Icon;

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
                Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, 0);
                ActiveImage.enabled = false;
                break;
            case RBPowerupActivityControl.SlotState.ReadyForActivation:
                GetComponent<Image>().color = new Color(1, 1, 1, 0.75f);
                Icon.sprite = slot.BaseStats.UIIcon;
                Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, 1);
                break;
        }
	}

    public void SetHotkey(KeyCode keycode)
    {
        _hotkey = keycode;
        HotkeyText.text = _hotkey.ToString();
    }
}
