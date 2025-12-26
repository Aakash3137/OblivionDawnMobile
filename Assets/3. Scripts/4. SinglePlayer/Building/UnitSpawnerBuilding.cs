using UnityEngine;
using System.Collections;

public class UnitSpawnerBuilding : MonoBehaviour
{
    [Header("Production Settings")]
    public UnitData unitToProduce;   // ScriptableObject reference
    public Transform spawnPoint;     // empty child where units appear
    public bool autoProduce = true;  // keep producing continuously

    private UnitSide buildingSide;   // building's faction marker
    private Transform unitPool;      // found by tag "UnitPool"

    // Reference to the faction's MainBuilding
    private BuildingHealth mainBuildingHealth;

    // void Awake()
    // {
    //     // Building itself must have UnitSide attached
    //     buildingSide = GetComponent<UnitSide>();
    //     if (buildingSide == null)
    //         Debug.LogError($"Building {name} missing UnitSide. Assign Player/Enemy.");

    //     // Find UnitPool by tag
    //     GameObject poolObj = GameObject.FindGameObjectWithTag("UnitPool");
    //     if (poolObj != null)
    //     {
    //         unitPool = poolObj.transform;
    //     }
    //     else
    //     {
    //         Debug.LogError("No GameObject with tag 'UnitPool' found in scene!");
    //     }

    //     // Find this side's MainBuilding
    //     GameObject[] allBuildings = GameObject.FindGameObjectsWithTag("MainBuilding");
    //     foreach (var b in allBuildings)
    //     {
    //         UnitSide side = b.GetComponent<UnitSide>();
    //         if (side != null && buildingSide != null && side.side == buildingSide.side)
    //         {
    //             mainBuildingHealth = b.GetComponent<BuildingHealth>();
    //             break;
    //         }
    //     }
    // }


    void Awake()
    {
        buildingSide = GetComponent<UnitSide>();
        if (buildingSide == null)
            Debug.LogError($"Building {name} missing UnitSide. Assign Player/Enemy.");

        // Find UnitPool by tag
        GameObject poolObj = GameObject.FindGameObjectWithTag("UnitPool");
        if (poolObj != null)
        {
            unitPool = poolObj.transform;
        }
        else
        {
            Debug.LogError("No GameObject with tag 'UnitPool' found in scene!");
        }

        // Find the ENEMY side's MainBuilding instead of ours
        GameObject[] allBuildings = GameObject.FindGameObjectsWithTag("MainBuilding");
        foreach (var b in allBuildings)
        {
            UnitSide side = b.GetComponent<UnitSide>();
            if (side != null && buildingSide != null && side.side != buildingSide.side)
            {
                mainBuildingHealth = b.GetComponent<BuildingHealth>();
                Debug.Log($"{buildingSide.side} spawner linked to ENEMY MainBuilding {mainBuildingHealth?.name}");
                break;
            }
        }
    }


    void Start()
    {
        if (autoProduce && unitToProduce != null)
            StartCoroutine(ProductionLoop());
    }

    // private IEnumerator ProductionLoop()
    // {
    //     while (true)
    //     {
    //         // Stop producing if main building is gone
    //         if (mainBuildingHealth == null || mainBuildingHealth.currentHealth <= 0)
    //         {
    //             Debug.Log($"{buildingSide.side} production stopped: MainBuilding destroyed.");
    //             yield break; // exit coroutine
    //         }

    //         yield return new WaitForSeconds(unitToProduce.buildTime);
    //         SpawnUnit();
    //     }
    // }


    private IEnumerator ProductionLoop()
    {
        while (true)
        {
            // Stop producing if no player buildings remain
            if (!AnyPlayerBuildingsAlive())
            {
                Debug.Log($"{buildingSide.side} production stopped: All player buildings destroyed.");
                yield break;
            }

            yield return new WaitForSeconds(unitToProduce.buildTime);
            SpawnUnit();
        }
    }

    private bool AnyPlayerBuildingsAlive()
    {
        BuildingHealth[] allBuildings = Object.FindObjectsByType<BuildingHealth>(FindObjectsSortMode.None);
        foreach (var b in allBuildings)
        {
            var side = b.GetComponent<UnitSide>();
            if (side != null && side.side == Side.Player && b.currentHealth > 0)
                return true;
        }
        return false;
    }


    private void SpawnUnit()
    {
        // NEW CHECK: don't spawn if main building is gone.
        if (mainBuildingHealth == null || mainBuildingHealth.currentHealth <= 0)
        {
            Debug.Log($"{buildingSide.side} tried to spawn unit but MainBuilding is destroyed.");
            return;
        }
        if (unitToProduce == null || spawnPoint == null) return;

        GameObject unit = Instantiate(unitToProduce.prefab, spawnPoint.position, Quaternion.identity);

        if (unitPool != null)
            unit.transform.SetParent(unitPool);

        // Assign side
        UnitSide unitSide = unit.GetComponent<UnitSide>();
        if (unitSide != null && buildingSide != null)
        {
            unitSide.side = buildingSide.side;
            SideManager manager = FindAnyObjectByType<SideManager>();
            if (manager != null) manager.SetSide(unit, unitSide.side);
        }

        // Assign stats
        UnitStats stats = unit.GetComponent<UnitStats>();
        if (stats != null)
        {
            stats.health = unitToProduce.health;
            stats.attackPower = unitToProduce.attackPower;
        }

        // Assign AI combat properties
        AIUnit ai = unit.GetComponent<AIUnit>();
        if (ai != null)
        {
            ai.attackDamage = unitToProduce.attackPower;
            ai.attackRange = unitToProduce.attackRange;
            ai.attackInterval = unitToProduce.attackInterval;
        }

        Debug.Log($"{buildingSide?.side} building produced {unitToProduce.unitName} inside UnitPool");
    }
}