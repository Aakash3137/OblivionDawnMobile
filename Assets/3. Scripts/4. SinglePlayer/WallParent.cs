using System.Collections.Generic;
using UnityEngine;

public class WallParent : MonoBehaviour
{
    [SerializeField] private WallStats[] wallStats;
    public float WallMaxHealth;
    public float wallCurrentHealth;
    private HealthProgress healthBar;
    //public UpgradeCost[] wallParentUpgradeCosts { get; private set; }

    private void Awake()
    {
        wallStats = GetComponentsInChildren<WallStats>();
        healthBar = GetComponentInChildren<HealthProgress>();
    }
    private void Start()
    {
        foreach (var wall in wallStats)
        {
            if (!wall.gameObject.activeSelf)
                continue;

            WallMaxHealth += wall.basicStats.maxHealth;

            // for (int i = 0; i < wall.defenseUpgradeCosts.Length; i++)
            // {
            //     wallParentUpgradeCosts[i].resourceCost += wall.defenseUpgradeCosts[i].resourceCost;
            // }
        }

        wallCurrentHealth = WallMaxHealth;
    }

    public void DamageWall(float amount)
    {
        wallCurrentHealth -= amount;

        wallCurrentHealth = Mathf.Clamp(wallCurrentHealth, 0, WallMaxHealth);
        if (healthBar != null)
        {
            healthBar.FadeInHealthBar();
            healthBar.UpdateFillAmount(wallCurrentHealth / WallMaxHealth);
        }
    }

    public void EnableWall(int index)
    {
        wallStats[index].gameObject.SetActive(true);
    }

    public void DisableWall(int index)
    {
        wallStats[index].gameObject.SetActive(false);
    }
}
