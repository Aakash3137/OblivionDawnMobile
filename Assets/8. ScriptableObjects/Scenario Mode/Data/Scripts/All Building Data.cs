using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Building Data", menuName = "Data/All Building Data")]
public class AllBuildingData : ScriptableObject
{
    public List<ResourceBuilding> resourceBuildings;
    public List<DefenseBuilding> defenseBuildings;
    public List<WallParentBuilding> wallBuildings;

    void OnValidate()
    {
        var enumValues = Enum.GetValues(typeof(FactionName));

        if (resourceBuildings == null)
            resourceBuildings = new List<ResourceBuilding>();

        while (resourceBuildings.Count < enumValues.Length)
            resourceBuildings.Add(new ResourceBuilding());

        while (resourceBuildings.Count > enumValues.Length)
            resourceBuildings.RemoveAt(resourceBuildings.Count - 1);

        for (int i = 0; i < enumValues.Length; i++)
            resourceBuildings[i].faction = (FactionName)enumValues.GetValue(i);

        if (defenseBuildings == null)
            defenseBuildings = new List<DefenseBuilding>();

        while (defenseBuildings.Count < enumValues.Length)
            defenseBuildings.Add(new DefenseBuilding());

        while (defenseBuildings.Count > enumValues.Length)
            defenseBuildings.RemoveAt(defenseBuildings.Count - 1);

        for (int i = 0; i < enumValues.Length; i++)
            defenseBuildings[i].faction = (FactionName)enumValues.GetValue(i);

        if (wallBuildings == null)
            wallBuildings = new List<WallParentBuilding>();

        while (wallBuildings.Count < enumValues.Length)
            wallBuildings.Add(new WallParentBuilding());

        while (wallBuildings.Count > enumValues.Length)
            wallBuildings.RemoveAt(wallBuildings.Count - 1);

        for (int i = 0; i < enumValues.Length; i++)
            wallBuildings[i].faction = (FactionName)enumValues.GetValue(i);
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
[Serializable]
public class WallParentBuilding
{
    public FactionName faction;
    public WallParent wallParentBuilding;
}