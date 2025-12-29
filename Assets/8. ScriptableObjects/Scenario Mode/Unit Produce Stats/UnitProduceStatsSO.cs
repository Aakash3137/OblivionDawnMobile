using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Upgrade Stats", menuName = "Scenario Stats/Unit Upgrade Stats")]
public class UnitProduceStatsSO : ScriptableObject
{

    [Header("Upgrade data is stored in array 0 represents Base Level. 1-n can represent level of upgrade")]
    public string unitName;
    public ScenarioUnitType unitType;
    public GameObject unitPrefab;
    public FactionName unitFactionName;

    [Tooltip("Unit Stats and Upgrade Costs per level")]
    public UnitUpgradeData[] unitLevelData;

    private void OnValidate()
    {
        // if (unitLevelData == null) return;

        // for (int i = 0; i < unitLevelData.Length; i++)
        // {
        //     var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

        //     unitLevelData[i].unitUpgradeCosts = new UnitUpgradeCost[enumValues.Length];

        //     for (int j = 0; j < enumValues.Length; j++)
        //     {
        //         unitLevelData[i].unitUpgradeCosts[j] = new UnitUpgradeCost();
        //         unitLevelData[i].unitUpgradeCosts[j].resourceType = (ScenarioResourceType)enumValues.GetValue(j);
        //     }
        // }
    }
}
[Serializable]
public class UnitUpgradeData
{
    public float unitHealth;
    public float unitArmour;
    public float unitFireRate;
    public float unitAttackDamage;
    public float unitMoveSpeed;
    public float unitAttackRange;
    public float unitBuildTime;

    // [Tooltip("The cost to upgrade a unit.")]
    // public UnitUpgradeCost[] unitUpgradeCosts;
}
[Serializable]
public struct UnitUpgradeCost
{
    public ScenarioResourceType resourceType;
    public int resourceCost;
}