using UnityEngine;

[System.Serializable]
public class BuildingUpgradeData
{
    public Mesh buildingMesh;
    public float buildingHealth;
    public float buildingFireRate;
    public float buildingAttackDamage;
    public float buildingAttackRange;
    public float buildingBuildTime;

    [Tooltip("Resources array 0 = Gold, 1 = Food, 2 = Power, 3 = Metal")]
    public float[] buildingResourcesCost;
}
