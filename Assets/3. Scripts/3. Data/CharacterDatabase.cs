using System.Collections.Generic;
using UnityEngine;

public class CharacterDatabase : MonoBehaviour
{
    public static CharacterDatabase Instance { get; private set; }

    public List<UnitStats> unitPrefabs;
    public List<MainBuildingStats> mainBuildingPrefabs;
    public List<DefenseBuildingStats> defenseBuildingPrefabs;
    public List<ResourceBuildingStats> resourceBuildingPrefabs;
    private Dictionary<UnitProduceStatsSO, UnitStats> lookup;
    private Dictionary<DefenseBuildingDataSO, DefenseBuildingStats> defenseLookup;
    private Dictionary<ResourceBuildingDataSO, ResourceBuildingStats> resourceLookup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void RegisterUnits()
    {
        lookup = new Dictionary<UnitProduceStatsSO, UnitStats>();

        foreach (var characterPrefab in unitPrefabs)
        {
            if (characterPrefab.unitProduceSO == null)
            {
                Debug.LogError(characterPrefab.name + " has no UnitProduceStatsSO assigned!");
                continue;
            }

            if (lookup.ContainsKey(characterPrefab.unitProduceSO))
            {
                Debug.LogError("Duplicate ScriptableObject found: " + characterPrefab.unitProduceSO.name);
                continue;
            }

            lookup.Add(characterPrefab.unitProduceSO, characterPrefab);
        }
    }

    // private void RegisterCityCenterBuildings()
    // {
    //     foreach (var cityCenterPrefab in cityCenterPrefabs)
    //     {
    //         if (cityCenterPrefab.GetBuildingSO() == null)
    //         {
    //             Debug.LogError(cityCenterPrefab.name + " has no MainBuildingDataSO assigned!");
    //             continue;
    //         }
    //     }
    // }

    private void RegisterDefenseBuildings()
    {
        defenseLookup = new Dictionary<DefenseBuildingDataSO, DefenseBuildingStats>();

        foreach (var defensePrefab in defenseBuildingPrefabs)
        {
            if (defensePrefab.GetBuildingSO() == null)
            {
                Debug.LogError(defensePrefab.name + " has no DefenseBuildingDataSO assigned!");
                continue;
            }

            if (defenseLookup.ContainsKey(defensePrefab.GetBuildingSO()))
            {
                Debug.LogError("Duplicate ScriptableObject found: " + defensePrefab.GetBuildingSO().name);
                continue;
            }

            defenseLookup.Add(defensePrefab.GetBuildingSO(), defensePrefab);
        }
    }

    private void RegisterResourceBuildings()
    {
        resourceLookup = new Dictionary<ResourceBuildingDataSO, ResourceBuildingStats>();

        foreach (var resourcePrefab in resourceBuildingPrefabs)
        {
            if (resourcePrefab.GetBuildingSO() == null)
            {
                Debug.LogError(resourcePrefab.name + " has no ResourceBuildingDataSO assigned!");
                continue;
            }

            if (resourceLookup.ContainsKey(resourcePrefab.GetBuildingSO()))
            {
                Debug.LogError("Duplicate ScriptableObject found: " + resourcePrefab.GetBuildingSO().name);
                continue;
            }

            resourceLookup.Add(resourcePrefab.GetBuildingSO(), resourcePrefab);
        }
    }

    public UnitStats GetUnitPrefab(UnitProduceStatsSO unitProduceStats)
    {
        if (lookup.TryGetValue(unitProduceStats, out UnitStats prefab))
            return prefab;

        Debug.LogError("No prefab found for: " + unitProduceStats.name);
        return null;
    }

    public OffenseBuildingStats GetSpawnerBuilding(UnitProduceStatsSO unitProduceStats)
    {
        return unitProduceStats.spawnerBuilding;
    }

    public DefenseBuildingStats GetDefenseBuildingPrefab(DefenseBuildingDataSO defenseBuildingData)
    {
        if (defenseLookup.TryGetValue(defenseBuildingData, out DefenseBuildingStats prefab))
            return prefab;

        Debug.LogError("No prefab found for: " + defenseBuildingData.name);
        return null;
    }

    public ResourceBuildingStats GetResourceBuildingPrefab(ResourceBuildingDataSO resourceBuildingData)
    {
        if (resourceLookup.TryGetValue(resourceBuildingData, out ResourceBuildingStats prefab))
            return prefab;

        Debug.LogError("No prefab found for: " + resourceBuildingData.name);
        return null;
    }

    private void OnValidate()
    {
        RegisterUnits();
        RegisterDefenseBuildings();
        RegisterResourceBuildings();
    }
}
