using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Building Upgrade Data", menuName = "Upgrade Data/Building Upgrade Data")]
public class BuildingUpgradeDataSO : ScriptableObject
{

    [Header("Upgrade data is stored in array 0 represents Base Level. 1-n can represent level of upgrade")]
    public string buildingName;
    public GameObject buildingPrefab;
    public BuildingType buildingType;
    public FactionName buildingFactionName;

    [Tooltip("Level 0 = base building")]
    public BuildingUpgradeData[] buildingLevelData;

    private void OnValidate()
    {
        if (buildingLevelData == null) return;

        for (int i = 0; i < buildingLevelData.Length; i++)
        {
            buildingLevelData[i].buildingLevel = i;

            var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

            // Only create the array if it doesn't exist or size is wrong
            if (buildingLevelData[i].buildingUpgradeCosts == null || buildingLevelData[i].buildingUpgradeCosts.Length != enumValues.Length)
            {
                buildingLevelData[i].buildingUpgradeCosts = new BuildingUpgradeCost[enumValues.Length];
            }

            for (int j = 0; j < enumValues.Length; j++)
            {
                // Set the resource type
                buildingLevelData[i].buildingUpgradeCosts[j].resourceType = (ScenarioResourceType)enumValues.GetValue(j);
            }
        }
    }
}

[Serializable]
public class BuildingUpgradeData
{
    public int buildingLevel;
    public Mesh buildingMesh;
    public float buildingHealth;
    public float buildingArmour;
    public float buildingBuildTime;

    [Header("Resource costs required for this upgrade")]
    [Tooltip("Resource Type are auto set from enum values of ScenarioResourceType")]
    public BuildingUpgradeCost[] buildingUpgradeCosts;
}
[Serializable]
public struct BuildingUpgradeCost
{
    public ScenarioResourceType resourceType;
    public int resourceCost;
}