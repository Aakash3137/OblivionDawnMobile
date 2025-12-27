using UnityEngine;
using System.Collections.Generic;
using Fusion;

[System.Serializable]
public class BuildingEntry
{
    public string buildingName;
    public GameObject playerBuildingPrefab;
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
    
    private void Start()
    {
        if (buildingDict == null)
            InitializeDictionary();
        
        RegisterFactionBuildings();
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
    
    private void RegisterFactionBuildings()
    {
        if (GameData.SelectedFactionName == null)
        {
            Debug.LogWarning("[BuildingRegistry] No faction selected, skipping faction building registration");
            return;
        }
        
        if (GameData.SelectedMPFaction == null)
        {
            Debug.LogWarning("[BuildingRegistry] No MP faction selected, skipping faction building registration");
            return;
        }
        
        var faction = GameData.SelectedMPFaction;
        
        RegisterBuilding(faction.mainBuildingPrefab);
        RegisterBuilding(faction.defenceBuildingPrefab);
        RegisterBuilding(faction.unitBuildingPrefab);
        RegisterBuilding(faction.resourceBuildingPrefab);
        
        Debug.Log($"[BuildingRegistry] Registered buildings for faction: {faction.factionName}");
    }
    
    private void RegisterBuilding(GameObject prefab)
    {
        if (prefab == null) return;
        
        string buildingName = prefab.name.ToLower();
        if (!buildingDict.ContainsKey(buildingName))
        {
            var entry = new BuildingEntry
            {
                buildingName = prefab.name,
                playerBuildingPrefab = prefab
            };
            buildingDict[buildingName] = entry;
            buildings.Add(entry);
            Debug.Log($"[BuildingRegistry] Dynamically registered: {prefab.name}");
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
