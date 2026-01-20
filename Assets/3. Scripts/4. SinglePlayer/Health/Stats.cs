using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header(" EDITOR VIEW ONLY ")]
    public int level;
    public Side side;
    public FactionName faction;
    public BasicStats basicStats;
    public Visuals visuals;
    public float currentHealth;
    public Collider hitCollider;

    [Header("Fade Health Bar is OLD UI in world space. Health Progress is NEW UI on world Canvas")]
    private FadeHealthBar healthBarFade;
    private HealthProgress healthBar;

    public AirUnit airUnit { get; private set; }

    [Header("Bools")]
    public virtual bool CanFly => false;
    //keep can Fly, canAttackAir and canAttackGround can be removed
    public bool canAttackAir = false;
    public bool canAttackGround = true;

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthProgress>();
        healthBarFade = GetComponentInChildren<FadeHealthBar>();
        hitCollider = GetComponent<Collider>();
        TryGetComponent<AirUnit>(out var airUnit);
        this.airUnit = airUnit;

        if (healthBar != null)
            healthBar.UpdateFillAmount(currentHealth / basicStats.maxHealth);
        // else
        //Debug.Log($"<color=#FFC0CB>{name} missing HealthBar. Assign the script.</color>");

    }
    internal virtual void Start()
    {
        currentHealth = basicStats.maxHealth;

        Renderer renderer = GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            switch (side)
            {
                case Side.Player:

                    if (CanFly)
                        gameObject.layer = LayerMask.NameToLayer("PlayerAir");
                    else
                        gameObject.layer = LayerMask.NameToLayer("PlayerGround");

                    renderer.material = visuals.playerUnitMaterial;
                    break;
                case Side.Enemy:

                    if (CanFly)
                        gameObject.layer = LayerMask.NameToLayer("EnemyAir");
                    else
                        gameObject.layer = LayerMask.NameToLayer("EnemyGround");

                    renderer.material = visuals.enemyUnitMaterial;
                    break;
            }
        }
    }

    public virtual void TakeDamage(float amount)
    {
        // if (canFly && airUnit != null && !airUnit.CanBeTargeted())
        //     return;

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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    internal virtual void Die()
    {
        Destroy(gameObject);
    }
}
