// using UnityEngine;
// using System.Collections;

// public class MainBuildingSpawner : MonoBehaviour
// {
//     [Header("Prefabs")]
//     public GameObject mainBuildingPrefab;

//     [Header("Spawn Settings")]
//     public float yOffset = 0.5f;

//     IEnumerator Start()
//     {
//         // Wait one frame so all Tile.Start() has run
//         yield return null;

//         SpawnMainBuildings();
//     }

//     void SpawnMainBuildings()
//     {
//         if (HexGridManager.Instance.hexTiles.Count == 0)
//         {
//             Debug.LogError("No tiles registered in HexGridManager!");
//             return;
//         }

//         // Bottom-left tile by world position
//         var playerTile = GetBottomLeftTile();
//         // Top-right tile by world position
//         var enemyTile = GetTopRightTile();

//         if (playerTile != null)
//         {
//             Vector3 pos = playerTile.transform.position + Vector3.up * yOffset;
//             var building = Instantiate(mainBuildingPrefab, pos, Quaternion.identity, playerTile.transform);
//             building.GetComponent<UnitSide>().side = Side.Player;
//             Debug.Log("Spawned Player building at " + playerTile.transform.position);
//         }

//         if (enemyTile != null)
//         {
//             Vector3 pos = enemyTile.transform.position + Vector3.up * yOffset;
//             var building = Instantiate(mainBuildingPrefab, pos, Quaternion.identity, enemyTile.transform);
//             building.GetComponent<UnitSide>().side = Side.Enemy;
//             Debug.Log("Spawned Enemy building at " + enemyTile.transform.position);
//         }
//     }

//     GameObject GetBottomLeftTile()
//     {
//         // Find tile with lowest x+z sum
//         GameObject tile = null;
//         float best = float.MaxValue;
//         foreach (var t in HexGridManager.Instance.hexTiles.Values)
//         {
//             float score = t.transform.position.x + t.transform.position.z;
//             if (score < best)
//             {
//                 best = score;
//                 tile = t;
//             }
//         }
//         return tile;
//     }

//     GameObject GetTopRightTile()
//     {
//         // Find tile with highest x+z sum
//         GameObject tile = null;
//         float best = float.MinValue;
//         foreach (var t in HexGridManager.Instance.hexTiles.Values)
//         {
//             float score = t.transform.position.x + t.transform.position.z;
//             if (score > best)
//             {
//                 best = score;
//                 tile = t;
//             }
//         }
//         return tile;
//     }
// }





// using System.Collections;
// using UnityEngine;

// public class MainBuildingSpawner : MonoBehaviour
// {
//     [Header("Prefabs")]
//     public GameObject mainBuildingPrefab;

//     [Header("Spawn Settings")]
//     public float yOffset = 0.5f;

//     [Header("Custom Spawn Coordinates")]
//     public Vector2Int playerSpawnCoord;   // e.g. (0,0)
//     public Vector2Int enemySpawnCoord;    // e.g. (5,5)

//     IEnumerator Start()
//     {
//         yield return null; // wait one frame for tiles to register
//         SpawnMainBuildings();
//     }

//     void SpawnMainBuildings()
//     {
//         // Player building
//         GameObject playerTile = HexGridManager.Instance.GetHex(playerSpawnCoord);
//         if (playerTile != null)
//         {
//             Vector3 pos = playerTile.transform.position + Vector3.up * yOffset;
//             var building = Instantiate(mainBuildingPrefab, pos, Quaternion.identity, playerTile.transform);
//             building.GetComponent<UnitSide>().side = Side.Player;
//         }

//         // Enemy building
//         GameObject enemyTile = HexGridManager.Instance.GetHex(enemySpawnCoord);
//         if (enemyTile != null)
//         {
//             Vector3 pos = enemyTile.transform.position + Vector3.up * yOffset;
//             var building = Instantiate(mainBuildingPrefab, pos, Quaternion.identity, enemyTile.transform);
//             building.GetComponent<UnitSide>().side = Side.Enemy;
//         }
//     }
// }






// using UnityEngine;
// using System.Collections;

// public class MainBuildingSpawner : MonoBehaviour
// {
//     [Header("Prefabs")]
//     public GameObject mainBuildingPrefab;

//     [Header("Spawn Settings")]
//     public float yOffset = 1f;

//     [Header("Custom Spawn Positions (world)")]
//     public Transform playerSpawnPoint;   // drag a marker placed on the tile
//     public Transform enemySpawnPoint;    // drag a marker placed on the tile

//     IEnumerator Start()
//     {
//         yield return null; // wait one frame for tiles to register
//         SpawnMainBuildings();
//     }

//     void SpawnMainBuildings()
//     {
//         // Player building
//         if (playerSpawnPoint != null)
//         {
//             Vector3 pos = playerSpawnPoint.position + Vector3.up * yOffset;
//             var building = Instantiate(mainBuildingPrefab, pos, Quaternion.identity);
//             building.GetComponent<UnitSide>().side = Side.Player;
//         }

//         // Enemy building
//         if (enemySpawnPoint != null)
//         {
//             Vector3 pos = enemySpawnPoint.position + Vector3.up * yOffset;
//             var building = Instantiate(mainBuildingPrefab, pos, Quaternion.identity);
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

    IEnumerator Start()
    {
        yield return null; // wait one frame for tiles to register
        SpawnMainBuildings();
    }

    public void SetPlayerFaction(FactionButton faction)
    {
        playerMainBuildingPrefab = faction.mainBuildingPrefab;

        // Enemy picks randomly but not the same as player
        var allButtons = Object.FindObjectsByType<FactionButton>(FindObjectsSortMode.None);
        var possibleEnemies = new System.Collections.Generic.List<FactionButton>(allButtons);
        possibleEnemies.Remove(faction);

        int randomIndex = Random.Range(0, possibleEnemies.Count);
        enemyMainBuildingPrefab = possibleEnemies[randomIndex].mainBuildingPrefab;

        // Respawn buildings after selection
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
