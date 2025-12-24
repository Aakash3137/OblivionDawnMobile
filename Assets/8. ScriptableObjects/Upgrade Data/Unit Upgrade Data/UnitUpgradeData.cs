using UnityEngine;

[System.Serializable]
public class UnitUpgradeData
{
    public int unitLevel;
    public Mesh unitMesh;
    public float unitHealth;
    public float unitFireRate;
    public float unitAttackDamage;
    public float unitMoveSpeed;
    public float unitAttackRange;
    public float unitBuildTime;
    [Tooltip("The cost to upgrade a unit.")]
    public UnitUpgradeCost[] unitUpgradeCosts;
}
[System.Serializable]
public struct UnitUpgradeCost
{
    public ResourceType resourceType;
    public float resourceCost;
}
