using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Building Data", menuName = "Data/All Building Data")]
public class AllBuildingData : ScriptableObject
{
    public List<ResourceBuilding> resourceBuildings;
    public List<DefenseBuilding> defenseBuildings;

    void OnValidate()
    {
        var enumValues = Enum.GetValues(typeof(FactionName));

        while (resourceBuildings.Count < enumValues.Length)
            resourceBuildings.Add(new ResourceBuilding());

        while (resourceBuildings.Count > enumValues.Length)
            resourceBuildings.RemoveAt(resourceBuildings.Count - 1);

        for (int i = 0; i < enumValues.Length; i++)
            resourceBuildings[i].faction = (FactionName)enumValues.GetValue(i);

        while (defenseBuildings.Count < enumValues.Length)
            defenseBuildings.Add(new DefenseBuilding());

        while (defenseBuildings.Count > enumValues.Length)
            defenseBuildings.RemoveAt(defenseBuildings.Count - 1);

        for (int i = 0; i < enumValues.Length; i++)
            defenseBuildings[i].faction = (FactionName)enumValues.GetValue(i);
    }
}

[Serializable]
public class ResourceBuilding
{
    public FactionName faction;
    public List<ResourceBuildingDataSO> foodResourceBuildings;
    public List<ResourceBuildingDataSO> goldResourceBuildings;
    public List<ResourceBuildingDataSO> metalResourceBuildings;
    public List<ResourceBuildingDataSO> powerResourceBuildings;
}

[Serializable]
public class DefenseBuilding
{
    public FactionName faction;
    public List<DefenseBuildingDataSO> antiAirBuildings;
    public List<DefenseBuildingDataSO> antiTankBuildings;
    public List<DefenseBuildingDataSO> turretBuildings;
    public List<DefenseBuildingDataSO> wallBuildings;
}