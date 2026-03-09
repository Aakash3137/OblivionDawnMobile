using Sirenix.OdinInspector;
using UnityEngine;

public class WallParent : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private WallStats[] wallStats;
    [ReadOnly]
    public float wallMaxHealth;
    [ReadOnly]
    public float wallCurrentHealth;
    private HealthProgress healthBar;
    DefenseBuildingStats defenseWall;

    private void OnEnable()
    {
        defenseWall = GetComponentInParent<DefenseBuildingStats>();

        foreach (var wall in wallStats)
        {
            wall.wallHealthEvent += HandleWallHealth;
            wall.onDieEvent += HandleWallDestroy;
        }

        foreach (var wall in wallStats)
        {
            wall.Initialize();
        }
    }

    private void OnDisable()
    {
        foreach (var wall in wallStats)
        {
            wall.wallHealthEvent -= HandleWallHealth;
            wall.onDieEvent -= HandleWallDestroy;
        }
    }

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthProgress>();
    }

    private void HandleWallHealth(float health, float maxHealth)
    {
        wallCurrentHealth += health;
        wallMaxHealth += maxHealth;

        if (defenseWall != null && defenseWall.defenseType == ScenarioDefenseType.Wall)
        {
            defenseWall.SetCurrentHealth(wallCurrentHealth);
        }
    }

    public void DamageWall(float amount)
    {
        wallCurrentHealth -= amount;

        wallCurrentHealth = Mathf.Clamp(wallCurrentHealth, 0, wallMaxHealth);

        if (defenseWall != null && defenseWall.defenseType == ScenarioDefenseType.Wall)
        {
            defenseWall.SetCurrentHealth(wallCurrentHealth);
        }

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar();
            healthBar.isVisible = true;
            healthBar.UpdateFillAmount(wallCurrentHealth / wallMaxHealth);
        }
    }

    private void HandleWallDestroy()
    {
        if (wallCurrentHealth <= 0)
        {
            Destroy(gameObject);

            if (defenseWall != null && defenseWall.defenseType == ScenarioDefenseType.Wall)
            {
                defenseWall.Die();
            }
        }
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
}
