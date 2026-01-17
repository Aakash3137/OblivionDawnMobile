using UnityEngine;

public class WallParent : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private WallStats[] wallStats;
    public float WallMaxHealth;
    public float wallCurrentHealth;
    private HealthProgress healthBar;

    private void OnEnable()
    {
        foreach (var wall in wallStats)
        {
            wall.onWallEnableOrDisable += HandleWallHealth;
            wall.onWallDestroyed += HandleWallDestroy;
        }
    }

    private void OnDisable()
    {
        foreach (var wall in wallStats)
        {
            wall.onWallEnableOrDisable -= HandleWallHealth;
            wall.onWallDestroyed -= HandleWallDestroy;
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

        await Awaitable.WaitForSecondsAsync(1f);
        if (WallMaxHealth <= 0)
        {
            Destroy(gameObject);
        }
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
            healthBar.isVisible = true;
            healthBar.UpdateFillAmount(wallCurrentHealth / WallMaxHealth);
        }
    }

    private void HandleWallDestroy()
    {
        if (wallCurrentHealth <= 0)
        {
            Destroy(gameObject);
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

    private void OnDestroy()
    {
        DefenseWallStats defenseWall = GetComponentInParent<DefenseWallStats>();

        if (defenseWall != null)
        {
            Destroy(defenseWall.gameObject);
        }
    }
}
