using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FactionPrefabStore", menuName = "RTS/Faction Prefab Store")]
public class FactionPrefabStore : ScriptableObject
{
    [Header("Main Building Prefabs")]
    public GameObject playerMainBuildingPrefab;
    public GameObject enemyMainBuildingPrefab;

    [Header("Runtime store (populated on faction select)")]
    [SerializeField] private List<GameObject> playerPrefabsRuntime = new List<GameObject>();
    [SerializeField] private List<GameObject> enemyPrefabsRuntime = new List<GameObject>();

    // Public read-only accessors
    public IReadOnlyList<GameObject> PlayerPrefabs => playerPrefabsRuntime;
    public IReadOnlyList<GameObject> EnemyPrefabs => enemyPrefabsRuntime;

    private Dictionary<string, GameObject> playerMap;
    private Dictionary<string, GameObject> enemyMap;

    /// <summary>
    /// Clears all stored prefabs and maps.
    /// </summary>
    public void Clear()
    {
        playerPrefabsRuntime.Clear();
        enemyPrefabsRuntime.Clear();
        playerMap = null;
        enemyMap = null;
    }

    /// <summary>
    /// Injects prefabs from a selected faction into the store.
    /// </summary>
    public void SetFromFaction(GameObject[] playerPrefabs, GameObject[] enemyPrefabs = null)
    {
        Clear();

        if (playerPrefabs != null)
            playerPrefabsRuntime.AddRange(playerPrefabs);

        if (enemyPrefabs != null)
            enemyPrefabsRuntime.AddRange(enemyPrefabs);

        BuildMaps();
    }

    private void BuildMaps()
    {
        playerMap = new Dictionary<string, GameObject>();
        foreach (var p in playerPrefabsRuntime)
        {
            if (p == null) continue;
            if (!playerMap.ContainsKey(p.name))
                playerMap.Add(p.name, p);
        }

        enemyMap = new Dictionary<string, GameObject>();
        foreach (var e in enemyPrefabsRuntime)
        {
            if (e == null) continue;
            if (!enemyMap.ContainsKey(e.name))
                enemyMap.Add(e.name, e);
        }
    }

    /// <summary>
    /// Get a player prefab by building name.
    /// </summary>
    public GameObject GetPlayerByName(string buildingName)
    {
        if (playerMap == null) BuildMaps();
        if (string.IsNullOrEmpty(buildingName)) return null;
        return playerMap.TryGetValue(buildingName, out var prefab) ? prefab : null;
    }

    /// <summary>
    /// Get an enemy prefab by building name.
    /// </summary>
    public GameObject GetEnemyByName(string buildingName)
    {
        if (enemyMap == null) BuildMaps();
        if (string.IsNullOrEmpty(buildingName)) return null;
        return enemyMap.TryGetValue(buildingName, out var prefab) ? prefab : null;
    }
}
