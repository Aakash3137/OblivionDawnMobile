using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Unit Upgrade Stats", menuName = "Scenario Stats/Unit Upgrade Stats")]
public class UnitProduceStatsSO : ScriptableObject
{
    public GameUnitName gameUnitName;
    public Identity unitIdentity;
    public ScenarioUnitType unitType;
    public Visuals unitVisuals;
    public VisionAngles unitVisionAngles;
    public AttackTargets unitAttackTargets;
    public CardDetails cardDetails;

    [Space(5)]
    public Sprite unitIcon;

    [Space(20)]
    public bool hasUpkeep;
    [ShowIf(nameof(hasUpkeep))]
    public BuildCost[] upKeepCost;
    [field: SerializeField, Space(10), ShowIf(nameof(hasUpkeep))]
    public float upKeepTime { get; private set; }

    [Space(20)]
    public OffenseBuildingStats spawnerBuilding;

    public bool canFly;
    [ShowIf(nameof(canFly))]
    public FlyStats unitFlyStats;

    public int populationCost;

    [Space(20), Header("Unit Upgrade Data")]
    public UnitUpgradeData[] unitUpgradeData;

    [Space(20), Header("Abilities")]
    public List<AbilitySO> abilities;

    public float GetUnitSpawnTime()
    {
        return unitUpgradeData[unitIdentity.spawnLevel].unitSpawnTime;
    }
#if UNITY_EDITOR
    [Button]
    public void GenerateLevels()
    {
        StatUpgrade.GenerateUpgradeData(this);
    }
#endif
    private void ValidateBase()
    {
        if (unitUpgradeData == null || unitUpgradeData.Length == 0)
        {
            unitUpgradeData = new UnitUpgradeData[1];
            unitUpgradeData[0] = new UnitUpgradeData();
        }

        for (int i = 0; i < unitUpgradeData.Length; i++)
        {
            unitUpgradeData[i].unitLevel = i + 1;
        }

        canFly = unitType == ScenarioUnitType.Air;

        unitIdentity.name = gameUnitName.ToString();

        unitIdentity.spawnLevel = Mathf.Clamp(unitIdentity.spawnLevel, 0, unitUpgradeData.Length - 1);

        if (!hasUpkeep)
            return;

        var enumValues = ScenarioDataTypes._resourceEnumValues;
        int targetLength = enumValues.Length;

        upKeepCost = BuildCostUtils.ResizePreservingData(upKeepCost, targetLength);

        for (int j = 0; j < upKeepCost.Length; j++)
            upKeepCost[j].resourceType = enumValues[j];

        if (cardDetails.upgradeCostMultiplier < 0)
            cardDetails.upgradeCostMultiplier = 1f;

        if (cardDetails.fragmentCostMultiplier < 0)
            cardDetails.fragmentCostMultiplier = 1f;
    }

    private void OnValidate()
    {
        ValidateBase();
    }

    [ShowIf(nameof(hasUpkeep)), Button]
    public void SetUpKeepCost(float increasePercent)
    {
        float discount = 1f + (increasePercent / 100f);

        for (int i = 0; i < upKeepCost.Length; i++)
        {
            if (upKeepCost[i].resourceAmount != 0)
            {
                var amt = Mathf.RoundToInt(upKeepCost[i].resourceAmount * discount);
                upKeepCost[i].resourceAmount = Mathf.Max(amt, 1);
            }
        }
    }
}

[Serializable]
public class UnitUpgradeData
{
    // show actual level
    public int unitLevel;
    public float unitSpawnTime;
    public BasicStats unitBasicStats;
    public AttackStats unitAttackStats;
    public MobilityStats unitMobilityStats;
    public RangeStats unitRangeStats;
}

