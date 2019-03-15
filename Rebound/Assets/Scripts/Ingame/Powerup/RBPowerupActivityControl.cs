using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This is a local singleton for every client.
/// </summary>
public class RBPowerupActivityControl : NetworkBehaviour
{
    public static Action<KeyCode, PowerupSlot> UpdateUISlotsEvent;

    public enum SlotState { Empty, ReadyForActivation, Active }

    [Serializable]
    public struct PowerupSlot
    {
        public SlotState State;
        public RBPowerupBaseStats BaseStats;

        public PowerupSlot(SlotState state, RBPowerupBaseStats baseStats)
        {
            State = state;
            BaseStats = baseStats;
        }
    }

    public static RBPowerupActivityControl Instance;


    [Serializable]
    public class RBPowerupKeyBinding : SerializableDictionaryBase<KeyCode, PowerupSlot> { }

    [SerializeField]
    private RBPowerupKeyBinding _powerupKeyBindings = new RBPowerupKeyBinding
    {
        { KeyCode.LeftControl, new PowerupSlot(SlotState.Empty, null) }
        //{ KeyCode.Q, null }
    };

    private PowerupSlot _defaultBinding = new PowerupSlot(SlotState.Empty, null);

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Listens for powerup activation.
    /// </summary>
    void Update()
    {
        CheckForActivatedPowerups();
    }

    public RBPowerupKeyBinding GetPowerupKeyBindings()
    {
        return _powerupKeyBindings;
    }

    /// <summary>
    /// Checks if a key for a powerup was pressed and activates the
    /// corresponding powerup if so.
    /// </summary>
    private void CheckForActivatedPowerups()
    {
        foreach (var powerupKey in _powerupKeyBindings.Keys.ToArray())
        {
            var powerupKeyBinding = _powerupKeyBindings[powerupKey];

            if (powerupKeyBinding.State == SlotState.ReadyForActivation)
            {
                if (Input.GetKeyUp(powerupKey))
                { // Found a powerup that has to be activated.
                    ActivatePowerup(powerupKey, powerupKeyBinding);
                }
            }
        }
    }

    /// <summary>
    /// Activates the powerup that corresponds to the given base stats.
    /// </summary>
    /// <param name="baseStats"></param>
    private void ActivatePowerup(KeyCode activationKey, PowerupSlot slot)
    {
        // Debug.Log("Activating a powerup with rarity " + baseStats.Rarity + ".");
        if (slot.BaseStats.HandlerPrefab != null)
        {
            _powerupKeyBindings[activationKey] = new PowerupSlot(SlotState.Active, slot.BaseStats);

            var handlerObject = Instantiate(RBPowerupGlobalObjects.Instance.GetPowerupHandlerPrefabById(slot.BaseStats.PowerupId));
            var handlerScript = handlerObject.GetComponent<ARBPowerupActionHandler>();
            handlerScript.OnComplete += () => HandlerScript_OnComplete(activationKey, handlerObject);
            handlerScript.DoAction(RBMatch.Instance.GetLocalUser().Username);
        }
        else _powerupKeyBindings[activationKey] = _defaultBinding;

        UpdateUISlots();
    }

    /// <summary>
    /// Called when a powerup action has completed.
    /// Destroys the handler object and frees the powerup slot.
    /// </summary>
    /// <param name="activationKey"></param>
    /// <param name="handlerObject"></param>
    private void HandlerScript_OnComplete(KeyCode activationKey, GameObject handlerObject)
    {
        Debug.Log("HandlerScript_OnComplete");
        Destroy(handlerObject);
        _powerupKeyBindings[activationKey] = _defaultBinding;

        UpdateUISlots();
    }

    /// <summary>
    /// Tries to find a free powerup slot and adds it if found.
    /// If there is no free slot, nothing happens.
    /// </summary>
    /// <param name="baseStats"></param>
    public void CollectPowerup(RBPowerupBaseStats baseStats)
    {
        foreach (var binding in _powerupKeyBindings)
        {
            Debug.Log(binding.Key + "; " + binding.Value.State);
            if (binding.Value.State == SlotState.Empty)
            {
                Debug.Log("Collected a powerup with rarity " + baseStats.Rarity + ".");
                _powerupKeyBindings[binding.Key] = new PowerupSlot(SlotState.ReadyForActivation, baseStats);
                break;
            }
        }

        UpdateUISlots();
    }

    private void UpdateUISlots()
    {
        foreach( var binding in _powerupKeyBindings)
        {
            UpdateUISlotsEvent?.Invoke(binding.Key, binding.Value);
        }
    }
}
