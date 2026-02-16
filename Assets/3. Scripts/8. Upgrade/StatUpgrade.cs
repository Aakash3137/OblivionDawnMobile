using UnityEngine;
using System;

public static class StatUpgrade
{
    public const float PERCENT_CHANGE = 0.45f;

    // Generic stat calculator
    public static float GetStat(float baseValue, int level, int maxLevel, bool increase)
    {
        if (maxLevel <= 0)
            return baseValue;

        level = Mathf.Clamp(level, 0, maxLevel);

        float t = (float)level / maxLevel;

        return increase
            ? baseValue * (1f + PERCENT_CHANGE * t)
            : baseValue * (1f - PERCENT_CHANGE * t);
    }

    // ===============================
    // COST FORMULA (QUADRATIC)
    // ===============================
    // Cost(n) = 10n² + 10n + 20
    public static int UpgradeCost(int level)
    {
        return 10 * level * level + 10 * level + 20;
    }

    public static float GetDiff(float BaseValue, float NextValue)
    {
        return NextValue - BaseValue;
    }

    // ===============================
    // SPECIFIC STATS (READABLE)
    // ===============================

    public static float BuildTime(float baseValue, int level, int maxLevel)
        => GetStat(baseValue, level, maxLevel, false);

    public static float MaxHealth(float baseValue, int level, int maxLevel)
        => GetStat(baseValue, level, maxLevel, true);

    public static float Armour(float baseValue, int level, int maxLevel)
        => GetStat(baseValue, level, maxLevel, true);

    public static float Damage(float baseValue, int level, int maxLevel)
        => GetStat(baseValue, level, maxLevel, true);

    // If FireRate = cooldown → decrease
    public static float FireRateCooldown(float baseValue, int level, int maxLevel)
        => GetStat(baseValue, level, maxLevel, false);

    // If FireRate = shots per second → increase
    public static float FireRate(float baseValue, int level, int maxLevel)
        => GetStat(baseValue, level, maxLevel, true);

    public static float MoveSpeed(float baseValue, int level, int maxLevel)
        => GetStat(baseValue, level, maxLevel, true);

    public static float DirectionRange(float baseValue, int level, int maxLevel)
        => GetStat(baseValue, level, maxLevel, true);

    public static float AttackRange(float baseValue, int level, int maxLevel)
        => GetStat(baseValue, level, maxLevel, true);
    public static int Resource(int baseValue, int level, int maxLevel)
   => (int)GetStat(baseValue, level, maxLevel, true);
    public static int Capacity(int baseValue, int level, int maxLevel)
    => (int)GetStat(baseValue, level, maxLevel, true);
}


public class UnitProduceUpgrade
{
    public int maxLevel = 20;
    public void UpgradeNext(UnitProduceStatsSO unit, DecManager _Dec = null)
    {
        int next = unit.unitUpgradeData.Length;

        if (next > maxLevel)
            return;

        var Last = unit.unitUpgradeData[next - 1];

        var cur = new UnitUpgradeData();
        cur.unitLevel = next;

        ApplyFormula(Last, ref cur);
        Array.Resize(ref unit.unitUpgradeData, next + 1);
        unit.unitUpgradeData[next] = cur;
        unit.unitIdentity.spawnLevel = next;

        if (_Dec != null)
            _Dec.diamondtext.text = (_Dec._Profile.Diamonds -= StatUpgrade.UpgradeCost(next)).ToString();

    }

    void ApplyFormula(UnitUpgradeData prev, ref UnitUpgradeData cur)
    {
        cur.unitBasicStats.maxHealth = StatUpgrade.MaxHealth(prev.unitBasicStats.maxHealth, cur.unitLevel, maxLevel);
        cur.unitBasicStats.armor = StatUpgrade.Armour(prev.unitBasicStats.armor, cur.unitLevel, maxLevel);
        cur.unitAttackStats.damage = StatUpgrade.Damage(prev.unitAttackStats.damage, cur.unitLevel, maxLevel);
        cur.unitAttackStats.fireRate = StatUpgrade.FireRate(prev.unitAttackStats.fireRate, cur.unitLevel, maxLevel);
        cur.unitMobilityStats.moveSpeed = StatUpgrade.MoveSpeed(prev.unitMobilityStats.moveSpeed, cur.unitLevel, maxLevel);
        cur.unitRangeStats.attackRange = StatUpgrade.AttackRange(prev.unitRangeStats.attackRange, cur.unitLevel, maxLevel);
        cur.unitBuildTime = StatUpgrade.BuildTime(prev.unitBuildTime, cur.unitLevel, maxLevel);
    }
}
