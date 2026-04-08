using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class UnitStats : Stats
{
    [Header("Assign Unit Produce Stats")]
    public UnitProduceStatsSO unitProduceSO;

    public ScenarioUnitType unitType { get; private set; }
    private GameObject unitPool;

    public VisionAngles unitVisionAngles { get; private set; }
    public AttackTargets unitAttackTargets { get; private set; }

    [SerializeField, ReadOnly]
    private bool canFly;
    [ShowIf(nameof(canFly)), ReadOnly]
    public FlyStats unitFlyStats;
    public override bool CanFly => canFly;
    
    private AbilityController abilityController;
    [field: SerializeField, ReadOnly]
    public GameUnitName gameUnitName { get; private set; }
    
    
    public BuildCost[] unitUpkeepCost;
    public OffenseBuildingStats spawnerBuilding { private get; set; }
    public UnitUpgradeData unitData { get; private set; }

    private ResourceManager rmInstance;
    private bool hasUpkeep;
    

    internal override void Initialize()
    {
        gameUnitName = unitProduceSO.gameUnitName;
        
        identity = unitProduceSO.unitIdentity;
        unitData = unitProduceSO.unitUpgradeData[identity.spawnLevel];
        basicStats = unitData.unitBasicStats;

        visuals = unitProduceSO.unitVisuals;
        canFly = unitProduceSO.canFly;

        unitUpkeepCost = unitProduceSO.upKeepCost;
        hasUpkeep = unitProduceSO.hasUpkeep;

        side = spawnerBuilding.side;

        unitVisionAngles = unitProduceSO.unitVisionAngles;
        unitAttackTargets = unitProduceSO.unitAttackTargets;

        unitType = unitProduceSO.unitType;
        
        abilityController = GetComponent<AbilityController>();

        if (abilityController != null)
        {
           // Debug.Log($"[UnitStats] {gameObject.name} - Found AbilityController, initializing with {(unitProduceSO.abilities != null ? unitProduceSO.abilities.Count : 0)} abilities");
            abilityController.Initialize(unitProduceSO.abilities);
        }
        else
        {
           // Debug.LogWarning($"[UnitStats] {gameObject.name} - No AbilityController component found!");
        }

// assign name
        gameUnitName = unitProduceSO.gameUnitName;
        
        if (canFly)
            unitFlyStats = unitProduceSO.unitFlyStats;

        if (visuals.playerUnitMaterial == null)
        {
            Debug.Log($"<color=magenta>Assign materials for {name} on {unitProduceSO.name} ScriptableObject</color>");
        }

        base.Initialize();
        AbilityManager.Instance?.OnUnitSpawned(this);

        unitPool = GameObject.FindWithTag("UnitPool");

        if (unitPool == null)
            Debug.Log("<color=green>No GameObject with tag 'UnitPool' found in scene!</color>");
        else
            transform.parent = unitPool.transform;

        switch (side)
        {
            case Side.Player:
                rmInstance = PlayerResourceManager.Instance;
                break;
            case Side.Enemy:
                rmInstance = EnemyResourceManager.Instance;
                break;
            default:
                Debug.Log("<color=green>No Resource Manager found</color>");
                break;
        }

        if (hasUpkeep)
            InitializeUnitUpkeep();
        

    }

    public void FireWeapon()
    {

    }

    public void Fly()
    {

    }

    private void InitializeUnitUpkeep() => StartCoroutine(UnitUpkeepHandler());

    private IEnumerator UnitUpkeepHandler()
    {
        var upKeepTime = new WaitForSeconds(unitProduceSO.upKeepTime);
        DecreaseGenerationRate();

        while (gameObject.activeInHierarchy)
        {
            if (CanMaintain())
                rmInstance.SpendResources(unitUpkeepCost);
            else
                TakeDamage(10f);

            yield return upKeepTime;
        }
    }
    private bool CanMaintain()
    {
        return rmInstance.HasResources(unitUpkeepCost);
    }
    private void IncreaseGenerationRate()
    {
        if (rmInstance != null)
            rmInstance.IncreaseResourceGenerationRate(unitUpkeepCost);
    }

    private void DecreaseGenerationRate()
    {
        if (rmInstance != null)
            rmInstance.DecreaseResourceGenerationRate(unitUpkeepCost);
    }

    internal override void Die()
    {
        base.Die();
        AbilityManager.Instance?.OnUnitDied(this);
        
        spawnerBuilding.producedUnits.Remove(this);
        KillCounterManager.Instance.AddUnitKillData(unitType, side);
    }

    private void OnDestroy()
    {
        if (hasUpkeep)
            IncreaseGenerationRate();
    }
}