using System;

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
        wallParent.DamageWall(amount);
        base.TakeDamage(amount);
    }
    private void OnEnable()
    {
        onWallEnableOrDisable?.Invoke(currentHealth, basicStats.maxHealth);
    }
    private void OnDisable()
    {
        onWallEnableOrDisable?.Invoke(currentHealth, basicStats.maxHealth);
    }
    internal override void OnDestroy()
    {
        onWallDestroyed?.Invoke();
    }
}
