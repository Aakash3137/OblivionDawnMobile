using System;
using UnityEngine;

public class WallStats : Stats
{
    public WallUpgradeDataSO wallStatsSO;
    public WallBuildingUpgradeData wallData { get; private set; }


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

        wallData = wallStatsSO.wallBuildingUpgradeData[identity.spawnLevel];

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
        if (amount <= currentHealth)
            wallParent.DamageWall(amount);
        else
            wallParent.DamageWall(currentHealth);

        base.TakeDamage(amount);
    }

    private void OnDisable()
    {
        if (currentHealth >= 0)
            wallHealthEvent?.Invoke(-currentHealth, -basicStats.maxHealth);
    }
}
