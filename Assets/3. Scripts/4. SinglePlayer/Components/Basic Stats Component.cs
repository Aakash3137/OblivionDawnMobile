using System;
using UnityEngine;

public class BasicStatsComponent : MonoBehaviour
{
    [field: SerializeField]
    public BasicStats basicStats;
    public Collider hitCollider;
    [field: SerializeField]
    public bool isDead { get; private set; }

    public float currentHealth { get; internal set; }
    public Action<float> onHealthChange;
    public Action onDeath;
    public Action onUniqueUnitDied;

    private HealthProgress healthBar;
    private IdentityComponent identityComponent;

    public void Initialize(BasicStats stats)
    {
        basicStats = stats;
        currentHealth = basicStats.maxHealth;

        healthBar = GetComponentInChildren<HealthProgress>();
        hitCollider = GetComponent<Collider>();
        identityComponent = GetComponent<IdentityComponent>();
    }

    public void TakeDamage(float damage)
    {
        damage = Mathf.Max(0, damage - basicStats.armor);

        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar();
            healthBar.isVisible = true;
            healthBar.UpdateFillAmount(currentHealth / basicStats.maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        onHealthChange?.Invoke(currentHealth);
    }

    private void Die()
    {
        isDead = true;

        onDeath?.Invoke();

        if (identityComponent != null && identityComponent.identity.isUnique == true)
            onUniqueUnitDied?.Invoke();

        // Implement return to pool
        // Destroy(gameObject);
    }
}
