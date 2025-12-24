using UnityEngine;

[System.Serializable]
public class UnitUpgradeData
{    
    public Mesh unitMesh;
    public float unitHealth;
    public float unitFireRate;
    public float unitAttackDamage;
    public float unitMoveSpeed;
    public float unitAttackRange;
    public float unitBuildTime;

    [Tooltip("Resources array 0 = Gold, 1 = Food, 2 = Power, 3 = Metal")]
    public float[] unitResourcesCost;
}
