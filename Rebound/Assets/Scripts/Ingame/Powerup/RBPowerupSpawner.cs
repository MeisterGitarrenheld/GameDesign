using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RBPowerupSpawner : NetworkBehaviour
{
    /// <summary>
    /// Dictionary for providing editor access to rarity properties.
    /// </summary>
    [Serializable]
    public class RBPowerupRarityProbabilityDict : SerializableDictionaryBase<RBPowerupRarity, RBPowerupRarityStats> { }

    /// <summary>
    /// Class for storing rarity meta data.
    /// </summary>
    [Serializable]
    public class RBPowerupRarityStats
    {
        public int SpawnProbability = 0;
        public List<RectTransform> SpawnAreas = new List<RectTransform>();
    }

    /// <summary>
    /// The time between two powerup spawns.
    /// </summary>
    [SerializeField]
    private float _spawnIntervallSeconds = 2;

    /// <summary>
    /// The maximum number of powerups that can exist simultaneously.
    /// </summary>
    [SerializeField]
    private int _maxPowerupCount = 5;

    /// <summary>
    /// Stores information about rarity spawn positions etc.
    /// </summary>
    [SerializeField]
    private RBPowerupRarityProbabilityDict _rarityStats = new RBPowerupRarityProbabilityDict
    {
        { RBPowerupRarity.Normal, new RBPowerupRarityStats{ SpawnProbability = 80 } },
        { RBPowerupRarity.Seldom, new RBPowerupRarityStats{ SpawnProbability = 20 } }
    };

    private float _spawnDelta = 0;
    private System.Random _random = new System.Random();

    /// <summary>
    /// Contains all powerup game objects that are currently available.
    /// </summary>
    private List<GameObject> _spawnedPowerups = new List<GameObject>();

    /// <summary>
    /// Spawns and removes powerups on the host and sends the updates to the clients.
    /// </summary>
    void Update()
    {
        if (!isServer) return;

        if (!ARBArenaSetup.Instance.GamePaused)
        {
            TrySpawnPowerup();
            RemoveOutdatedPowerups();
        }
    }

    /// <summary>
    /// Spawns a random powerup if enough time has passed since the last spawn.
    /// </summary>
    private void TrySpawnPowerup()
    {
        _spawnDelta += Time.deltaTime;

        // only spawn when enough time has passed
        if (_spawnDelta < _spawnIntervallSeconds) return;

        _spawnDelta = 0.0f;
        SpawnPowerup();
    }

    /// <summary>
    /// Selects a random powerup and spawn position and spawns it.
    /// </summary>
    private void SpawnPowerup()
    {
        // select a random power up
        var powerupPrefab = GetRandomPowerupPrefab();
        var powerupBaseStats = powerupPrefab.GetComponent<RBPowerupBaseStatHolder>();

        // select the spawn position
        var spawnPosition = GetRandomPowerupSpawnPosition(powerupBaseStats.BaseStats.Rarity);

        // spawn the powerup on all clients and the host
        var powerup = Instantiate(powerupPrefab, spawnPosition, powerupPrefab.transform.rotation);
        NetworkServer.Spawn(powerup);

        // register the powerup as active
        _spawnedPowerups.Add(powerup);
    }

    /// <summary>
    /// Chooses the rarity based on the settings in <see cref="_rarityStats"/>.
    /// Then chooses a random powerup with the selected rarity.
    /// </summary>
    /// <returns></returns>
    private GameObject GetRandomPowerupPrefab()
    {
        // at first select the rarity based on the percentage
        var rarityTypeIndex = _random.Next(1, 101);

        RBPowerupRarity rarity = RBPowerupRarity.Normal;
        var rarityProbabilitySum = 0;
        foreach (var rarityKvp in _rarityStats)
        {
            rarityProbabilitySum += rarityKvp.Value.SpawnProbability;

            if (rarityTypeIndex <= rarityProbabilitySum)
            {
                rarity = rarityKvp.Key;
                break;
            }
        }

        // get a list of the powerups of the chosen rarity
        var powerups = RBPowerupGlobalObjects.Instance.PowerupPrefabs.Where(x => x.BaseStats.Rarity == rarity).ToArray();

        // get a random powerup from the list where each has the same probability
        return powerups[_random.Next(0, powerups.Length)].gameObject;
    }

    /// <summary>
    /// Selects a random spawn area and then a random position inside the spawn area.
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    private Vector3 GetRandomPowerupSpawnPosition(RBPowerupRarity rarity)
    {
        // Choose a random spawn area.
        var rarityStats = _rarityStats[rarity];
        var spawnAreas = rarityStats.SpawnAreas;
        var spawnArea = spawnAreas[_random.Next(0, spawnAreas.Count)];

        // Choose a random position within the chosen spawn area.
        var spawnAreaPosition = spawnArea.transform.position;
        var spawnAreaRect = spawnArea.rect;

        var x = spawnAreaPosition.x + _random.Next(0, (int)spawnAreaRect.width + 1);
        var z = spawnAreaPosition.z + _random.Next(0, (int)spawnAreaRect.height + 1);
        var y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Ensures that there are not too many powerups on the field.
    /// </summary>
    private void RemoveOutdatedPowerups()
    {
        // If a powerup has been used the game object has been destroyed and the slot is null.
        // So we just have to clean that up.
        _spawnedPowerups.RemoveAll(x => x == null);

        // If there are still too many non empty slots, we have to remove the oldest ones without leaving null slots.
        while (_spawnedPowerups.Count > _maxPowerupCount)
        {
            var powerupToRemove = _spawnedPowerups[0];
            _spawnedPowerups.RemoveAt(0);
            NetworkServer.Destroy(powerupToRemove);
        }
    }
}
