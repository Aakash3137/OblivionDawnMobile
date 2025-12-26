using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBuildingSpawner : MonoBehaviour
{
    [Header("Faction Data")]
    public AllFactionsData allFactionsData;   // assign in inspector

    [Header("Building Prefabs (auto-filled)")]
    public GameObject[] enemyBuildingPrefabs; // will be filled at runtime

    [Header("Spawn Settings")]
    public float minDelay = 3f;
    public float maxDelay = 6f;
    public float yOffset = 1f;

    private CubeGridManager grid;
    private UnitSide unitSide; // which side this MainBuilding belongs to
    private Vector2Int myCoord;

    void Start()
    {
        grid = CubeGridManager.Instance;
        unitSide = GetComponent<UnitSide>();

        // Only run if this building is Enemy
        if (unitSide == null || unitSide.side != Side.Enemy)
        {
            enabled = false; // disable script for Player side
            return;
        }

        // Fill enemyBuildingPrefabs based on enemy faction
        PopulateEnemyBuildings();

        // Find grid coord of this MainBuilding
        myCoord = grid.WorldToGrid(transform.position);

        // Claim the tile for Enemy
        GameObject tileObj = grid.GetCube(myCoord);
        if (tileObj != null)
        {
            Tile tile = tileObj.GetComponent<Tile>();
            if (tile != null)
            {
                tile.ownerSide = Side.Enemy;
                tile.hasBuilding = true;
                tile.isOpen = false;
            }
        }

        // Start spawning loop
        StartCoroutine(SpawnLoop());
    }

    private void PopulateEnemyBuildings()
    {
        // Detect which faction this enemy MainBuilding belongs to by prefab name
        string mainName = gameObject.name; // or use prefab reference

        List<GameObject> list = new List<GameObject>();

        if (mainName.Contains("Past"))
        {
            list.Add(allFactionsData.pastGoldMine.prefab);
            list.Add(allFactionsData.pastUnitBuilding.prefab);
            list.Add(allFactionsData.pastTurretBuilding.prefab);
        }
        else if (mainName.Contains("Present"))
        {
            list.Add(allFactionsData.presentGoldMine.prefab);
            list.Add(allFactionsData.presentUnitBuilding.prefab);
            list.Add(allFactionsData.presentTurretBuilding.prefab);
        }
        else if (mainName.Contains("Future"))
        {
            list.Add(allFactionsData.futureGoldMine.prefab);
            list.Add(allFactionsData.futureUnitBuilding.prefab);
            list.Add(allFactionsData.futureTurretBuilding.prefab);
        }
        else if (mainName.Contains("Monster"))
        {
            list.Add(allFactionsData.monsterGoldMine.prefab);
            list.Add(allFactionsData.monsterUnitBuilding.prefab);
            list.Add(allFactionsData.monsterTurretBuilding.prefab);
        }

        enemyBuildingPrefabs = list.ToArray();

        Debug.Log($"[EnemyBuildingSpawner] Enemy faction buildings loaded: {enemyBuildingPrefabs.Length}");
    }

    // private IEnumerator SpawnLoop()
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
    //         TrySpawnBuilding();
    //     }
    // }



    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            // Stop if no enemy buildings remain
            if (!AnyEnemyBuildingsAlive())
            {
                Debug.Log("[EnemyBuildingSpawner] All enemy buildings destroyed. Stopping spawn loop.");
                yield break;
            }

            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            TrySpawnBuilding();
        }
    }

    private bool AnyEnemyBuildingsAlive()
    {
        BuildingHealth[] allBuildings = Object.FindObjectsByType<BuildingHealth>(FindObjectsSortMode.None);
        foreach (var b in allBuildings)
        {
            var side = b.GetComponent<UnitSide>();
            if (side != null && side.side == Side.Enemy && b.currentHealth > 0)
                return true;
        }
        return false;
    }


    private void TrySpawnBuilding()
    {
        // Get neighbors of this MainBuilding tile
        List<Vector2Int> neighbors = grid.GetCardinalNeighbors(myCoord);
        List<Tile> candidateTiles = new List<Tile>();

        foreach (var coord in neighbors)
        {
            GameObject tileObj = grid.GetCube(coord);
            if (tileObj == null) continue;

            Tile tile = tileObj.GetComponent<Tile>();
            if (tile == null) continue;

            if (tile.ownerSide != Side.Enemy) continue;

            if (!tile.hasBuilding)
                tile.SetOpen(true);

            if (tile.isOpen && !tile.hasBuilding)
                candidateTiles.Add(tile);
        }

        if (candidateTiles.Count == 0 || enemyBuildingPrefabs == null || enemyBuildingPrefabs.Length == 0) return;

        // Pick random tile + random building prefab
        Tile chosenTile = candidateTiles[Random.Range(0, candidateTiles.Count)];
        GameObject prefab = enemyBuildingPrefabs[Random.Range(0, enemyBuildingPrefabs.Length)];

        Vector3 pos = chosenTile.transform.position + Vector3.up * yOffset;
        GameObject building = Instantiate(prefab, pos, Quaternion.identity);

        // Assign side
        UnitSide side = building.GetComponent<UnitSide>();
        if (side != null) side.side = Side.Enemy;

        // Apply enemy material from the correct BuildingSlot
        AllFactionsData.BuildingSlot slot = FindSlotForPrefab(prefab);
        if (slot != null)
        {
            Renderer rend = building.GetComponentInChildren<Renderer>();
            if (rend != null && slot.enemyMaterial != null)
            {
                rend.material = slot.enemyMaterial;
            }
        }

        // Mark tile as occupied
        chosenTile.SetBuildingPlaced();

        Debug.Log($"Enemy spawned {prefab.name} at {chosenTile.name}");
    }

    // Helper: find the BuildingSlot that owns this prefab
    private AllFactionsData.BuildingSlot FindSlotForPrefab(GameObject prefab)
    {
        if (prefab == allFactionsData.pastGoldMine.prefab) return allFactionsData.pastGoldMine;
        if (prefab == allFactionsData.pastUnitBuilding.prefab) return allFactionsData.pastUnitBuilding;
        if (prefab == allFactionsData.pastTurretBuilding.prefab) return allFactionsData.pastTurretBuilding;

        if (prefab == allFactionsData.presentGoldMine.prefab) return allFactionsData.presentGoldMine;
        if (prefab == allFactionsData.presentUnitBuilding.prefab) return allFactionsData.presentUnitBuilding;
        if (prefab == allFactionsData.presentTurretBuilding.prefab) return allFactionsData.presentTurretBuilding;

        if (prefab == allFactionsData.futureGoldMine.prefab) return allFactionsData.futureGoldMine;
        if (prefab == allFactionsData.futureUnitBuilding.prefab) return allFactionsData.futureUnitBuilding;
        if (prefab == allFactionsData.futureTurretBuilding.prefab) return allFactionsData.futureTurretBuilding;

        if (prefab == allFactionsData.monsterGoldMine.prefab) return allFactionsData.monsterGoldMine;
        if (prefab == allFactionsData.monsterUnitBuilding.prefab) return allFactionsData.monsterUnitBuilding;
        if (prefab == allFactionsData.monsterTurretBuilding.prefab) return allFactionsData.monsterTurretBuilding;

        return null;
    }

}
