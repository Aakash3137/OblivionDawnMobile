using UnityEngine;

public class DefenseBuilding : BuildingBlueprint
{
    [Space(50)]
    public ScenarioDefenseType defenseType;

    // Initialize this when spawning building
    internal override void Initialize(BuildingDataSO buildingData, Tile tile)
    {
        buildingType = ScenarioBuildingType.DefenseBuilding;

        base.Initialize(buildingData, tile);

        if (buildingData is DefenseBuildingDataSO defenseBuildingDataSO)
        {
            defenseType = defenseBuildingDataSO.defenseType;
            InitializeDefenseBuilding(defenseBuildingDataSO);
        }
        else
        {
            Debug.Log($"<color=red>Building {name} missing DefenseBuildingDataSO. Assign the correct ScriptableObject.</color>");
        }
    }

    private void InitializeDefenseBuilding(DefenseBuildingDataSO buildingData)
    {
        basicStatsComponent.Initialize(buildingData.defenseBuildingUpgradeData[spawnLevel].buildingBasicStats);
    }
    internal override void AssignComponents()
    {
        base.AssignComponents();
    }

    internal override void HandleKillData()
    {
        base.HandleKillData();
    }

    internal override void EventsHandler()
    {
        base.EventsHandler();
    }

    internal override void OnDisable()
    {
        base.OnDisable();
    }
}
