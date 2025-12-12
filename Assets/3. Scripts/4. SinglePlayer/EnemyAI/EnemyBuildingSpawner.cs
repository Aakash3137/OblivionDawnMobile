using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBuildingSpawner : MonoBehaviour
{
    [Header("Building Prefabs")]
    public GameObject[] enemyBuildingPrefabs; // assign in inspector

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

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            TrySpawnBuilding();
        }
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

            // Only allow tiles that belong to Enemy
            if (tile.ownerSide != Side.Enemy) continue;

            // Mark tile open for AI (but PlusIcon won't show because it's Enemy side)
            if (!tile.hasBuilding)
                tile.SetOpen(true);

            // Must be open and not already occupied
            if (tile.isOpen && !tile.hasBuilding)
            {
                candidateTiles.Add(tile);
            }
        }

        if (candidateTiles.Count == 0) return;

        // Pick random tile + random building prefab
        Tile chosenTile = candidateTiles[Random.Range(0, candidateTiles.Count)];
        GameObject prefab = enemyBuildingPrefabs[Random.Range(0, enemyBuildingPrefabs.Length)];

        Vector3 pos = chosenTile.transform.position + Vector3.up * yOffset;
        GameObject building = Instantiate(prefab, pos, Quaternion.identity);

        // Assign side
        UnitSide side = building.GetComponent<UnitSide>();
        if (side != null) side.side = Side.Enemy;

        // Mark tile as occupied
        chosenTile.SetBuildingPlaced();

        Debug.Log($"Enemy spawned {prefab.name} at {chosenTile.name}");
    }
}
