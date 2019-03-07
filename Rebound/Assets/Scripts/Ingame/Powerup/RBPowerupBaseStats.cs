using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RBPowerupRarity { None, Normal, Seldom }

[Serializable]
public class RBPowerupBaseStats
{
    public int PowerupId = 0;

    public RBPowerupRarity Rarity = RBPowerupRarity.None;

    public GameObject HandlerPrefab = null;
}
