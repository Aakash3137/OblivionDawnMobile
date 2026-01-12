using UnityEngine;
using System.Collections;

public class UnitSpawnerScenario : MonoBehaviour
{
    [Header("Production stats variable")]
    public UnitProduceStatsSO unitProduceStats;
    private Vector3 spawnPoint;

    private PlayerResourceManager prmInstance;

    [Header("Building Faction and stats")]
    public SideScenario buildingSide { get; private set; }
    private BuildingStats buildingStats;
    public bool autoProduce;

    [Header("Tile/Cube variables")]
    private Tile currentTile;
    private Tile nearestTile;
    private CubeGridManager cgmInstance;

    [Header("Unit Pool")]
    private Transform unitPool;

    [Header("Unit Production variables")]
    public GameObject unitPrefab { get; private set; }
    public int unitSpawnLevel { get; private set; }
    public UnitUpgradeData currentUnitLevelData { get; private set; }
    public float unitBuildTime { get; private set; }
    public UpgradeCost[] unitUpgradeCosts { get; private set; }

    private void Awake()
    {
        prmInstance = PlayerResourceManager.Instance;
        cgmInstance = CubeGridManager.Instance;

        currentTile = GetComponentInParent<Tile>();
        buildingSide = GetComponent<SideScenario>();
        buildingStats = GetComponent<BuildingStats>();

    }
    private void Start()
    {
        unitPrefab = unitProduceStats.unitPrefab;
        unitSpawnLevel = unitProduceStats.unitSpawnLevel;
        currentUnitLevelData = unitProduceStats.unitLevelData[unitSpawnLevel];
        unitBuildTime = currentUnitLevelData.unitBuildTime;

        unitUpgradeCosts = currentUnitLevelData.unitUpgradeCosts;

        GameObject poolObj = GameObject.FindGameObjectWithTag("UnitPool");

        if (poolObj != null)
            unitPool = poolObj.transform;
        else
            Debug.LogError("No GameObject with tag 'UnitPool' found in scene!");

        if (unitProduceStats != null)
            StartCoroutine(ProductionLoop());


    }

    private IEnumerator ProductionLoop()
    {
        while (autoProduce && buildingStats.currentHealth > 0)
        {
            if (HasResources() || true)
            {
                yield return new WaitForSeconds(unitBuildTime);
                SpawnUnit();
                prmInstance.SpendResources(unitUpgradeCosts);
            }
        }
    }

    private void SpawnUnit()
    {
        if (unitProduceStats == null) return;

        nearestTile = cgmInstance.GetNearestOpenTile(currentTile);

        if (nearestTile == null)
        {
            Debug.Log($"{name} has no open tile to spawn units on!");
            return;
        }

        spawnPoint = nearestTile.transform.position + Vector3.up * 2f;

        GameObject unit = Instantiate(unitPrefab, spawnPoint, Quaternion.identity, transform);

        // Assign side
        SideScenario unitSide = unit.GetComponent<SideScenario>();
        if (unitSide != null && buildingSide != null)
        {
            unitSide.side = buildingSide.side;
        }
    }

    public bool HasResources()
    {
        return prmInstance.HasResources(unitUpgradeCosts);
    }
}