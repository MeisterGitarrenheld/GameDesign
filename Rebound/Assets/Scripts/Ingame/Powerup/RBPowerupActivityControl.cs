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
    public static RBPowerupActivityControl Instance;


    [Serializable]
    public class RBPowerupKeyBinding : SerializableDictionaryBase<KeyCode, RBPowerupBaseStats> { }

    [SerializeField]
    private RBPowerupKeyBinding _powerupKeyBindings = new RBPowerupKeyBinding
    {
        { KeyCode.LeftControl, null },
        { KeyCode.Q, null }
    };

    private RBPowerupBaseStats _defaultBinding = new RBPowerupBaseStats();

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

    /// <summary>
    /// Checks if a key for a powerup was pressed and activates the
    /// corresponding powerup if so.
    /// </summary>
    private void CheckForActivatedPowerups()
    {
        var activatedPowerups = new List<KeyCode>();

        foreach (var powerupKeyBinding in _powerupKeyBindings)
        {
            if (powerupKeyBinding.Value.Rarity != RBPowerupRarity.None)
            {
                if (Input.GetKeyUp(powerupKeyBinding.Key))
                { // Found a powerup that has to be activated.
                    ActivatePowerup(powerupKeyBinding.Value);
                    activatedPowerups.Add(powerupKeyBinding.Key);
                }
            }
        }

        activatedPowerups.ForEach(x => _powerupKeyBindings[x] = _defaultBinding);
    }

    /// <summary>
    /// Activates the powerup that corresponds to the given base stats.
    /// </summary>
    /// <param name="baseStats"></param>
    private void ActivatePowerup(RBPowerupBaseStats baseStats)
    {
        Debug.Log("Activating a powerup with rarity " + baseStats.Rarity + ".");
        if (baseStats.HandlerPrefab != null)
        {
            var handlerObject = Instantiate(RBPowerupGlobalObjects.Instance.GetPowerupHandlerPrefabById(baseStats.PowerupId));
            var handlerScript = handlerObject.GetComponent<ARBPowerupActionHandler>();
            handlerScript.DoAction(RBMatch.Instance.GetLocalUser().Username);
        }
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
            if (binding.Value.Rarity == RBPowerupRarity.None)
            {
                Debug.Log("Collected a powerup with rarity " + baseStats.Rarity + ".");
                _powerupKeyBindings[binding.Key] = baseStats;
                break;
            }
        }
    }
}
