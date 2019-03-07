using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBPowerupGlobalObjects : RBMonoBehaviourSingleton<RBPowerupGlobalObjects>
{
    /// <summary>
    /// A list of all available powerup prefabs.
    /// </summary>
    public List<RBPowerupBaseStatHolder> PowerupPrefabs;

    /// <summary>
    /// Searches the powerup with the given id and returns its handler prefab.
    /// </summary>
    /// <param name="powerupId"></param>
    /// <returns></returns>
    public GameObject GetPowerupHandlerPrefabById(int powerupId)
    {
        return PowerupPrefabs.Find(x => x.BaseStats.PowerupId == powerupId).BaseStats.HandlerPrefab;
    }
}
