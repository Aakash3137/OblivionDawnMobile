using UnityEngine;

public class ResourceBuilding : BuildingBlueprint
{
    [Space(50)]
    public ScenarioResourceType resourceType;
    internal ResourceGenerationComponent resourceGenerationComponent;

    // Initialize this when spawning building
    internal override void Initialize(BuildingDataSO buildingData, Tile tile)
    {
        buildingType = ScenarioBuildingType.ResourceBuilding;

        base.Initialize(buildingData, tile);

        if (buildingData is ResourceBuildingDataSO resourceBuildingDataSO)
        {
            resourceType = resourceBuildingDataSO.resourceType;
            InitializeResourceBuilding(resourceBuildingDataSO);
        }
        else
        {
            Debug.Log($"<color=red>Building {name} missing ResourceBuildingDataSO. Assign the correct ScriptableObject.</color>");
        }

        EventsHandler();
    }

    private void InitializeResourceBuilding(ResourceBuildingDataSO buildingData)
    {
        basicStatsComponent.Initialize(buildingData.resourceBuildingUpgradeData[spawnLevel].buildingBasicStats);
        resourceGenerationComponent.Initialize(buildingData, spawnLevel);
    }

    internal override void AssignComponents()
    {
        base.AssignComponents();
        resourceGenerationComponent = TryGetComponent<ResourceGenerationComponent>(out var resourceGeneration) ? resourceGeneration : null;
    }

    internal override void HandleKillData()
    {
        base.HandleKillData();
    }

    // Don't forget to call this in each script
    internal override void EventsHandler()
    {
        base.EventsHandler();
        resourceGenerationComponent.ResourceGenerationRateHandler(resourceType, 1);
    }

    internal override void OnDisable()
    {
        base.OnDisable();
        resourceGenerationComponent.ResourceGenerationRateHandler(resourceType, -1);
        KillCounterManager.Instance.AddResourceBuildingDestroyedData(resourceType, identityComponent.side);
    }
}
