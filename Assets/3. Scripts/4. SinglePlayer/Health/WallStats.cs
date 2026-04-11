using System;
using UnityEngine;

public class WallStats : Stats
{
    public DefenseBuildingDataSO wallStatsSO;
    public DefenseBuildingUpgradeData wallData { get; private set; }


    public Action<float, float> wallHealthEvent;
    private WallParent wallParent;

    internal override void Initialize()
    {
        wallParent = GetComponentInParent<WallParent>();

        if (wallStatsSO == null)
        {
            Debug.Log($"<color=red>Building {name} missing BuildingStats. Assign the script.</color>");
        }

        identity = wallStatsSO.buildingIdentity;
        visuals = wallStatsSO.buildingVisuals;

        wallData = wallStatsSO.defenseBuildingUpgradeData[identity.spawnLevel];

        basicStats = wallData.buildingBasicStats;

        side = GetComponentInParent<BuildingStats>().side;

        if (visuals.playerUnitMaterial == null)
        {
            Debug.Log($"<color=magenta>Assign materials for {name} on {wallStatsSO.name} ScriptableObject</color>");
        }

        base.Initialize();

        if (currentHealth > 0)
            wallHealthEvent?.Invoke(currentHealth, basicStats.maxHealth);
    }

    public override void TakeDamage(float amount, Stats stat)
    {
        base.TakeDamage(amount);

        var damage = Mathf.Max(0, amount - basicStats.armor);

        if (damage <= currentHealth)
            wallParent.DamageWall(damage);
        else
        {
            damage = currentHealth;
            wallParent.DamageWall(damage);
        }
    }

    public override void ResetHealth()
    {
        base.ResetHealth();
        wallHealthEvent?.Invoke(currentHealth, 0f);
    }

    private void OnDisable()
    {
        if (currentHealth > 0)
            wallHealthEvent?.Invoke(-currentHealth, -basicStats.maxHealth);
    }
}
