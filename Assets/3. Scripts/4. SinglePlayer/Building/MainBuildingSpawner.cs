using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainBuildingSpawner : MonoBehaviour
{
    public static MainBuildingSpawner Instance;

    [Header("Default Prefabs (fallback)")]
    public GameObject mainBuildingPrefab;

    [Header("Spawn Settings")]
    public float yOffset = 1f;

    [Header("Custom Spawn Positions (world)")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    private GameObject playerMainBuildingPrefab;
    private GameObject enemyMainBuildingPrefab;

    private void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        yield return null; // wait one frame for tiles to register

        Debug.Log($"[Spawner] Game starting...");
        Debug.Log($"[Spawner] Selected Faction: {GameData.SelectedFactionName ?? "None"}");
        Debug.Log($"[Spawner] Selected Game Mode: {GameData.GameModeType ?? "None"}");

        // Player main building from store
        if (GameData.PrefabStore != null && GameData.PrefabStore.playerMainBuildingPrefab != null)
        {
            playerMainBuildingPrefab = GameData.PrefabStore.playerMainBuildingPrefab;
            Debug.Log($"[Spawner] Player main building prefab loaded: {playerMainBuildingPrefab.name}");
        }
        else
        {
            playerMainBuildingPrefab = mainBuildingPrefab; // fallback
            Debug.LogWarning("[Spawner] No player prefab found in store, using fallback.");
        }

        // Enemy main building: either explicit or random pick
        if (GameData.PrefabStore != null && GameData.PrefabStore.enemyMainBuildingPrefab != null)
        {
            enemyMainBuildingPrefab = GameData.PrefabStore.enemyMainBuildingPrefab;
            Debug.Log($"[Spawner] Enemy main building prefab loaded: {enemyMainBuildingPrefab.name}");
        }
        else if (GameData.PrefabStore != null && GameData.PrefabStore.PlayerPrefabs.Count > 1)
        {
            // Pick random from player prefabs list, excluding the player’s choice
            var possibleEnemies = new List<GameObject>(GameData.PrefabStore.PlayerPrefabs);
            possibleEnemies.Remove(playerMainBuildingPrefab);

            if (possibleEnemies.Count > 0)
            {
                int randomIndex = Random.Range(0, possibleEnemies.Count);
                enemyMainBuildingPrefab = possibleEnemies[randomIndex];
                Debug.Log($"[Spawner] Enemy prefab chosen randomly: {enemyMainBuildingPrefab.name}");
            }
            else
            {
                enemyMainBuildingPrefab = mainBuildingPrefab; // fallback
                Debug.LogWarning("[Spawner] No valid enemy prefabs, using fallback.");
            }
        }
        else
        {
            enemyMainBuildingPrefab = mainBuildingPrefab; // fallback
            Debug.LogWarning("[Spawner] Enemy prefab list empty, using fallback.");
        }

        // Finally spawn buildings
        SpawnMainBuildings();
    }


    void SpawnMainBuildings()
    {
        // Player building
        if (playerSpawnPoint != null && playerMainBuildingPrefab != null)
        {
            Tile tile = playerSpawnPoint.GetComponent<Tile>();
            if (tile != null)
            {
                Vector3 pos = tile.transform.position + Vector3.up * yOffset;

                // Instantiate building as child of the tile
                var building = Instantiate(playerMainBuildingPrefab, pos, Quaternion.identity, tile.transform);

                var unitSide = building.GetComponent<UnitSide>();
                if (unitSide != null)
                {
                    unitSide.side = Side.Player;
                    Debug.Log("[Spawner] Player building spawned inside tile.");

                    tile.SetBuildingPlaced();   // mark tile
                }
            }
        }

        // Enemy building
        if (enemySpawnPoint != null && enemyMainBuildingPrefab != null)
        {
            Tile tile = enemySpawnPoint.GetComponent<Tile>();
            if (tile != null)
            {
                Vector3 pos = tile.transform.position + Vector3.up * yOffset;

                // Instantiate building as child of the tile
                var building = Instantiate(enemyMainBuildingPrefab, pos, Quaternion.identity, tile.transform);

                var unitSide = building.GetComponent<UnitSide>();
                if (unitSide != null)
                {
                    unitSide.side = Side.Enemy;
                    Debug.Log("[Spawner] Enemy building spawned inside tile.");

                    tile.SetBuildingPlaced();   // mark tile
                }
            }
        }
    }


}
