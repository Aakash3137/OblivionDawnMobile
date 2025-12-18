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








// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class MainBuildingSpawner : MonoBehaviour
// {
//     public static MainBuildingSpawner Instance;

//     [Header("Spawn Settings")]
//     public float yOffset = 1f;

//     [Header("Main building slot prefab (empty root with UnitSide + optional helper)")]
//     public GameObject mainBuildingSlotPrefab;

//     [Header("Custom Spawn Positions (world)")]
//     public Transform playerSpawnPoint;
//     public Transform enemySpawnPoint;

//     private FactionButton playerFaction;
//     private FactionButton enemyFaction;

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
//         playerFaction = faction;

//         // Enemy picks randomly but not the same as player
//         var allButtons = Object.FindObjectsByType<FactionButton>(FindObjectsSortMode.None);
//         var possibleEnemies = new List<FactionButton>(allButtons);
//         possibleEnemies.Remove(faction);

//         if (possibleEnemies.Count > 0)
//         {
//             int randomIndex = Random.Range(0, possibleEnemies.Count);
//             enemyFaction = possibleEnemies[randomIndex];
//         }
//         else
//         {
//             enemyFaction = null;
//         }

//         SpawnMainBuildings();
//     }

//     void SpawnMainBuildings()
//     {
//         // Player HQ
//         if (playerSpawnPoint != null && playerFaction != null && mainBuildingSlotPrefab != null)
//         {
//             Vector3 pos = playerSpawnPoint.position + Vector3.up * yOffset;

//             var buildingRoot = Instantiate(mainBuildingSlotPrefab, pos, Quaternion.identity);
//             buildingRoot.name = "PlayerHQ";

//             var unitSide = buildingRoot.GetComponent<UnitSide>();
//             if (unitSide == null) unitSide = buildingRoot.AddComponent<UnitSide>();

//             unitSide.side = Side.Player;
//             unitSide.buildingType = BuildingType.MainBuilding;
//             unitSide.faction = playerFaction;
//             unitSide.AssignFromFaction();
//         }

//         // Enemy HQ
//         if (enemySpawnPoint != null && enemyFaction != null && mainBuildingSlotPrefab != null)
//         {
//             Vector3 pos = enemySpawnPoint.position + Vector3.up * yOffset;

//             var buildingRoot = Instantiate(mainBuildingSlotPrefab, pos, Quaternion.identity);
//             buildingRoot.name = "EnemyHQ";

//             var unitSide = buildingRoot.GetComponent<UnitSide>();
//             if (unitSide == null) unitSide = buildingRoot.AddComponent<UnitSide>();

//             unitSide.side = Side.Enemy;
//             unitSide.buildingType = BuildingType.MainBuilding;
//             unitSide.faction = enemyFaction;
//             unitSide.AssignFromFaction();
//         }
//     }
// }
