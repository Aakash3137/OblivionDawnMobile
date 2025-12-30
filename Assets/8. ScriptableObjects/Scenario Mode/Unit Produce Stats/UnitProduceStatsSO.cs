using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Upgrade Stats", menuName = "Scenario Stats/Unit Upgrade Stats")]
public class UnitProduceStatsSO : ScriptableObject
{
    [Header("Unit Building specific stats and stats per level")]
    public string unitName;
    public ScenarioUnitType unitType;
    public GameObject unitPrefab;
    public FactionName unitFactionName;

    [Tooltip("Unit Stats and Upgrade Costs per level")]
    public UnitUpgradeData[] unitLevelData;
    private void OnValidate()
    {
        for (int i = 0; i < unitLevelData.Length; i++)
        {
            unitLevelData[i].level = i;
        }
    }
}
[Serializable]
public class UnitUpgradeData
{

    public int level;
    public float unitHealth;
    public float unitArmour;
    public float unitFireRate;
    public float unitAttackDamage;
    public float unitMoveSpeed;
    public float unitAttackRange;
    public float unitBuildTime;
}