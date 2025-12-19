// using UnityEngine;
// using System.Collections;

// public class MainBuildingSpawner : MonoBehaviour
// {
//     public static MainBuildingSpawner Instance;

//     [Header("Default Prefabs (fallback)")]
//     public GameObject mainBuildingPrefab;

//     [Header("Spawn Settings")]
//     public float yOffset = 1f;

//     [Header("Custom Spawn Positions (world)")]
//     public Transform playerSpawnPoint;
//     public Transform enemySpawnPoint;

//     private GameObject playerMainBuildingPrefab;
//     private GameObject enemyMainBuildingPrefab;

//     private void Awake()
//     {
//         Instance = this;
//     }

//     IEnumerator Start()
//     {
//         yield return null; // wait one frame for tiles to register
//         SpawnMainBuildings();
//     }

//     public void SetPlayerFaction(FactionButton faction)
//     {
//         playerMainBuildingPrefab = faction.mainBuildingPrefab;

//         // Enemy picks randomly but not the same as player
//         var allButtons = Object.FindObjectsByType<FactionButton>(FindObjectsSortMode.None);
//         var possibleEnemies = new System.Collections.Generic.List<FactionButton>(allButtons);
//         possibleEnemies.Remove(faction);

//         int randomIndex = Random.Range(0, possibleEnemies.Count);
//         enemyMainBuildingPrefab = possibleEnemies[randomIndex].mainBuildingPrefab;

//         // Respawn buildings after selection
//         SpawnMainBuildings();
//     }


//     void SpawnMainBuildings()
//     {
//         // Player building
//         if (playerSpawnPoint != null && playerMainBuildingPrefab != null)
//         {
//             Vector3 pos = playerSpawnPoint.position + Vector3.up * yOffset;
//             var building = Instantiate(playerMainBuildingPrefab, pos, Quaternion.identity);
//             building.GetComponent<UnitSide>().side = Side.Player;
//         }

//         // Enemy building
//         if (enemySpawnPoint != null && enemyMainBuildingPrefab != null)
//         {
//             Vector3 pos = enemySpawnPoint.position + Vector3.up * yOffset;
//             var building = Instantiate(enemyMainBuildingPrefab, pos, Quaternion.identity);
//             building.GetComponent<UnitSide>().side = Side.Enemy;
//         }
//     }
// }


using UnityEngine;
using System.Collections;

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

    // IEnumerator Start()
    // {
    //     yield return null; // wait one frame for tiles to register

    //     // Load player choice from GameData
    //     if (GameData.SelectedMainBuildingPrefab != null)
    //         playerMainBuildingPrefab = GameData.SelectedMainBuildingPrefab;
    //     else
    //         playerMainBuildingPrefab = mainBuildingPrefab; // fallback

    //     // Pick random enemy faction (different from player)
    //     var allButtons = Object.FindObjectsByType<FactionButton>(FindObjectsSortMode.None);
    //     if (allButtons.Length > 0)
    //     {
    //         var possibleEnemies = new System.Collections.Generic.List<FactionButton>(allButtons);
    //         possibleEnemies.RemoveAll(f => f.mainBuildingPrefab == playerMainBuildingPrefab);

    //         if (possibleEnemies.Count > 0)
    //         {
    //             int randomIndex = Random.Range(0, possibleEnemies.Count);
    //             enemyMainBuildingPrefab = possibleEnemies[randomIndex].mainBuildingPrefab;
    //         }
    //     }

    //     SpawnMainBuildings();
    // }






    // IEnumerator Start()
    // {
    //     yield return null; // wait one frame for tiles to register

    //     // Debug logs for player choice
    //     Debug.Log($"[Spawner] Game starting...");
    //     Debug.Log($"[Spawner] Selected Faction: {GameData.SelectedFactionName ?? "None"}");
    //     Debug.Log($"[Spawner] Selected Game Mode: {GameData.GameModeType ?? "None"}");

    //     // Load player choice from GameData
    //     if (GameData.SelectedMainBuildingPrefab != null)
    //     {
    //         playerMainBuildingPrefab = GameData.SelectedMainBuildingPrefab;
    //         Debug.Log("[Spawner] Player main building prefab loaded from GameData.");
    //     }
    //     else
    //     {
    //         playerMainBuildingPrefab = mainBuildingPrefab; // fallback
    //         Debug.LogWarning("[Spawner] No player prefab found in GameData, using fallback prefab.");
    //     }

    //     // Pick random enemy faction (different from player)
    //     var allButtons = Object.FindObjectsByType<FactionButton>(FindObjectsSortMode.None);
    //     if (allButtons.Length > 0)
    //     {
    //         var possibleEnemies = new System.Collections.Generic.List<FactionButton>(allButtons);
    //         possibleEnemies.RemoveAll(f => f.mainBuildingPrefab == playerMainBuildingPrefab);

    //         if (possibleEnemies.Count > 0)
    //         {
    //             int randomIndex = Random.Range(0, possibleEnemies.Count);
    //             enemyMainBuildingPrefab = possibleEnemies[randomIndex].mainBuildingPrefab;
    //             Debug.Log($"[Spawner] Enemy faction chosen: {possibleEnemies[randomIndex].name}");
    //         }
    //         else
    //         {
    //             Debug.LogWarning("[Spawner] No valid enemy factions available.");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogWarning("[Spawner] No FactionButtons found in scene to pick enemy faction.");
    //     }

    //     // Finally spawn buildings
    //     SpawnMainBuildings();
    // }






    IEnumerator Start()
    {
        yield return null; // wait one frame for tiles to register

        // Debug logs for player choice
        Debug.Log($"[Spawner] Game starting...");
        Debug.Log($"[Spawner] Selected Faction: {GameData.SelectedFactionName ?? "None"}");
        Debug.Log($"[Spawner] Selected Game Mode: {GameData.GameModeType ?? "None"}");

        // Load player choice from GameData
        if (GameData.SelectedMainBuildingPrefab != null)
        {
            playerMainBuildingPrefab = GameData.SelectedMainBuildingPrefab;
            Debug.Log("[Spawner] Player main building prefab loaded from GameData.");
        }
        else
        {
            playerMainBuildingPrefab = mainBuildingPrefab; // fallback
            Debug.LogWarning("[Spawner] No player prefab found in GameData, using fallback prefab.");
        }

        // Pick random enemy faction (different from player)
        if (GameData.AllFactionPrefabs != null && GameData.AllFactionPrefabs.Count > 1)
        {
            var possibleEnemies = new System.Collections.Generic.List<GameObject>(GameData.AllFactionPrefabs);
            possibleEnemies.Remove(playerMainBuildingPrefab);

            if (possibleEnemies.Count > 0)
            {
                int randomIndex = Random.Range(0, possibleEnemies.Count);
                enemyMainBuildingPrefab = possibleEnemies[randomIndex];
                Debug.Log($"[Spawner] Enemy prefab chosen: {enemyMainBuildingPrefab.name}");
            }
            else
            {
                Debug.LogWarning("[Spawner] No valid enemy prefabs available.");
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
            Vector3 pos = playerSpawnPoint.position + Vector3.up * yOffset;
            var building = Instantiate(playerMainBuildingPrefab, pos, Quaternion.identity);
            building.GetComponent<UnitSide>().side = Side.Player;
        }

        // Enemy building
        if (enemySpawnPoint != null && enemyMainBuildingPrefab != null)
        {
            Vector3 pos = enemySpawnPoint.position + Vector3.up * yOffset;
            var building = Instantiate(enemyMainBuildingPrefab, pos, Quaternion.identity);
            building.GetComponent<UnitSide>().side = Side.Enemy;
        }
    }
}
