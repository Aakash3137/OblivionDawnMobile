using UnityEngine;

[RequireComponent(typeof(BasicStatsComponent), typeof(IdentityComponent), typeof(VisualsComponent)), RequireComponent(typeof(LayerAssignComponent))]
public class BuildingBlueprint : MonoBehaviour
{
    public ScenarioBuildingType buildingType;
    public Tile currentTile { get; private set; }
    internal BasicStatsComponent basicStatsComponent;
    internal IdentityComponent identityComponent;
    internal VisualsComponent visualsComponent;
    internal LayerAssignComponent layerAssignComponent;
    internal SetParentComponent setParentComponent;
    internal int spawnLevel;

    public BuildCost[] buildCost { get; internal set; }

    #region  Temporary
    [Header("Temp")]
    public BuildingDataSO dataSO;

    private void Start()
    {
        currentTile = GetComponentInParent<Tile>();
        Initialize(dataSO, currentTile);
    }
    #endregion

    // Initialize this when spawning building
    internal virtual void Initialize(BuildingDataSO buildingData, Tile tile)
    {
        AssignComponents();

        currentTile = tile;
        currentTile.SetOccupant(gameObject);

        buildCost = buildingData.buildingBuildCost;

        identityComponent.Initialize(buildingData.buildingIdentity, tile.ownerSide);
        spawnLevel = identityComponent.identity.spawnLevel;
        visualsComponent.Initialize(buildingData.buildingVisuals);
        layerAssignComponent.Initialize(identityComponent.side);
        setParentComponent.Initialize();
    }

    internal virtual void AssignComponents()
    {
        basicStatsComponent = TryGetComponent<BasicStatsComponent>(out var basicStats) ? basicStats : null;
        identityComponent = TryGetComponent<IdentityComponent>(out var identity) ? identity : null;
        visualsComponent = TryGetComponent<VisualsComponent>(out var visuals) ? visuals : null;
        layerAssignComponent = TryGetComponent<LayerAssignComponent>(out var layerAssign) ? layerAssign : null;
        setParentComponent = TryGetComponent<SetParentComponent>(out var setParent) ? setParent : null;
    }

    internal virtual void HandleKillData()
    {
        KillCounterManager.Instance.AddBuildingDestroyedData(buildingType, identityComponent.side);

        if (buildingType == ScenarioBuildingType.MainBuilding)
        {
            switch (identityComponent.side)
            {
                case Side.Player:
                    RTSGameStateManager.Instance.ChangeState(RTSGameState.DEFEAT);
                    break;
                case Side.Enemy:
                    RTSGameStateManager.Instance.ChangeState(RTSGameState.VICTORY);
                    break;
            }
        }

        currentTile.ClearOccupant();
        currentTile.hasBuilding = false;
    }

    internal virtual void EventsHandler()
    {
        if (basicStatsComponent != null)
            basicStatsComponent.onDeath += HandleKillData;
    }

    internal virtual void OnDisable()
    {
        if (basicStatsComponent != null)
            basicStatsComponent.onDeath -= HandleKillData;
    }
}
