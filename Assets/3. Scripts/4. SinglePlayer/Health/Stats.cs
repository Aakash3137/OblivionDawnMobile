using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class Stats : MonoBehaviour
{

    [field: SerializeField, ReadOnly]
    public Identity identity { get; protected set; }

    [field: SerializeField, ReadOnly]
    public BasicStats basicStats { get; protected set; }

    [SerializeField] private float CurrentHealth;

    public float currentHealth
    {
        get => CurrentHealth;
        set
        {
            CurrentHealth = value;
            EnableRepairButton();
        }
    }
    public RepairButtonHandler RepairObj;

    [field: SerializeField, ReadOnly]
    public Side side { get; protected set; }

    public Visuals visuals { get; protected set; }

    public Collider hitCollider { get; protected set; }
    public BuildCost[] buildCost { get; protected set; }

    private bool isDamageImmune = false;

    [Header("Fade Health Bar is OLD UI in world space. Health Progress is NEW UI on world Canvas")]
    private FadeHealthBar healthBarFade;
    private HealthProgress healthBar;

    public AirUnit airUnit { get; private set; }
    public virtual bool CanFly => false;

    public Action onDieEvent;
    private float TempCurrentHealth = 0;


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
        TempCurrentHealth = currentHealth = basicStats.maxHealth;
        //        Debug.Log($"<color=green> <size=16>Initial Health: {basicStats.maxHealth} </size></color>");

        if (healthBar != null)
            healthBar.UpdateFillAmount(currentHealth / basicStats.maxHealth);

        ApplyMaterial();
    }

    private void EnableRepairButton()
    {
        if (RepairObj == null)
            return;

        if (!RepairObj.IsReady && side != Side.Player)
            return;

        if (currentHealth < basicStats.maxHealth / 2)
        {
           //callRepair();
            // RepairObj.PlayShow();
        }
    }

    void callRepair()
    {
        BuildingSkeleton building = GetComponent<BuildingSkeleton>();
            if (building != null)
                RepairManager.Instance.OnClickRepairBtnOpen(building.RepairEffectPlace, building.RepairEffectPlace, this, true);
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
        if (isDamageImmune)
        {
            Debug.Log($"[Stats] {gameObject.name} is immune to damage");
            return;
        }

        amount = Mathf.Max(0, amount - basicStats.armor);

       // if (amount == 0)
            //Debug.Log("<size=16> Armor is high cannot take damage</size>");

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, basicStats.maxHealth);
        // Debug.Log("Current Health: " + currentHealth+ " Who is "+identity.name);

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
            DefenseUnit defenseUnit = GetComponent<DefenseUnit>();

            if (groundUnit != null)
            {
                groundUnit.SetReplyTarget(attacker);
            }
            if (defenseUnit != null)
            {
                defenseUnit.SetReplyTarget(attacker);
            }
        }
        // Debug.Log("Object => " + RepairObj + "On "+ gameObject.name);
        if (currentHealth <= 0)
        {
            Die();
        }
        else if (currentHealth <= TempCurrentHealth / 2 && currentHealth > 0 && side == Side.Player)
        {
            if (RepairObj == null)
                return;

            if (!RepairObj.IsReady)
                return;

            // callRepair();
            // Debug.Log($"<color=red>Your Health is too low {currentHealth} Repair Health Now</color>");
        }
    }

    internal virtual void Die()
    {
        Destroy(gameObject);
        onDieEvent?.Invoke();
        /*if (TryGetComponent<GemSpawner>(out var gemSpawner) && side == Side.Enemy)
            gemSpawner.SpawnGem();*/
    }

    public void BuffBasicStats(float buffStrength)
    {
        Debug.Log($"Applying Health Buff currentMaxHealth: {currentHealth}, maxHealth: {basicStats.maxHealth}, buffPercent: {buffStrength}");
        BasicStats buffedBasicStats = new()
        {
            maxHealth = basicStats.maxHealth * buffStrength,
            armor = basicStats.armor
        };
        basicStats = buffedBasicStats;
        currentHealth = basicStats.maxHealth;
        Debug.Log($"Applied Health Buff currentMaxHealth: {currentHealth}, maxHealth: {basicStats.maxHealth}");
    }

    [Button]
    public virtual void ResetHealth()
    {
        Debug.Log("Reset Health");
        currentHealth = basicStats.maxHealth;
        Debug.Log("Reset Health: " + currentHealth);
        if (healthBar != null)
            healthBar.UpdateFillAmount(currentHealth / basicStats.maxHealth);
    }
    #region Repair
    [Button]
    public void HealthRepair()
    {
        float missingHealthRatio = (basicStats.maxHealth - currentHealth) / basicStats.maxHealth;

        if (this is BuildingStats building)
        {
            var buildCost = building.buildingStatsSO.buildingBuildCost;

            var repairCost = new BuildCost[buildCost.Length];

            for (int i = 0; i < buildCost.Length; i++)
            {
                repairCost[i] = new()
                {
                    resourceType = buildCost[i].resourceType,
                    resourceAmount = Mathf.CeilToInt(buildCost[i].resourceAmount * missingHealthRatio)
                };
            }

            // if (!building.rmInstance.HasResources(repairCost))
            //     return;

            building.rmInstance.SpendResources(repairCost);
        }

        ResetHealth();
    }
    #endregion
    [Button]
    public void DealDamage(float amount = 50f)
    {
        TakeDamage(amount);
    }

    [Button]
    public virtual void Kill()
    {
        TakeDamage(currentHealth + basicStats.armor);
    }
    [Button]
    public void SetDamageImmunity(bool immune)
    {
        isDamageImmune = immune;
    }
}
