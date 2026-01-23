using UnityEngine;

public class BuildingStats : Stats
{
    [Header("Building Settings")]
    [field: SerializeField]
    public BuildingUpgradeDataSO buildingStats { get; private set; }
    [field: SerializeField]
    public ScenarioBuildingType buildingType { get; private set; }
    public BuildingUpgradeData buildingData { get; private set; }
    private GameObject buildingPool;
    public Tile currentTile { get; private set; }


    internal override void Start()
    {
        if (buildingStats == null)
        {
            Debug.Log($"<color=red>Building {name} missing BuildingStats. Assign the script.</color>");
        }

        buildingType = buildingStats.buildingType;
        level = buildingStats.buildingSpawnLevel;
        visuals = buildingStats.buildingVisuals;

        buildingData = buildingStats.buildingUpgradeData[level];

        basicStats = buildingData.buildingBasicStats;

        currentTile = GetComponentInParent<Tile>();
        side = currentTile.ownerSide;
        currentTile.SetOccupant(gameObject);

        faction = buildingStats.buildingFaction;
        targetPriority = buildingStats.targetPriority;

        if (visuals.playerUnitMaterial == null)
        {
            Debug.Log($"<color=magenta>Assign materials for {name} on {buildingStats.name} ScriptableObject</color>");
        }

        SetParent();

        base.Start();
    }

    private void SetParent()
    {
        switch (buildingType)
        {
            case ScenarioBuildingType.MainBuilding:
                buildingPool = GameObject.FindWithTag("MainPool");
                if (buildingPool == null)
                    Debug.Log("<color=red>No GameObject with tag 'MainPool' found in scene!</color>");
                break;
            case ScenarioBuildingType.DefenseBuilding:
                buildingPool = GameObject.FindWithTag("DefensePool");
                if (buildingPool == null)
                    Debug.Log("<color=red>No GameObject with tag 'DefensePool' found in scene!</color>");
                break;
            case ScenarioBuildingType.OffenseBuilding:
                buildingPool = GameObject.FindWithTag("OffensePool");
                if (buildingPool == null)
                    Debug.Log("<color=red>No GameObject with tag 'OffensePool' found in scene!</color>");
                break;
            case ScenarioBuildingType.ResourceBuilding:
                buildingPool = GameObject.FindWithTag("ResourcePool");
                if (buildingPool == null)
                    Debug.Log("<color=red>No GameObject with tag 'ResourcePool' found in scene!</color>");
                break;
        }

        transform.parent = buildingPool?.transform;
    }

    // public void UpgradeBuilding()
    // {
    //     //Upgrade building and updating resource generation values
    //     level++;
    //     buildingData = buildingStats.buildingLevelData[level];
    //     maxHealth = buildingData.buildingBasicStats.maxHealth;

    //     visuals = buildingData.buildingVisuals;
    //     basicStats = buildingData.buildingBasicStats;

    //     buildingUpgradeCosts = buildingData.buildingUpgradeCosts;
    // }
    // public bool canUpgrade()
    // {
    //     return PlayerResourceManager.Instance.HasResources(buildingUpgradeCosts);
    // }

    internal override void Die()
    {
        base.Die();

        KillCounterManager.Instance.AddBuildingDestroyedData(buildingType, side);

        if (buildingType == ScenarioBuildingType.MainBuilding)
        {
            switch (side)
            {
                case Side.Player:
                    RTSGameStateManager.Instance.ChangeState(RTSGameState.DEFEAT);
                    break;
                case Side.Enemy:
                    RTSGameStateManager.Instance.ChangeState(RTSGameState.VICTORY);
                    break;
            }
        }
    }
    internal virtual void OnDestroy()
    {
        currentTile.ClearOccupant();
        currentTile.hasBuilding = false;
    }
}
