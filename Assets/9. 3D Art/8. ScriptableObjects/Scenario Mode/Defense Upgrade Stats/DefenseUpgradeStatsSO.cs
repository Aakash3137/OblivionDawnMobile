using UnityEngine;

[CreateAssetMenu(fileName = "Defense Upgrade Stats", menuName = "Scenario Stats/Defense Upgrade Stats")]
public class DefenseUpgradeStatsSO : ScriptableObject
{
    [Header("Defense Building specific stats and stats per level")]
    public string defenseBuildingName;
    public ScenarioDefenseType defenseBuildingType;

    [Tooltip("Defense building Stats per level")]
    public DefenseUpgradeData[] defenseUpgradeData;

    private void OnValidate()
    {
        for (int i = 0; i < defenseUpgradeData.Length; i++)
        {
            defenseUpgradeData[i].level = i;
        }
    }
}

[System.Serializable]
public class DefenseUpgradeData
{
    public int level;
    public float buildingFireRate;
    public float buildingAttackDamage;
    public float buildingAttackRange;
}