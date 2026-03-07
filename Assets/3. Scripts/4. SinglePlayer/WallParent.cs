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

    private void OnEnable()
    {
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

    // private async Awaitable Start()
    // {
    //     await Awaitable.WaitForSecondsAsync(0.1f);

    //     foreach (var wall in wallStats)
    //     {
    //         wallMaxHealth += wall.basicStats.maxHealth;
    //     }

    //     wallCurrentHealth = wallMaxHealth;

    //     await Awaitable.WaitForSecondsAsync(1f);
    //     if (wallMaxHealth <= 0)
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    private void HandleWallHealth(float health, float maxHealth)
    {
        wallCurrentHealth += health;
        wallMaxHealth += maxHealth;
    }

    public void DamageWall(float amount)
    {
        wallCurrentHealth -= amount;

        wallCurrentHealth = Mathf.Clamp(wallCurrentHealth, 0, wallMaxHealth);

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
        }
    }

    public void EnableWall(int index)
    {
        if (wallStats[index] != null)
            wallStats[index].gameObject.SetActive(true);
    }

    public void DisableWall(int index)
    {
        /*if (wallStats[index] != null)
            wallStats[index].gameObject.SetActive(false);*/
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
