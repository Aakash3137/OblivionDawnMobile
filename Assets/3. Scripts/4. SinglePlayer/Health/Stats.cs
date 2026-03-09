using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Stats : MonoBehaviour
{

    [field: SerializeField, ReadOnly]
    public Identity identity { get; protected set; }

    [field: SerializeField, ReadOnly]
    public BasicStats basicStats { get; protected set; }

    [field: SerializeField, ReadOnly]
    public float currentHealth { get; protected set; }

    [field: SerializeField, ReadOnly]
    public Side side { get; protected set; }

    public Visuals visuals { get; protected set; }

    public Collider hitCollider { get; protected set; }
    public BuildCost[] buildCost { get; protected set; }

    [Header("Fade Health Bar is OLD UI in world space. Health Progress is NEW UI on world Canvas")]
    private FadeHealthBar healthBarFade;
    private HealthProgress healthBar;

    public AirUnit airUnit { get; private set; }
    public virtual bool CanFly => false;

    public Action onDieEvent;


    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthProgress>();
        healthBarFade = GetComponentInChildren<FadeHealthBar>();
        hitCollider = GetComponent<Collider>();

        TryGetComponent<AirUnit>(out var airUnit);
        this.airUnit = airUnit;

        // else
        //Debug.Log($"<color=#FFC0CB>{name} missing HealthBar. Assign the script.</color>");

    }
    internal virtual void Initialize()
    {
        currentHealth = basicStats.maxHealth;

        if (healthBar != null)
            healthBar.UpdateFillAmount(currentHealth / basicStats.maxHealth);

        ApplyMaterial();
    }

    private void ApplyMaterial()
    {
        BuildingSkeleton buildingSkeleton = GetComponent<BuildingSkeleton>();
        Renderer renderer = null;

        if (buildingSkeleton != null)
            renderer = buildingSkeleton.graphicObject.GetComponentInChildren<Renderer>();
        else
            renderer = GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            switch (side)
            {
                case Side.Player:

                    if (CanFly)
                        gameObject.layer = LayerMask.NameToLayer("PlayerAir");
                    else
                        gameObject.layer = LayerMask.NameToLayer("PlayerGround");
                    if (visuals.playerUnitMaterial != null)
                        renderer.sharedMaterial = visuals.playerUnitMaterial;
                    break;
                case Side.Enemy:

                    if (CanFly)
                        gameObject.layer = LayerMask.NameToLayer("EnemyAir");
                    else
                        gameObject.layer = LayerMask.NameToLayer("EnemyGround");

                    if (visuals.enemyUnitMaterial != null)
                        renderer.sharedMaterial = visuals.enemyUnitMaterial;
                    break;
            }
        }
    }

    public virtual void TakeDamage(float amount, Stats attacker = null)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, basicStats.maxHealth);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar();
            healthBar.isVisible = true;
            healthBar.UpdateFillAmount(currentHealth / basicStats.maxHealth);
        }

        if (healthBarFade != null)
        {
            healthBarFade.ShowOnHit();
            healthBarFade.Isvisible = true;
        }

        if (attacker != null)
        {
            GroundUnit groundUnit = GetComponent<GroundUnit>();
            if (groundUnit != null)
            {
                groundUnit.SetReplyTarget(attacker);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    internal virtual void Die()
    {
        Destroy(gameObject);
        onDieEvent?.Invoke();
        if (TryGetComponent<GemSpawner>(out var gemSpawner) && side == Side.Enemy)
            gemSpawner.SpawnGem();
    }

    [Button]
    public virtual void ResetHealth()
    {
        currentHealth = basicStats.maxHealth;

        if (healthBar != null)
            healthBar.UpdateFillAmount(currentHealth / basicStats.maxHealth);
    }

    [Button]
    public void DealDamage(float amount = 50f)
    {
        TakeDamage(amount);
    }

    [Button]
    public virtual void Kill()
    {
        TakeDamage(currentHealth);
    }
}
