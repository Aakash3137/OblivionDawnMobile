using System;
using UnityEngine;

public class WallStats : BuildingStats
{
    public Action<float, float> onWallEnableOrDisable;
    public Action onWallDestroyed;
    private WallParent wallParent;

    internal override void Start()
    {
        base.Start();
        wallParent = GetComponentInParent<WallParent>();
    }

    public override void TakeDamage(float amount)
    {
        if (amount <= currentHealth)
            wallParent.DamageWall(amount);
        else
            wallParent.DamageWall(currentHealth);

        base.TakeDamage(amount);
    }
    private void OnEnable()
    {
        if (currentHealth > 0)
            onWallEnableOrDisable?.Invoke(currentHealth, basicStats.maxHealth);
    }
    private void OnDisable()
    {
        if (currentHealth > 0)
            onWallEnableOrDisable?.Invoke(currentHealth, basicStats.maxHealth);
    }
    internal override void OnDestroy()
    {
        onWallDestroyed?.Invoke();
    }
}
