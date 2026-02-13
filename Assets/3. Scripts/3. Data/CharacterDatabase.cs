using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterDatabase : MonoBehaviour
{
    public List<GameObject> UnitPrefabs;

    public List<GameObject> BuildingPrefabs;
    
    private Dictionary<UnitProduceStatsSO, GameObject> lookup;
    private Dictionary<BuildingDataSO, GameObject> lookupBuildings;

    void Awake()
    {
        lookup = new Dictionary<UnitProduceStatsSO, GameObject>();
        
        lookupBuildings = new Dictionary<BuildingDataSO, GameObject>();

        foreach (GameObject characterPrefab in UnitPrefabs)
        {
            UnitStats unitStats = characterPrefab.GetComponent<UnitStats>();
            
            if (unitStats == null)
            {
                Debug.LogError(characterPrefab.name + " has no UnitStats component!");
                continue;
            }

            if (unitStats.unitProduceSO == null)
            {
                Debug.LogError(characterPrefab.name + " has no UnitProduceStatsSO assigned!");
                continue;
            }

            if (lookup.ContainsKey(unitStats.unitProduceSO))
            {
                Debug.LogError("Duplicate ScriptableObject found: " + unitStats.unitProduceSO.name);
                continue;
            }
            
            lookup.Add(unitStats.unitProduceSO, characterPrefab);
        }
        
        foreach (GameObject buildingPrefab in BuildingPrefabs)
        {
            BuildingStats buildingStats = buildingPrefab.GetComponent<BuildingStats>();

            if (buildingStats == null)
            {
                Debug.LogError(buildingPrefab.name + " has no BuildingStats component!");
                continue;
            }

            if (buildingStats.buildingStats == null)
            {
                Debug.LogError(buildingPrefab.name + " has no BuildingDataSO assigned!");
                continue;
            }

            if (lookupBuildings.ContainsKey(buildingStats.buildingStats))
            {
                Debug.LogError("Duplicate ScriptableObject found: " + buildingStats.buildingStats.name);
                continue;
            }

            lookupBuildings.Add(buildingStats.buildingStats, buildingPrefab);
        }
    }

    public GameObject GetPrefab(UnitProduceStatsSO unitProduceStats)
    {
        if (lookup.TryGetValue(unitProduceStats, out GameObject prefab))
            return prefab;

        Debug.LogError("No prefab found for: " + unitProduceStats.name);
        return null;
    }
    /*public GameObject GetOffenseBuilding(BuildingDataSO buildingStats)
    {
        if (buildingStats is OffenseBuildingDataSO offenseBuildingStats)
        {
            if (lookupBuildings.TryGetValue(offenseBuildingStats, out GameObject prefab))
                return prefab;
        }
        Debug.LogError("Scriptable Object is not offense Building: " + buildingStats.name);
        return null;
    }*/
}
