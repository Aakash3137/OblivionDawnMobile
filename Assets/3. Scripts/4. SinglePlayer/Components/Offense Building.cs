using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class OffenseBuilding : BuildingBlueprint
{
    [Space(50)]
    public ScenarioOffenseType offenseType;
    public UnitProduceStatsSO unitProduceStats { get; private set; }
    internal SpawnerComponent spawnerComponent;

    // Initialize this when spawning building
    internal override void Initialize(BuildingDataSO buildingData, Tile tile)
    {
        buildingType = ScenarioBuildingType.OffenseBuilding;

        base.Initialize(buildingData, tile);

        if (buildingData is OffenseBuildingDataSO offenseBuildingDataSO)
        {
            offenseType = offenseBuildingDataSO.offenseType;
            InitializeOffenseBuilding(offenseBuildingDataSO);
        }
        else
        {
            Debug.Log($"<color=red>Building {name} missing OffenseBuildingDataSO. Assign the correct ScriptableObject.</color>");
        }
    }
    private void InitializeOffenseBuilding(OffenseBuildingDataSO buildingData)
    {
        basicStatsComponent.Initialize(buildingData.offenseBuildingUpgradeData[spawnLevel].buildingBasicStats);
        //take spawn level from here and cut more things in spawner component
        spawnerComponent.Initialize(unitProduceStats);
    }
    internal override void AssignComponents()
    {
        base.AssignComponents();
        spawnerComponent = TryGetComponent<SpawnerComponent>(out var spawner) ? spawner : null;
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

        KillCounterManager.Instance.AddOffenseBuildingDestroyedData(offenseType, identityComponent.side);
    }

}
