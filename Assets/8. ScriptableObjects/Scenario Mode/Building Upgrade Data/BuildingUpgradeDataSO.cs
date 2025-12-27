using UnityEngine;

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
        }
    }
}

[System.Serializable]
public class BuildingUpgradeData
{
    public int buildingLevel;
    public Mesh buildingMesh;
    public float buildingHealth;
    public float buildingArmour;
    public float buildingBuildTime;
    [Tooltip("Resource costs required for this upgrade")]
    public BuildingUpgradeCost[] buildingUpgradeCosts;
}
[System.Serializable]
public struct BuildingUpgradeCost
{
    public ResourceType resourceType;
    public float resourceCost;
}