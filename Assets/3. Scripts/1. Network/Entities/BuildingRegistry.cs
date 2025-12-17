using UnityEngine;
using System.Collections.Generic;
using Fusion;

[System.Serializable]
public class BuildingEntry
{
    public string buildingName;
    public GameObject playerBuildingPrefab;
    public GameObject enemyBuildingPrefab;
}

public class BuildingRegistry : MonoBehaviour
{
    public static BuildingRegistry Instance;

    [Header("Building Prefabs")]
    public List<BuildingEntry> buildings = new List<BuildingEntry>();

    private Dictionary<string, BuildingEntry> buildingDict;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        buildingDict = new Dictionary<string, BuildingEntry>();

        foreach (var entry in buildings)
        {
            if (!string.IsNullOrEmpty(entry.buildingName))
            {
                buildingDict[entry.buildingName.ToLower()] = entry;
                Debug.Log($"[BuildingRegistry] Registered: {entry.buildingName}");
            }
        }
    }

    public BuildingEntry GetBuildingEntry(string buildingName)
    {
        if (string.IsNullOrEmpty(buildingName))
        {
            Debug.LogWarning("[BuildingRegistry] Building name is null or empty");
            return null;
        }

        string key = buildingName.ToLower();
        if (buildingDict.TryGetValue(key, out BuildingEntry entry))
        {
            Debug.Log($"[BuildingRegistry] Found entry for: {buildingName}");
            return entry;
        }

        Debug.LogWarning($"[BuildingRegistry] No entry found for: {buildingName}");
        return null;
    }
    
}
