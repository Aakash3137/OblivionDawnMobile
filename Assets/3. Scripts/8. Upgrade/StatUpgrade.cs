
using UnityEngine;
using System;
using System.Collections.Generic;

public static class StatUpgrade
{
    private static int MaxLevel = GameData.GameMaxObjectLevel;

    // Increase percent per level (Linear Increase)
    private static float HealthChangePercent = 12f;
    private static float ArmorChangePercent = 6f;
    private static float DamageChangePercent = 12f;
    private static float FireRateChangePercent = -5f;
    private static float UnitBuildTimeChangePercent = -3f;
    private static float BuildingBuildTimeChangePercent = -3f;
    private static float MoveSpeedChangePercent = 6f;
    private static float AttackRangeChangePercent = 2f;
    private static float starterResourcesChangePercent = 25f;
    private static float resourceAmountChangePercent = 20f;
    private static float resourceCapacityChangePercent = 20f;


    // Increase value per level(Constant Increase) => Calculate based on max level interval and max and min values
    private static int maxDeckSizeStep;
    private static int maxPopulationStep;


    // Interval after how many levels the range changes
    private static int RangeChangeInterval = 5;
    private static int FireRateChangeInterval = 5;
    private static int BuildTimeChangeInterval = 10;
    private static int MoveSpeedChangeInterval = 8;
    private static int maxDeckSizeInterval = 5;
    private static int maxPopulationInterval = 3;
    private static int starterResourcesInterval = 4;
    private static int resourceAmountInterval = 6;
    private static int resourceCapacityInterval = 4;

    // Clamp Values
    private static int maxDeckEquipCountClamp = GameData.GameMaxDeckSize;
    private static int maxPopulationClamp = GameData.GameMaxPopulation;

    private const float PERCENT = 0.01f;

#if UNITY_EDITOR
    public static void GenerateUpgradeData(ScriptableObject objectSO, bool allLevels = true)
    {
        switch (objectSO)
        {
            case UnitProduceStatsSO unitSO:
                if (allLevels)
                    GenerateUnitUpgradeData(unitSO);
                break;
            case MainBuildingDataSO mainBuildingSO:
                if (allLevels)
                    GenerateMainUpgradeData(mainBuildingSO);
                break;
            case OffenseBuildingDataSO offenseBuildingSO:
                if (allLevels)
                    GenerateOffenseUpgradeData(offenseBuildingSO);
                break;
            case DefenseBuildingDataSO defenseBuildingSO:
                if (allLevels)
                    GenerateDefenseUpgradeData(defenseBuildingSO);
                break;
            case ResourceBuildingDataSO resourceBuildingSO:
                if (allLevels)
                    GenerateResourceUpgradeData(resourceBuildingSO);
                break;
        }
    }

    private static void GenerateUnitUpgradeData(UnitProduceStatsSO unitSO)
    {
        var unitUpgradeData = new UnitUpgradeData[MaxLevel];
        for (int i = 0; i < MaxLevel; i++)
        {
            unitUpgradeData[i] = UnitUpgradeData(unitSO.unitUpgradeData[0], i);
        }
        unitSO.unitUpgradeData = unitUpgradeData;
    }
    private static UnitUpgradeData UnitUpgradeData(UnitUpgradeData baseUpgradeData, int spawnLevel)
    {
        var buildTime = baseUpgradeData.unitSpawnTime;
        var maxHealth = baseUpgradeData.unitBasicStats.maxHealth;
        var armor = baseUpgradeData.unitBasicStats.armor;
        var damage = baseUpgradeData.unitAttackStats.damage;
        var buildingDamage = baseUpgradeData.unitAttackStats.buildingDamage;
        var fireRate = baseUpgradeData.unitAttackStats.fireRate;
        var moveSpeed = baseUpgradeData.unitMobilityStats.moveSpeed;
        var attackRange = baseUpgradeData.unitRangeStats.attackRange;

        var detectionRange = baseUpgradeData.unitRangeStats.detectionRange;
        var minRange = baseUpgradeData.unitRangeStats.minAttackRange;

        var unitUpgradeData = new UnitUpgradeData
        {
            unitLevel = spawnLevel + 1,
            unitSpawnTime = FloatPercentUpgrade(buildTime, UnitBuildTimeChangePercent, spawnLevel, BuildTimeChangeInterval),
            unitBasicStats = new()
            {
                maxHealth = FloatPercentUpgrade(maxHealth, HealthChangePercent, spawnLevel),
                armor = FloatPercentUpgrade(armor, ArmorChangePercent, spawnLevel)
            },
            unitAttackStats = new()
            {
                damage = FloatPercentUpgrade(damage, DamageChangePercent, spawnLevel),
                buildingDamage = FloatPercentUpgrade(buildingDamage, DamageChangePercent, spawnLevel),
                fireRate = FloatPercentUpgrade(fireRate, FireRateChangePercent, spawnLevel, FireRateChangeInterval)
            },
            unitMobilityStats = new()
            {
                moveSpeed = FloatPercentUpgrade(moveSpeed, MoveSpeedChangePercent, spawnLevel, MoveSpeedChangeInterval)
            },
            unitRangeStats = new()
            {
                attackRange = FloatPercentUpgrade(attackRange, AttackRangeChangePercent, spawnLevel, RangeChangeInterval),
                detectionRange = detectionRange,
                minAttackRange = minRange
            }
        };

        return unitUpgradeData;
    }

    private static void GenerateMainUpgradeData(MainBuildingDataSO mainBuildingSO)
    {
        var mainBuildingUpgradeData = new List<MainBuildingUpgradeData>();
        for (int i = 0; i < MaxLevel; i++)
        {
            mainBuildingUpgradeData.Add(MainBuildingData(mainBuildingSO.mainBuildingUpgradeData[0], i));
        }
        mainBuildingSO.mainBuildingUpgradeData = mainBuildingUpgradeData;
    }

    private static MainBuildingUpgradeData MainBuildingData(MainBuildingUpgradeData baseUpgradeData, int spawnLevel)
    {
        var maxHealth = baseUpgradeData.buildingBasicStats.maxHealth;
        var armor = baseUpgradeData.buildingBasicStats.armor;
        var maxDeckSize = baseUpgradeData.maxDeckEquipCount;
        var maxPopulation = baseUpgradeData.maxPopulation;
        var startResources = baseUpgradeData.starterResources;

        // Deck Size Step
        var cardsNeededForMaxLevel = maxDeckEquipCountClamp - maxDeckSize;
        var deckRewardTimesTriggered = MaxLevel / maxDeckSizeInterval;
        maxDeckSizeStep = Mathf.CeilToInt((float)cardsNeededForMaxLevel / deckRewardTimesTriggered);

        // Population Step
        var populationNeededForMaxLevel = maxPopulationClamp - maxPopulation;
        var populationRewardTimesTriggered = MaxLevel / maxPopulationInterval;
        maxPopulationStep = Mathf.CeilToInt((float)populationNeededForMaxLevel / populationRewardTimesTriggered);

        MainBuildingUpgradeData mainBuildingUpgradeData = new()
        {
            buildingLevel = spawnLevel + 1,
            buildingBasicStats = new()
            {
                maxHealth = FloatPercentUpgrade(maxHealth, HealthChangePercent, spawnLevel),
                armor = FloatPercentUpgrade(armor, ArmorChangePercent, spawnLevel)
            },
            maxDeckEquipCount = IntegerStepUpgrade(maxDeckSize, maxDeckSizeStep, spawnLevel, maxDeckEquipCountClamp, maxDeckSizeInterval),
            maxPopulation = IntegerStepUpgrade(maxPopulation, maxPopulationStep, spawnLevel, maxPopulationClamp, maxPopulationInterval),
            starterResources = IntegerPercentUpgrade(startResources, starterResourcesChangePercent, spawnLevel, starterResourcesInterval)
        };

        return mainBuildingUpgradeData;
    }

    private static void GenerateOffenseUpgradeData(OffenseBuildingDataSO offenseBuildingSO)
    {
        var offenseBuildingUpgradeData = new List<OffenseBuildingUpgradeData>();
        for (int i = 0; i < MaxLevel; i++)
        {
            offenseBuildingUpgradeData.Add(OffenseBuildingData(offenseBuildingSO.offenseBuildingUpgradeData[0], i));
        }
        offenseBuildingSO.offenseBuildingUpgradeData = offenseBuildingUpgradeData;
    }

    private static OffenseBuildingUpgradeData OffenseBuildingData(OffenseBuildingUpgradeData baseUpgradeData, int spawnLevel)
    {
        var maxHealth = baseUpgradeData.buildingBasicStats.maxHealth;
        var armor = baseUpgradeData.buildingBasicStats.armor;
        var buildTime = baseUpgradeData.buildingBuildTime;
        var maxSpawnUnits = baseUpgradeData.maxSpawnableUnits;

        OffenseBuildingUpgradeData offenseBuildingUpgradeData = new()
        {
            buildingLevel = spawnLevel + 1,
            buildingBasicStats = new()
            {
                maxHealth = FloatPercentUpgrade(maxHealth, HealthChangePercent, spawnLevel),
                armor = FloatPercentUpgrade(armor, ArmorChangePercent, spawnLevel)
            },
            buildingBuildTime = FloatPercentUpgrade(buildTime, BuildingBuildTimeChangePercent, spawnLevel, BuildTimeChangeInterval),
            maxSpawnableUnits = maxSpawnUnits
        };

        return offenseBuildingUpgradeData;
    }

    private static void GenerateDefenseUpgradeData(DefenseBuildingDataSO defenseBuildingSO)
    {
        var defenseBuildingUpgradeData = new List<DefenseBuildingUpgradeData>();
        for (int i = 0; i < MaxLevel; i++)
        {
            defenseBuildingUpgradeData.Add(DefenseBuildingData(defenseBuildingSO.defenseBuildingUpgradeData[0], i));
        }
        defenseBuildingSO.defenseBuildingUpgradeData = defenseBuildingUpgradeData;
    }

    private static DefenseBuildingUpgradeData DefenseBuildingData(DefenseBuildingUpgradeData baseUpgradeData, int spawnLevel)
    {
        var maxHealth = baseUpgradeData.buildingBasicStats.maxHealth;
        var armor = baseUpgradeData.buildingBasicStats.armor;
        var buildTime = baseUpgradeData.buildingBuildTime;
        var unitDamage = baseUpgradeData.defenseAttackStats.damage;
        var buildingDamage = baseUpgradeData.defenseAttackStats.buildingDamage;
        var fireRate = baseUpgradeData.defenseAttackStats.fireRate;
        var range = baseUpgradeData.defenseRangeStats.attackRange;

        var detectionRange = baseUpgradeData.defenseRangeStats.detectionRange;
        var minRange = baseUpgradeData.defenseRangeStats.minAttackRange;

        DefenseBuildingUpgradeData defenseBuildingUpgradeData = new()
        {
            buildingLevel = spawnLevel + 1,
            buildingBasicStats = new()
            {
                maxHealth = FloatPercentUpgrade(maxHealth, HealthChangePercent, spawnLevel),
                armor = FloatPercentUpgrade(armor, ArmorChangePercent, spawnLevel)
            },
            buildingBuildTime = FloatPercentUpgrade(buildTime, BuildingBuildTimeChangePercent, spawnLevel, BuildTimeChangeInterval),
            defenseAttackStats = new()
            {
                damage = FloatPercentUpgrade(unitDamage, DamageChangePercent, spawnLevel),
                buildingDamage = FloatPercentUpgrade(buildingDamage, DamageChangePercent, spawnLevel),
                fireRate = FloatPercentUpgrade(fireRate, FireRateChangePercent, spawnLevel, FireRateChangeInterval)
            },
            defenseRangeStats = new()
            {
                attackRange = FloatPercentUpgrade(range, AttackRangeChangePercent, spawnLevel, RangeChangeInterval),
                detectionRange = detectionRange,
                minAttackRange = minRange
            }
        };

        return defenseBuildingUpgradeData;
    }

    private static void GenerateResourceUpgradeData(ResourceBuildingDataSO resourceBuildingSO)
    {
        var resourceBuildingUpgradeData = new List<ResourceBuildingUpgradeData>();
        for (int i = 0; i < MaxLevel; i++)
        {
            resourceBuildingUpgradeData.Add(ResourceBuildingData(resourceBuildingSO.resourceBuildingUpgradeData[0], i));
        }

        resourceBuildingSO.resourceBuildingUpgradeData = resourceBuildingUpgradeData;
    }

    private static ResourceBuildingUpgradeData ResourceBuildingData(ResourceBuildingUpgradeData baseUpgradeData, int spawnLevel)
    {
        var maxHealth = baseUpgradeData.buildingBasicStats.maxHealth;
        var armor = baseUpgradeData.buildingBasicStats.armor;
        var buildTime = baseUpgradeData.buildingBuildTime;
        var resourceGenerationAmount = baseUpgradeData.resourceAmountPerBatch;
        var resourceCapacity = baseUpgradeData.resourceAmountCapacity;

        ResourceBuildingUpgradeData resourceBuildingUpgradeData = new()
        {
            buildingLevel = spawnLevel + 1,
            buildingBasicStats = new()
            {
                maxHealth = FloatPercentUpgrade(maxHealth, HealthChangePercent, spawnLevel),
                armor = FloatPercentUpgrade(armor, ArmorChangePercent, spawnLevel)
            },
            buildingBuildTime = FloatPercentUpgrade(buildTime, BuildingBuildTimeChangePercent, spawnLevel, BuildTimeChangeInterval),
            resourceAmountPerBatch = IntegerPercentUpgrade(resourceGenerationAmount, resourceAmountChangePercent, spawnLevel, resourceAmountInterval),
            resourceAmountCapacity = IntegerPercentUpgrade(resourceCapacity, resourceCapacityChangePercent, spawnLevel, resourceCapacityInterval)
        };

        return resourceBuildingUpgradeData;
    }

    private static float FloatPercentUpgrade(float initialValue, float changePercent, int currentLevel, int intervalChange = -1)
    {
        if (intervalChange > 0)
        {
            int timesFired = (currentLevel + 1) / intervalChange;
            float result = initialValue + initialValue * changePercent * PERCENT * timesFired;
            return CeilToTwoDecimals(result);
        }
        else
        {
            float result = initialValue + initialValue * changePercent * PERCENT * currentLevel;
            return CeilToTwoDecimals(result);
        }
    }

    private static int IntegerStepUpgrade(int initialValue, int stepChange, int currentLevel, int clampValue, int intervalChange = -1)
    {
        if (intervalChange > 0)
        {
            int timesFired = (currentLevel + 1) / intervalChange;
            return Mathf.Clamp(initialValue + stepChange * timesFired, initialValue, clampValue);
        }
        else
        {
            return Mathf.Clamp(initialValue + stepChange * currentLevel, initialValue, clampValue);
        }
    }

    private static int IntegerPercentUpgrade(int initialValue, float changePercent, int currentLevel, int intervalChange = -1)
    {
        if (intervalChange > 0)
        {
            int timesFired = (currentLevel + 1) / intervalChange;
            // we return integer part + 1 if it is not a integer
            return Mathf.CeilToInt(initialValue + initialValue * changePercent * PERCENT * timesFired);
        }
        else
        {
            return Mathf.CeilToInt(initialValue + initialValue * changePercent * PERCENT * currentLevel);
        }

    }

    private static float CeilToTwoDecimals(float value)
    {
        return Mathf.Ceil(value * 100f) / 100f;
    }

#endif

    public static int GetUpgradeCost(int spawnLevel, float multiplier)
    {
        // pass spawn level, looking according to the index of array i.e.(level - 1)
        var minValue = 10;
        var maxValue = 2000;
        var lastValue = maxValue - minValue;
        int divisibleBy = 10;

        float cost = CustomCurve((float)spawnLevel / MaxLevel) * lastValue + minValue;
        cost *= multiplier;
        cost = Mathf.CeilToInt(cost);
        cost -= cost % divisibleBy;

        return (int)cost;
    }

    public static int GetFragmentCost(int spawnLevel, float multiplier)
    {
        // pass spawn level, looking according to the index of array i.e.(level - 1)
        var minValue = 4;
        var maxValue = 80;
        var lastValue = maxValue - minValue;
        int divisibleBy = 2;

        float cost = CustomCurve((float)spawnLevel / MaxLevel) * lastValue + minValue;
        cost *= multiplier;
        cost = Mathf.CeilToInt(cost);
        cost -= cost % divisibleBy;

        return (int)cost;
    }
    public static float CustomCurve(float x)
    {
        x = Mathf.Clamp01(x);
        return x < 0.5f ? 2f * x * x : 4 * x - 2 * x * x - 1;
    }
}
