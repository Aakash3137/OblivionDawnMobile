using UnityEngine;

[RequireComponent(typeof(BasicStatsComponent), typeof(IdentityComponent), typeof(VisualsComponent)), RequireComponent(typeof(AttackComponent)), RequireComponent(typeof(LayerAssignComponent))]
public class MilitaryUnit : MonoBehaviour
{
    private Animator animator;
    public ScenarioUnitType unitType;
    public int unitPopulationCost;
    public UnitSpawnerScenario spawnerBuilding;

    public bool canFly;
    private BasicStatsComponent basicStatsComponent;
    private IdentityComponent identityComponent;
    private VisualsComponent visualsComponent;
    private AttackComponent attackComponent;
    private LayerAssignComponent layerAssignComponent;
    private SetParentComponent setParentComponent;

    // Initialize when the unit is spawned from spawner building
    internal void InitializeUnit(UnitProduceStatsSO unit, UnitSpawnerScenario spawner)
    {
        AssignComponents();

        spawnerBuilding = spawner;

        unitType = unit.unitType;
        unitPopulationCost = unit.unitPopulationCost;
        canFly = unit.canFly;

        identityComponent.Initialize(unit.unitIdentity, identityComponent.side);
        int spawnLevel = identityComponent.identity.spawnLevel;
        basicStatsComponent.Initialize(unit.unitUpgradeData[spawnLevel].unitBasicStats);

        visualsComponent.Initialize(unit.unitVisuals);

        attackComponent.Initialize(unit.unitUpgradeData[spawnLevel].unitAttackStats, unit.unitUpgradeData[spawnLevel].unitRangeStats);
        layerAssignComponent.Initialize(identityComponent.side);

        setParentComponent.Initialize();
    }

    private void AssignComponents()
    {
        basicStatsComponent = TryGetComponent<BasicStatsComponent>(out var basicStats) ? basicStats : null;
        identityComponent = TryGetComponent<IdentityComponent>(out var identity) ? identity : null;
        visualsComponent = TryGetComponent<VisualsComponent>(out var visuals) ? visuals : null;
        layerAssignComponent = TryGetComponent<LayerAssignComponent>(out var layerAssign) ? layerAssign : null;
        setParentComponent = TryGetComponent<SetParentComponent>(out var setParent) ? setParent : null;
        attackComponent = TryGetComponent<AttackComponent>(out var attack) ? attack : null;
    }

    private void HandleKillData()
    {
        KillCounterManager.Instance.AddUnitKillData(unitType, identityComponent.side);
    }

    private void OnEnable()
    {
        if (basicStatsComponent != null)
            basicStatsComponent.onDeath += HandleKillData;
        else
            Debug.Log("BasicStatsComponent is null");
    }

    private void OnDisable()
    {
        if (basicStatsComponent != null)
            basicStatsComponent.onDeath -= HandleKillData;
    }
}
