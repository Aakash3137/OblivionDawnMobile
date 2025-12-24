using UnityEngine;

[System.Serializable]
public class BuildingUpgradeData
{
    public int buildingLevel;
    public Mesh buildingMesh;
    public float buildingHealth;
    public float buildingFireRate;
    public float buildingAttackDamage;
    public float buildingAttackRange;
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
