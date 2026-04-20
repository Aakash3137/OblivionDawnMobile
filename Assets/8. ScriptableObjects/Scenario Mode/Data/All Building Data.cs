using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "All Building Data", menuName = "Data/All Building Data")]
public class AllBuildingData : SerializedScriptableObject
{
    [SerializeField, Space(10)] private List<MainBuildingDataSO> _mainBuildingSO;
    public List<MainBuildingDataSO> mainBuildingSO => _mainBuildingSO;

    [SerializeField, Space(10)] private List<BuildingDataSO> _allBuildingsSO;
    public List<BuildingDataSO> allBuildingsSO => _allBuildingsSO;

    [Space(10)] public Dictionary<FactionName, WallParent> wallParentBuilding;

    private ScriptableRegistry<OffenseBuildingDataSO, FactionName, ScenarioOffenseType> _offenseSO =
        new(so => so.buildingIdentity.faction, so => so.offenseType, ScenarioDataTypes._offenseEnumValues);
    private ScriptableRegistry<DefenseBuildingDataSO, FactionName, ScenarioDefenseType> _defenseSO =
        new(so => so.buildingIdentity.faction, so => so.defenseType, ScenarioDataTypes._defenseEnumValues);
    private ScriptableRegistry<ResourceBuildingDataSO, FactionName, ScenarioResourceType> _resourceSO =
        new(so => so.buildingIdentity.faction, so => so.resourceType, ScenarioDataTypes._resourceEnumValues);

    public List<OffenseBuildingDataSO> AllOffenseBuildingSO => _offenseSO.All;
    public List<DefenseBuildingDataSO> AllDefenseBuildingSO => _defenseSO.All;
    public List<ResourceBuildingDataSO> AllResourceBuildingSO => _resourceSO.All;

    public List<OffenseBuildingDataSO> GetOffenseBuildingsSO(FactionName faction) => _offenseSO.ByFaction(faction);
    public List<OffenseBuildingDataSO> GetOffenseBuildingsSO(ScenarioOffenseType type) => _offenseSO.ByType(type);
    public List<OffenseBuildingDataSO> GetOffenseBuildingsSO(FactionName faction, ScenarioOffenseType type) => _offenseSO.ByFactionAndType(faction, type);

    public List<DefenseBuildingDataSO> GetDefenseBuildingsSO(FactionName faction) => _defenseSO.ByFaction(faction);
    public List<DefenseBuildingDataSO> GetDefenseBuildingsSO(ScenarioDefenseType type) => _defenseSO.ByType(type);
    public List<DefenseBuildingDataSO> GetDefenseBuildingsSO(FactionName faction, ScenarioDefenseType type) => _defenseSO.ByFactionAndType(faction, type);

    public List<ResourceBuildingDataSO> GetResourceBuildingsSO(FactionName faction) => _resourceSO.ByFaction(faction);
    public List<ResourceBuildingDataSO> GetResourceBuildingsSO(ScenarioResourceType type) => _resourceSO.ByType(type);
    public List<ResourceBuildingDataSO> GetResourceBuildingsSO(FactionName faction, ScenarioResourceType type) => _resourceSO.ByFactionAndType(faction, type);

    private void Awake()
    {
        Populate();
    }
    private void OnValidate()
    {
        //  need this for editor mode Upgrade Data Editor Window
        Populate();
    }

    public void Populate()
    {
        _allBuildingsSO.Sort(CompareBuildingSO);
        _mainBuildingSO.Sort(CompareMainSO);

        foreach (var building in _allBuildingsSO)
        {
            switch (building)
            {
                case OffenseBuildingDataSO:
                    _offenseSO.Register((OffenseBuildingDataSO)building);
                    break;
                case DefenseBuildingDataSO:
                    _defenseSO.Register((DefenseBuildingDataSO)building);
                    break;
                case ResourceBuildingDataSO:
                    _resourceSO.Register((ResourceBuildingDataSO)building);
                    break;
            }
        }
    }

    #region Sorting internal 
    private static int CompareBuildingSO(BuildingDataSO buildingA, BuildingDataSO buildingB)
    {
        int order = buildingA.buildingIdentity.faction.CompareTo(buildingB.buildingIdentity.faction);

        // if(both are same then compare by buildingType)
        if (order == 0)
            order = buildingA.buildingType.CompareTo(buildingB.buildingType);

        // if building type is same compare with it own types
        if (order == 0)
        {
            switch (buildingA)
            {
                case MainBuildingDataSO:
                    order = CompareMainSO((MainBuildingDataSO)buildingA, (MainBuildingDataSO)buildingB);
                    break;
                case OffenseBuildingDataSO:
                    order = CompareOffenseSO((OffenseBuildingDataSO)buildingA, (OffenseBuildingDataSO)buildingB);
                    break;
                case DefenseBuildingDataSO:
                    order = CompareDefenseSO((DefenseBuildingDataSO)buildingA, (DefenseBuildingDataSO)buildingB);
                    break;
                case ResourceBuildingDataSO:
                    order = CompareResourceSO((ResourceBuildingDataSO)buildingA, (ResourceBuildingDataSO)buildingB);
                    break;
            }
        }

        return order;
    }
    private static int CompareMainSO(MainBuildingDataSO a, MainBuildingDataSO b)
    {
        int order = a.buildingIdentity.faction.CompareTo(b.buildingIdentity.faction);

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
}