using UnityEngine; // V.6
using System.Collections;

public class Building : MonoBehaviour
{
    [Header("Production Settings")]
    public UnitData unitToProduce;   // ScriptableObject reference
    public Transform spawnPoint;     // empty child where units appear
    public bool autoProduce = true;  // keep producing continuously

    private UnitSide buildingSide;   // building's faction marker
    private Transform unitPool;      // found by tag "UnitPool"

    void Awake()
    {
        // Building itself must have UnitSide attached
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
    }

    void Start()
    {
        if (autoProduce && unitToProduce != null)
            StartCoroutine(ProductionLoop());
    }

    private IEnumerator ProductionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(unitToProduce.buildTime);
            SpawnUnit();
        }
    }

    // private void SpawnUnit()
    // {
    //     if (unitToProduce == null || spawnPoint == null) return;

    //     // Instantiate prefab at spawn point
    //     GameObject unit = Instantiate(unitToProduce.prefab, spawnPoint.position, Quaternion.identity);

    //     // Parent under UnitPool if found
    //     if (unitPool != null)
    //     {
    //         unit.transform.SetParent(unitPool);
    //     }

    //     // Assign side from building → unit
    //     UnitSide unitSide = unit.GetComponent<UnitSide>();
    //     if (unitSide != null && buildingSide != null)
    //     {
    //         unitSide.side = buildingSide.side;

    //         // Apply correct material immediately
    //         SideManager manager = FindAnyObjectByType<SideManager>();
    //         if (manager != null)
    //         {
    //             manager.SetSide(unit, unitSide.side);
    //         }
    //     }

    //     // Assign stats from ScriptableObject
    //     UnitStats stats = unit.GetComponent<UnitStats>();
    //     if (stats != null)
    //     {
    //         stats.health = unitToProduce.health;
    //         stats.attackPower = unitToProduce.attackPower;
    //     }

    //     // Sync AI attackDamage with stats.attackPower if present
    //     AIUnit ai = unit.GetComponent<AIUnit>();
    //     if (ai != null && stats != null)
    //     {
    //         ai.attackDamage = stats.attackPower;
    //     }

    //     Debug.Log($"{buildingSide?.side} building produced {unitToProduce.unitName} inside UnitPool");
    // }





    private void SpawnUnit()
    {
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
