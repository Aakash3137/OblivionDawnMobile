using UnityEngine;

public class DefenseBuildingStats : BuildingStats
{
    public ScenarioDefenseType defenseType { get; private set; }
    public DefenseBuildingUpgradeData defenseBuildingData { get; private set; }
    public AttackStats attackStats { get; private set; }
    public override TileEffectType compatibleTileEffectType => TileEffectType.DefenseTile;
    DefenseUnit defenseUnit;

    internal override void Initialize()
    {
        identity = buildingStatsSO.buildingIdentity;

        if (buildingStatsSO is DefenseBuildingDataSO defenseWallSO)
        {
            defenseType = defenseWallSO.defenseType;
            defenseBuildingData = defenseWallSO.defenseBuildingUpgradeData[identity.spawnLevel];
            basicStats = defenseBuildingData.buildingBasicStats;
            attackStats = defenseBuildingData.defenseAttackStats;
            buildTime = defenseBuildingData.buildingBuildTime;
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing DefenseBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Initialize();

        defenseUnit = GetComponent<DefenseUnit>();
    }
    internal override void EnableFunctionality()
    {
        base.EnableFunctionality();

        if (defenseUnit != null)
            defenseUnit.enabled = true;
    }
    internal override void DisableFunctionality()
    {
        base.DisableFunctionality();

        if (defenseUnit != null)
            defenseUnit.enabled = false;
    }
    internal override void Die()
    {
        base.Die();

        KillCounterManager.Instance.AddDefenseBuildingDestroyedData(defenseType, side);
    }

    public DefenseBuildingUpgradeData GetBuildingData()
    {
        return defenseBuildingData;
    }
    public DefenseBuildingDataSO GetBuildingSO()
    {
        return buildingStatsSO as DefenseBuildingDataSO;
    }
    public void SetCurrentHealth(float health)
    {
        currentHealth = health;
    }
    public void BuffDamageStats(float buffStrength)
    {
        AttackStats buffedAttackStats = new()
        {
            damage = attackStats.damage * buffStrength,
            buildingDamage = attackStats.buildingDamage * buffStrength,
            fireRate = attackStats.fireRate,
        };

        attackStats = buffedAttackStats;
    }
}
