using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Building Upgrade Data", menuName = "Upgrade Data/Building Upgrade Data")]
public class BuildingUpgradeDataSO : ScriptableObject
{
    [Header("Building health stats and Resource cost for upgrades")]
    public string buildingName;
    public GameObject buildingPrefab;
    public ScenarioBuildingType buildingType;
    public FactionName buildingFactionName;

    [Tooltip("Level 0 = base building")]
    public BuildingUpgradeData[] buildingLevelData;

    protected virtual void ValidateBase()
    {
        if (buildingLevelData == null)
        {
            buildingLevelData = new BuildingUpgradeData[1];
            buildingLevelData[0] = new BuildingUpgradeData();
        }

        for (int i = 0; i < buildingLevelData.Length; i++)
        {
            buildingLevelData[i].buildingLevel = i;

            var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

            if (buildingLevelData[i].buildingUpgradeCosts == null ||
                buildingLevelData[i].buildingUpgradeCosts.Length != enumValues.Length)
            {
                buildingLevelData[i].buildingUpgradeCosts =
                    new BuildingUpgradeCost[enumValues.Length];
            }

            for (int j = 0; j < enumValues.Length; j++)
            {
                buildingLevelData[i].buildingUpgradeCosts[j].resourceType =
                    (ScenarioResourceType)enumValues.GetValue(j);
            }
        }
    }
    private void OnValidate()
    {
        ValidateBase();
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