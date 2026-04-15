using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Building Data", menuName = "Data/All Building Data")]
public class AllBuildingData : ScriptableObject
{
    public List<MainBuildingDataSO> mainBuildingSO;
    [Space(10)]
    public List<BuildingDataSO> allBuildingsSO;
    [Space(10)]
    public List<WallParentBuilding> wallParentBuildings;

    public List<OffenseBuilding> offenseBuildings { get; internal set; }
    public List<ResourceBuilding> resourceBuildings { get; internal set; }
    public List<DefenseBuilding> defenseBuildings { get; internal set; }

    public List<OffenseBuildingDataSO> offenseBuildingsSO { get; internal set; }
    public List<ResourceBuildingDataSO> resourceBuildingsSO { get; internal set; }
    public List<DefenseBuildingDataSO> defenseBuildingsSO { get; internal set; }

    private void Awake()
    {
        Populate();
        ValidateBase();

        allBuildingsSO.Sort(CompareBuildingSO);
    }
    private void OnValidate()
    {
        //  need this for editor mode Upgrade Data Editor Window
        Populate();
        ValidateBase();
        allBuildingsSO.Sort(CompareBuildingSO);
    }

    public void Populate()
    {
        // Clear Previous Data and repopulate
        var enumValues = ScenarioDataTypes._factionEnumValues;

        offenseBuildings = new List<OffenseBuilding>();
        resourceBuildings = new List<ResourceBuilding>();
        defenseBuildings = new List<DefenseBuilding>();

        offenseBuildingsSO = new List<OffenseBuildingDataSO>();
        resourceBuildingsSO = new List<ResourceBuildingDataSO>();
        defenseBuildingsSO = new List<DefenseBuildingDataSO>();

        foreach (FactionName factionName in enumValues)
        {
            offenseBuildings.Add(new OffenseBuilding { faction = factionName });
            resourceBuildings.Add(new ResourceBuilding { faction = factionName });
            defenseBuildings.Add(new DefenseBuilding { faction = factionName });
        }

        AddBuildingsDataSO();

        SortBuildingData();
    }

    #region  Data Management to custom Class
    private void AddBuildingsDataSO()
    {
        foreach (var buildingSO in allBuildingsSO)
        {
            switch (buildingSO.buildingType)
            {
                case ScenarioBuildingType.ResourceBuilding:
                    resourceBuildingsSO.Add((ResourceBuildingDataSO)buildingSO);
                    AddResourceBuildings((ResourceBuildingDataSO)buildingSO);
                    break;
                case ScenarioBuildingType.OffenseBuilding:
                    offenseBuildingsSO.Add((OffenseBuildingDataSO)buildingSO);
                    AddOffenseBuildings((OffenseBuildingDataSO)buildingSO);
                    break;
                case ScenarioBuildingType.DefenseBuilding:
                    defenseBuildingsSO.Add((DefenseBuildingDataSO)buildingSO);
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
    #endregion

    public List<DefenseBuildingDataSO> GetFactionDefenseBuildingsSO(FactionName faction)
    {
        var defenseSO = new List<DefenseBuildingDataSO>();

        foreach (var buildingSO in defenseBuildingsSO)
        {
            if (buildingSO.buildingIdentity.faction == faction)
                defenseSO.Add(buildingSO);
        }
        return defenseSO;
    }

    public List<OffenseBuildingDataSO> GetFactionOffenseBuildingsSO(FactionName faction)
    {
        var offenseSO = new List<OffenseBuildingDataSO>();

        foreach (var buildingSO in offenseBuildingsSO)
        {
            if (buildingSO.buildingIdentity.faction == faction)
                offenseSO.Add(buildingSO);
        }
        return offenseSO;
    }

    public List<ResourceBuildingDataSO> GetFactionResourceBuildingsSO(FactionName faction)
    {
        var resourceSO = new List<ResourceBuildingDataSO>();

        foreach (var buildingSO in resourceBuildingsSO)
        {
            if (buildingSO.buildingIdentity.faction == faction)
            {
                resourceSO.Add(buildingSO);
            }
        }

        resourceSO.Sort(CompareResourceSO);
        return resourceSO;
    }

    public ResourceBuildingDataSO GetResourceBuildingSO(FactionName faction, ScenarioResourceType resourceType)
    {
        foreach (var buildingSO in resourceBuildingsSO)
        {
            if (buildingSO.buildingIdentity.faction == faction && buildingSO.resourceType == resourceType)
                return buildingSO;
        }

        return null;
    }

    #region Sorting internal 
    private void SortBuildingData()
    {
        offenseBuildingsSO.Sort(CompareOffenseSO);
        defenseBuildingsSO.Sort(CompareDefenseSO);
        resourceBuildingsSO.Sort(CompareResourceSO);
        mainBuildingSO.Sort(CompareMainSO);
    }
    private static int CompareBuildingSO(BuildingDataSO a, BuildingDataSO b)
    {
        int order = a.buildingIdentity.faction.CompareTo(b.buildingIdentity.faction);

        // if(both are same then compare by buildingType)
        if (order == 0)
            order = a.buildingType.CompareTo(b.buildingType);

        // if building type is same compare with it own types
        if (order == 0)
        {
            if (a is MainBuildingDataSO)
                order = CompareMainSO((MainBuildingDataSO)a, (MainBuildingDataSO)b);
            else if (a is OffenseBuildingDataSO)
                order = CompareOffenseSO(a as OffenseBuildingDataSO, b as OffenseBuildingDataSO);
            else if (a is DefenseBuildingDataSO)
                order = CompareDefenseSO(a as DefenseBuildingDataSO, b as DefenseBuildingDataSO);
            else if (a is ResourceBuildingDataSO)
                order = CompareResourceSO(a as ResourceBuildingDataSO, b as ResourceBuildingDataSO);
        }

        return order;
    }
    private static int CompareMainSO(MainBuildingDataSO a, MainBuildingDataSO b)
    {
        int order = a.buildingIdentity.faction.CompareTo(b.buildingIdentity.faction);

        // if(both are same then compare by defenseType)
        // if (order == 0)
        // order = a.defenseType.CompareTo(b.defenseType);

        return order;
    }
    private static int CompareOffenseSO(OffenseBuildingDataSO a, OffenseBuildingDataSO b)
    {
        int order = a.buildingIdentity.faction.CompareTo(b.buildingIdentity.faction);

        // if(both are same then compare by offenseType)
        if (order == 0)
            order = a.offenseType.CompareTo(b.offenseType);

        return order;
    }
    private static int CompareDefenseSO(DefenseBuildingDataSO a, DefenseBuildingDataSO b)
    {
        int order = a.buildingIdentity.faction.CompareTo(b.buildingIdentity.faction);

        // if(both are same then compare by defenseType)
        if (order == 0)
            order = a.defenseType.CompareTo(b.defenseType);

        return order;
    }
    private static int CompareResourceSO(ResourceBuildingDataSO a, ResourceBuildingDataSO b)
    {
        int order = a.buildingIdentity.faction.CompareTo(b.buildingIdentity.faction);

        // if(both are same then compare by resourceType)
        if (order == 0)
            order = a.resourceType.CompareTo(b.resourceType);

        return order;
    }
    #endregion

    private void ValidateBase()
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