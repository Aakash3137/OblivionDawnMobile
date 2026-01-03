using UnityEngine;

[CreateAssetMenu(fileName = "Defense Upgrade Stats", menuName = "Scenario Stats/Defense Upgrade Stats")]
public class DefenseUpgradeStatsSO : ScriptableObject
{
    public string defenseBuildingName;
    public ScenarioDefenseType defenseBuildingType;

    [Tooltip("Defense building Stats per level")]
    public DefenseUpgradeData[] defenseUpgradeData;
}

[System.Serializable]
public class DefenseUpgradeData
{    
    public float buildingFireRate;
    public float buildingAttackDamage;
    public float buildingAttackRange;
}