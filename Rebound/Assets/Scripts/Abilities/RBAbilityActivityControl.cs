using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBAbilityActivityControl : MonoBehaviour
{
    public static RBAbilityActivityControl Instance;

    public static Action<KeyCode, RBAbilityHandler> UpdateUISlotsEvent;

    [Serializable]
    public class RBAbilityKeyBinding : SerializableDictionaryBase<KeyCode, RBAbilityHandler> { }

    [Serializable]
    public class RBAbilityHandler
    {
        public ARBAbility Prefab;
        public ARBAbility Object;

        public float GetCurrentCooldown()
        {
            if (Object == null) return 0;

            return Object.CurrCooldownTime;
        }

        public float GetMaxCooldown()
        {
            return Prefab.CooldownTime;
        }
    }

    [SerializeField]
    private RBAbilityKeyBinding _abilityKeyBindings = new RBAbilityKeyBinding
    {
        { KeyCode.F, null },
        { KeyCode.Q, null }
    };

    void OnEnable()
    {
        Instance = this;
        RBIngameCanvas.Instance.CreateSkillSlots();
    }
	
    void Update()
    {
        foreach(var binding in _abilityKeyBindings)
        { 
            if (Input.GetKeyDown(binding.Key))
            {
                if (binding.Value.Object == null)
                    binding.Value.Object = Instantiate(binding.Value.Prefab, gameObject.transform);

                binding.Value.Object.Trigger();
            }

            UpdateUISlotsEvent?.Invoke(binding.Key, binding.Value);
        }
    }

    public RBAbilityKeyBinding GetAbilityBinding()
    {
        return _abilityKeyBindings;
    }
}
