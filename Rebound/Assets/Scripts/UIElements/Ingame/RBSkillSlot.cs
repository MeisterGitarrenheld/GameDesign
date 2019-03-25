using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBSkillSlot : MonoBehaviour {

    public Text HotkeyText;
    public Image ImageCD;
    public Image IconImage;

    private KeyCode _hotkey;
    private float _prevCooldownCount = 0;

    void Awake()
    {
        RBAbilityActivityControl.UpdateUISlotsEvent += UpdateSlot;
    }

    public void UpdateSlot(KeyCode kc, RBAbilityActivityControl.RBAbilityHandler handler)
    {
        if (kc != _hotkey) return;

        if (IconImage.sprite == null)
            IconImage.sprite = handler.Prefab.AbilityIcon;

        if(ImageCD != null)
        {
            ImageCD.fillAmount = 1 - (handler.GetCurrentCooldown() / handler.GetMaxCooldown());

            if (handler.GetCurrentCooldown() == 0 && _prevCooldownCount > 0)
            {
                // toggle cd finished anim
                print("cd ready");
                GetComponent<Animator>().SetTrigger("CDReady");
            }

            _prevCooldownCount = handler.GetCurrentCooldown();
        }
    }

    public void SetHotkey(KeyCode keycode)
    {
        _hotkey = keycode;
        HotkeyText.text = _hotkey.ToString();
    }
}
