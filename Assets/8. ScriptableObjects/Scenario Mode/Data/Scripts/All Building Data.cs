using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "All Building Data", menuName = "Data/All Building Data")]
public class AllBuildingData : ScriptableObject
{
    public List<MainBuildingDataSO> cityCenterBuildingsSO;
    [Space(10)]
    public List<BuildingDataSO> allBuildingsSO;
    [Space(10)]
    public List<WallParentBuilding> wallParentBuildings;

    public List<OffenseBuilding> offenseBuildings { get; internal set; }
    public List<ResourceBuilding> resourceBuildings { get; internal set; }
    public List<DefenseBuilding> defenseBuildings { get; internal set; }

    private void Awake()
    {
        Populate();
    }

    public void Populate()
    {
        // Clear Previous Data and repopulate
        var enumValues = ScenarioDataTypes._factionEnumValues;

        offenseBuildings = new List<OffenseBuilding>();
        resourceBuildings = new List<ResourceBuilding>();
        defenseBuildings = new List<DefenseBuilding>();

        foreach (FactionName factionName in enumValues)
        {
            offenseBuildings.Add(new OffenseBuilding { faction = factionName });
            resourceBuildings.Add(new ResourceBuilding { faction = factionName });
            defenseBuildings.Add(new DefenseBuilding { faction = factionName });
        }

        AddBuildingsDataSO();
    }

    private void AddBuildingsDataSO()
    {
        foreach (var buildingSO in allBuildingsSO)
        {
            switch (buildingSO.buildingType)
            {
                case ScenarioBuildingType.ResourceBuilding:
                    AddResourceBuildings((ResourceBuildingDataSO)buildingSO);
                    break;
                case ScenarioBuildingType.OffenseBuilding:
                    AddOffenseBuildings((OffenseBuildingDataSO)buildingSO);
                    break;
                case ScenarioBuildingType.DefenseBuilding:
                    AddDefenseBuildings((DefenseBuildingDataSO)buildingSO);
                    break;
            }
        }
    }

    private void AddResourceBuildings(ResourceBuildingDataSO dataSO)
    {
        if (dataSO == null) return;

        var faction = dataSO.buildingIdentity.faction;

        switch (dataSO.resourceType)
        {
            case ScenarioResourceType.Food:
                resourceBuildings[(int)faction].foodResourceBuildings.Add(dataSO);
                break;
            case ScenarioResourceType.Gold:
                resourceBuildings[(int)faction].goldResourceBuildings.Add(dataSO);
                break;
            case ScenarioResourceType.Metal:
                resourceBuildings[(int)faction].metalResourceBuildings.Add(dataSO);
                break;
            case ScenarioResourceType.Power:
                resourceBuildings[(int)faction].powerResourceBuildings.Add(dataSO);
                break;
        }
    }

    private void AddOffenseBuildings(OffenseBuildingDataSO dataSO)
    {
        if (dataSO == null) return;
        var faction = dataSO.buildingIdentity.faction;

        switch (dataSO.offenseType)
        {
            case ScenarioOffenseType.AirBuilding:
                offenseBuildings[(int)faction].airBuildings.Add(dataSO);
                break;
            case ScenarioOffenseType.AOERangedBuilding:
                offenseBuildings[(int)faction].aoeRangedBuildings.Add(dataSO);
                break;
            case ScenarioOffenseType.RangedBuilding:
                offenseBuildings[(int)faction].rangedBuildings.Add(dataSO);
                break;
            case ScenarioOffenseType.MeleeBuilding:
                offenseBuildings[(int)faction].meleeBuildings.Add(dataSO);
                break;
        }
    }

    private void AddDefenseBuildings(DefenseBuildingDataSO dataSO)
    {
        if (dataSO == null) return;
        var faction = dataSO.buildingIdentity.faction;

        switch (dataSO.defenseType)
        {
            case ScenarioDefenseType.AntiAir:
                defenseBuildings[(int)faction].antiAirBuildings.Add(dataSO);
                break;
            case ScenarioDefenseType.AntiTank:
                defenseBuildings[(int)faction].antiTankBuildings.Add(dataSO);
                break;
            case ScenarioDefenseType.Turret:
                defenseBuildings[(int)faction].turretBuildings.Add(dataSO);
                break;
            case ScenarioDefenseType.Wall:
                defenseBuildings[(int)faction].wallBuildings.Add(dataSO);
                break;
        }
    }

    public List<BuildingDataSO> GetFactionDefenseBuildingsSO(FactionName faction)
    {
        var defenseSO = new List<BuildingDataSO>();

        foreach (var buildingSO in allBuildingsSO)
        {
            if (buildingSO.buildingIdentity.faction == faction && buildingSO.buildingType == ScenarioBuildingType.DefenseBuilding)
                defenseSO.Add(buildingSO);
        }

        return defenseSO;
    }

    public List<BuildingDataSO> GetFactionOffenseBuildingsSO(FactionName faction)
    {
        var offenseSO = new List<BuildingDataSO>();

        foreach (var buildingSO in allBuildingsSO)
        {
            if (buildingSO.buildingIdentity.faction == faction && buildingSO.buildingType == ScenarioBuildingType.OffenseBuilding)
                offenseSO.Add(buildingSO);
        }

        return offenseSO;
    }

    public List<ResourceBuildingDataSO> GetFactionResourceBuildingsSO(FactionName faction)
    {
        var resourceSO = new List<ResourceBuildingDataSO>();

        foreach (var buildingSO in allBuildingsSO)
        {
            if (buildingSO.buildingIdentity.faction == faction && buildingSO.buildingType == ScenarioBuildingType.ResourceBuilding)
            {
                if (buildingSO is ResourceBuildingDataSO resourceBuildingDataSO)
                    resourceSO.Add(resourceBuildingDataSO);
            }
        }

        resourceSO.Sort(CompareResourceSO);

        return resourceSO;
    }

    public ResourceBuildingDataSO GetResourceBuildingSO(FactionName faction, ScenarioResourceType resourceType)
    {
        foreach (var buildingSO in allBuildingsSO)
        {
            if (buildingSO.buildingIdentity.faction == faction && buildingSO.buildingType == ScenarioBuildingType.ResourceBuilding && ((ResourceBuildingDataSO)buildingSO).resourceType == resourceType)
                return (ResourceBuildingDataSO)buildingSO;
        }

        return null;
    }

    public List<BuildingDataSO> GetDefenseBuildingsSO()
    {
        var defenseBuildingSO = new List<BuildingDataSO>();

        foreach (var buildingSO in allBuildingsSO)
        {
            if (buildingSO.buildingType == ScenarioBuildingType.DefenseBuilding)
                defenseBuildingSO.Add(buildingSO);
        }

        return defenseBuildingSO;
    }
    private static int CompareResourceSO(ScriptableObject x, ScriptableObject y)
    {
        var rx = (ResourceBuildingDataSO)x;
        var ry = (ResourceBuildingDataSO)y;
        return rx.resourceType.CompareTo(ry.resourceType);
    }
    private void OnValidate()
    {
        var enumValues = Enum.GetValues(typeof(FactionName));

        if (wallParentBuildings == null)
            wallParentBuildings = new List<WallParentBuilding>();

        while (wallParentBuildings.Count < enumValues.Length)
            wallParentBuildings.Add(new WallParentBuilding());

        while (wallParentBuildings.Count > enumValues.Length)
            wallParentBuildings.RemoveAt(wallParentBuildings.Count - 1);

        for (int i = 0; i < wallParentBuildings.Count; i++)
            wallParentBuildings[i].faction = (FactionName)enumValues.GetValue(i);
    }

}
[Serializable]
public class OffenseBuilding
{
    public FactionName faction;
    public List<OffenseBuildingDataSO> airBuildings;
    public List<OffenseBuildingDataSO> aoeRangedBuildings;
    public List<OffenseBuildingDataSO> meleeBuildings;
    public List<OffenseBuildingDataSO> rangedBuildings;

    public OffenseBuilding()
    {
        airBuildings = new List<OffenseBuildingDataSO>();
        aoeRangedBuildings = new List<OffenseBuildingDataSO>();
        meleeBuildings = new List<OffenseBuildingDataSO>();
        rangedBuildings = new List<OffenseBuildingDataSO>();
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

    public ResourceBuilding()
    {
        foodResourceBuildings = new List<ResourceBuildingDataSO>();
        goldResourceBuildings = new List<ResourceBuildingDataSO>();
        metalResourceBuildings = new List<ResourceBuildingDataSO>();
        powerResourceBuildings = new List<ResourceBuildingDataSO>();
    }
}

[Serializable]
public class DefenseBuilding
{
    public FactionName faction;
    public List<DefenseBuildingDataSO> antiAirBuildings;
    public List<DefenseBuildingDataSO> antiTankBuildings;
    public List<DefenseBuildingDataSO> turretBuildings;
    public List<DefenseBuildingDataSO> wallBuildings;

    public DefenseBuilding()
    {
        antiAirBuildings = new List<DefenseBuildingDataSO>();
        antiTankBuildings = new List<DefenseBuildingDataSO>();
        turretBuildings = new List<DefenseBuildingDataSO>();
        wallBuildings = new List<DefenseBuildingDataSO>();
    }
}
[Serializable]
public class WallParentBuilding
{
    public FactionName faction;
    public WallParent wallParentBuilding;
}