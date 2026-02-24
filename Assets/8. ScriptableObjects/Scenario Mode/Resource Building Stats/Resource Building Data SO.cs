using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Building Data SO", menuName = "Scenario Stats/Resource Building Data")]
public class ResourceBuildingDataSO : BuildingDataSO
{
    [Space(30)]
    public ScenarioResourceType resourceType;
    public List<ResourceBuildingUpgradeData> resourceBuildingUpgradeData;

    internal override void ValidateBase()
    {
        if (resourceBuildingUpgradeData.Count == 0)
            resourceBuildingUpgradeData = new List<ResourceBuildingUpgradeData> { new ResourceBuildingUpgradeData() };

        for (int i = 0; i < resourceBuildingUpgradeData.Count; i++)
        {
            resourceBuildingUpgradeData[i].buildingLevel = i;

            //resourceBuildingUpgradeData[i].resourceGenerationRate = resourceBuildingUpgradeData[i].resourceAmountPerBatch / resourceBuildingUpgradeData[i].resourceTimeToProduce;
            resourceBuildingUpgradeData[i].resourceGenerationRate = resourceBuildingUpgradeData[i].resourceAmountPerBatch;
        }

        buildingIdentity.spawnLevel = Mathf.Clamp(buildingIdentity.spawnLevel, 0, resourceBuildingUpgradeData.Count - 1);
        base.ValidateBase();
    }
}

[Serializable]
public class ResourceBuildingUpgradeData : BuildingUpgradeData
{
    public int resourceAmountPerBatch;
    [field: SerializeField] public float resourceTimeToProduce { get; private set; }
    public int resourceAmountCapacity;
    [ReadOnly]
    public float resourceGenerationRate;
}