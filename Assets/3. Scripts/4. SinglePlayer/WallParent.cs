using System.Collections.Generic;
using UnityEngine;

public class WallParent : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private WallStats[] wallStats;
    public float WallMaxHealth;
    public float wallCurrentHealth;
    private HealthProgress healthBar;
    //public UpgradeCost[] wallParentUpgradeCosts { get; private set; }

    private void OnEnable()
    {
        foreach (var wall in wallStats)
        {
            wall.onWallEnableOrDisable += HandleWallHealth;
        }
    }

    private void OnDisable()
    {
        foreach (var wall in wallStats)
        {
            wall.onWallEnableOrDisable -= HandleWallHealth;
        }
    }

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthProgress>();
    }
    private async Awaitable Start()
    {
        await Awaitable.WaitForSecondsAsync(0.1f);
        foreach (var wall in wallStats)
        {
            WallMaxHealth += wall.basicStats.maxHealth;
        }

        wallCurrentHealth = WallMaxHealth;
    }

    private void HandleWallHealth(float health, float maxHealth)
    {
        wallCurrentHealth -= health;
        WallMaxHealth -= maxHealth;
    }

    public void DamageWall(float amount)
    {
        wallCurrentHealth -= amount;

        wallCurrentHealth = Mathf.Clamp(wallCurrentHealth, 0, WallMaxHealth);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar();
            healthBar.UpdateFillAmount(wallCurrentHealth / WallMaxHealth);
        }

        if (wallCurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void HandleWallDestroy()
    {

    }

    public void EnableWall(int index)
    {
        if (wallStats[index] != null)
            wallStats[index].gameObject.SetActive(true);
    }

    public void DisableWall(int index)
    {
        if (wallStats[index] != null)
            wallStats[index].gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (GetComponentInParent<WallStats>() != null)
        {
            Tile currentTile = GetComponentInParent<Tile>();

            if (currentTile != null)
                currentTile.hasBuilding = false;
        }
    }
}
