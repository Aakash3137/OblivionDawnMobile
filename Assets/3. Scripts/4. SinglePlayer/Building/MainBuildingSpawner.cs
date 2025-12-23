// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

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

//         Debug.Log($"[Spawner] Game starting...");
//         Debug.Log($"[Spawner] Selected Faction: {GameData.SelectedFactionName ?? "None"}");
//         Debug.Log($"[Spawner] Selected Game Mode: {GameData.GameModeType ?? "None"}");

//         // Player main building from store
//         if (GameData.PrefabStore != null && GameData.PrefabStore.playerMainBuildingPrefab != null)
//         {
//             playerMainBuildingPrefab = GameData.PrefabStore.playerMainBuildingPrefab;
//             Debug.Log($"[Spawner] Player main building prefab loaded: {playerMainBuildingPrefab.name}");
//         }
//         else
//         {
//             playerMainBuildingPrefab = mainBuildingPrefab; // fallback
//             Debug.LogWarning("[Spawner] No player prefab found in store, using fallback.");
//         }

//         // Enemy main building: either explicit or random pick
//         if (GameData.PrefabStore != null && GameData.PrefabStore.enemyMainBuildingPrefab != null)
//         {
//             enemyMainBuildingPrefab = GameData.PrefabStore.enemyMainBuildingPrefab;
//             Debug.Log($"[Spawner] Enemy main building prefab loaded: {enemyMainBuildingPrefab.name}");
//         }
//         else if (GameData.PrefabStore != null && GameData.PrefabStore.PlayerPrefabs.Count > 1)
//         {
//             // Pick random from player prefabs list, excluding the player’s choice
//             var possibleEnemies = new List<GameObject>(GameData.PrefabStore.PlayerPrefabs);
//             possibleEnemies.Remove(playerMainBuildingPrefab);

//             if (possibleEnemies.Count > 0)
//             {
//                 int randomIndex = Random.Range(0, possibleEnemies.Count);
//                 enemyMainBuildingPrefab = possibleEnemies[randomIndex];
//                 Debug.Log($"[Spawner] Enemy prefab chosen randomly: {enemyMainBuildingPrefab.name}");
//             }
//             else
//             {
//                 enemyMainBuildingPrefab = mainBuildingPrefab; // fallback
//                 Debug.LogWarning("[Spawner] No valid enemy prefabs, using fallback.");
//             }
//         }
//         else
//         {
//             enemyMainBuildingPrefab = mainBuildingPrefab; // fallback
//             Debug.LogWarning("[Spawner] Enemy prefab list empty, using fallback.");
//         }

//         // Finally spawn buildings
//         SpawnMainBuildings();
//     }


//     void SpawnMainBuildings()
//     {
//         // Player building
//         if (playerSpawnPoint != null && playerMainBuildingPrefab != null)
//         {
//             Tile tile = playerSpawnPoint.GetComponent<Tile>();
//             if (tile != null)
//             {
//                 Vector3 pos = tile.transform.position + Vector3.up * yOffset;

//                 // Instantiate building as child of the tile
//                 var building = Instantiate(playerMainBuildingPrefab, pos, Quaternion.identity, tile.transform);

//                 var unitSide = building.GetComponent<UnitSide>();
//                 if (unitSide != null)
//                 {
//                     unitSide.side = Side.Player;
//                     Debug.Log("[Spawner] Player building spawned inside tile.");

//                     tile.SetBuildingPlaced();   // mark tile
//                 }
//             }
//         }

//         // Enemy building
//         if (enemySpawnPoint != null && enemyMainBuildingPrefab != null)
//         {
//             Tile tile = enemySpawnPoint.GetComponent<Tile>();
//             if (tile != null)
//             {
//                 Vector3 pos = tile.transform.position + Vector3.up * yOffset;

//                 // Instantiate building as child of the tile
//                 var building = Instantiate(enemyMainBuildingPrefab, pos, Quaternion.identity, tile.transform);

//                 var unitSide = building.GetComponent<UnitSide>();
//                 if (unitSide != null)
//                 {
//                     unitSide.side = Side.Enemy;
//                     Debug.Log("[Spawner] Enemy building spawned inside tile.");

//                     tile.SetBuildingPlaced();   // mark tile
//                 }
//             }
//         }
//     }


// }







// using UnityEngine;

// public class MainBuildingSpawner : MonoBehaviour
// {
//     [Header("Data")]
//     public AllFactionsData data; // assign the single asset in the scene

//     [Header("Faction selections")]
//     public FactionName playerFaction = FactionName.Past;

//     [Header("Spawn points")]
//     public Transform playerSpawnPoint;
//     public Transform enemySpawnPoint;
//     public float yOffset = 1f;

//     void Start()
//     {
//         if (data == null)
//         {
//             Debug.LogError("[Spawner] FactionsData is not assigned.");
//             return;
//         }

//         var playerBlock = GetFactionBlock(playerFaction);
//         var enemyBlock  = GetRandomEnemyFaction(playerFaction);

//         if (playerBlock == null || enemyBlock == null) return;

//         // Spawn all buildings for player
//         SpawnAllBuildings(playerSpawnPoint, playerBlock, Side.Player);

//         // Spawn all buildings for enemy
//         SpawnAllBuildings(enemySpawnPoint, enemyBlock, Side.Enemy);
//     }

//     AllFactionsData.FactionBlock GetFactionBlock(FactionName name)
//     {
//         switch (name)
//         {
//             case FactionName.Past:    return data.past;
//             case FactionName.Present: return data.present;
//             case FactionName.Future:  return data.future;
//             case FactionName.Monster: return data.monster;
//             default: return null;
//         }
//     }

//     AllFactionsData.FactionBlock GetRandomEnemyFaction(FactionName player)
//     {
//         var values = (FactionName[])System.Enum.GetValues(typeof(FactionName));
//         FactionName pick;
//         do
//         {
//             pick = values[Random.Range(0, values.Length)];
//         } while (pick == player);

//         return GetFactionBlock(pick);
//     }

//     void SpawnAllBuildings(Transform rootPoint, AllFactionsData.FactionBlock block, Side side)
//     {
//         if (rootPoint == null || block == null) return;

//         SpawnEntry(rootPoint, block.mainBuilding, side, "MainBuilding");
//         SpawnEntry(rootPoint, block.goldMine, side, "GoldMine");
//         SpawnEntry(rootPoint, block.unitBuilding, side, "UnitBuilding");
//         SpawnEntry(rootPoint, block.turretBuilding, side, "TurretBuilding");
//     }

//     void SpawnEntry(Transform point, AllFactionsData.BuildingSlot slot, Side side, string label)
//     {
//         if (slot == null || slot.prefab == null) return;

//         var pos = point.position + Vector3.up * yOffset;
//         var go  = Instantiate(slot.prefab, pos, Quaternion.identity, point);

//         var unitSide = go.GetComponent<UnitSide>();
//         if (unitSide != null)
//         {
//             unitSide.side = side;
//             unitSide.ApplySideMaterial(slot);
//         }

//         Debug.Log($"[Spawner] Spawned {label} for {side}: {slot.prefab.name}");
//     }
// }



// using UnityEngine;

// public class MainBuildingSpawner : MonoBehaviour
// {
//     public static MainBuildingSpawner Instance { get; private set; }

//     [Header("Data")]
//     public AllFactionsData data;

//     [Header("Faction selections")]
//     public FactionName playerFaction = FactionName.Past;

//     [Header("Spawn points")]
//     public Transform playerSpawnPoint;
//     public Transform enemySpawnPoint;
//     public float yOffset = 1f;

//     void Awake()
//     {
//         Instance = this;
//     }

//     void Start()
//     {
//         if (data == null)
//         {
//             Debug.LogError("[Spawner] FactionsData is not assigned.");
//             return;
//         }

//         var playerBlock = GetFactionBlock(playerFaction);
//         var enemyBlock  = GetRandomEnemyFaction(playerFaction);
//         if (playerBlock == null || enemyBlock == null) return;

//         SpawnAllBuildings(playerSpawnPoint, playerBlock, Side.Player);
//         SpawnAllBuildings(enemySpawnPoint, enemyBlock, Side.Enemy);
//     }

//     AllFactionsData.FactionBlock GetFactionBlock(FactionName name)
//     {
//         switch (name)
//         {
//             case FactionName.Past:    return data.past;
//             case FactionName.Present: return data.present;
//             case FactionName.Future:  return data.future;
//             case FactionName.Monster: return data.monster;
//             default: return null;
//         }
//     }

//     AllFactionsData.FactionBlock GetRandomEnemyFaction(FactionName player)
//     {
//         var values = (FactionName[])System.Enum.GetValues(typeof(FactionName));
//         FactionName pick;
//         do { pick = values[Random.Range(0, values.Length)]; } while (pick == player);
//         return GetFactionBlock(pick);
//     }

//     void SpawnAllBuildings(Transform rootPoint, AllFactionsData.FactionBlock block, Side side)
//     {
//         if (rootPoint == null || block == null) return;
//         SpawnEntry(rootPoint, block.mainBuilding, side, "MainBuilding");
//         SpawnEntry(rootPoint, block.goldMine, side, "GoldMine");
//         SpawnEntry(rootPoint, block.unitBuilding, side, "UnitBuilding");
//         SpawnEntry(rootPoint, block.turretBuilding, side, "TurretBuilding");
//     }

//     void SpawnEntry(Transform point, AllFactionsData.BuildingSlot slot, Side side, string label)
//     {
//         if (slot == null || slot.prefab == null) return;

//         var pos = point.position + Vector3.up * yOffset;
//         var go  = Instantiate(slot.prefab, pos, Quaternion.identity, point);

//         var unitSide = go.GetComponent<UnitSide>();
//         if (unitSide != null)
//         {
//             unitSide.side = side;
//             unitSide.ApplySideMaterial(slot);
//         }

//         Debug.Log($"[Spawner] Spawned {label} for {side}: {slot.prefab.name}");
//     }
// }











using UnityEngine;

public class MainBuildingSpawner : MonoBehaviour
{
    public static MainBuildingSpawner Instance { get; private set; }

    [Header("Data")]
    public AllFactionsData data;

    [Header("Faction selections")]
    public FactionName playerFaction = FactionName.Past;

    [Header("Spawn points")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    public float yOffset = 1f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (data == null)
        {
            Debug.LogError("[Spawner] FactionsData is not assigned.");
            return;
        }

        var playerSlots = GetFactionSlots(playerFaction);
        var enemySlots  = GetFactionSlots(GetRandomEnemyFaction(playerFaction));

        if (playerSlots == null || enemySlots == null) return;

        SpawnAllBuildings(playerSpawnPoint, playerSlots, Side.Player);
        SpawnAllBuildings(enemySpawnPoint, enemySlots, Side.Enemy);
    }

    // Return all 4 building slots for a faction
    AllFactionsData.BuildingSlot[] GetFactionSlots(FactionName name)
    {
        switch (name)
        {
            case FactionName.Past:
                return new [] { data.pastMainBuilding, data.pastGoldMine, data.pastUnitBuilding, data.pastTurretBuilding };
            case FactionName.Present:
                return new [] { data.presentMainBuilding, data.presentGoldMine, data.presentUnitBuilding, data.presentTurretBuilding };
            case FactionName.Future:
                return new [] { data.futureMainBuilding, data.futureGoldMine, data.futureUnitBuilding, data.futureTurretBuilding };
            case FactionName.Monster:
                return new [] { data.monsterMainBuilding, data.monsterGoldMine, data.monsterUnitBuilding, data.monsterTurretBuilding };
            default:
                return null;
        }
    }

    FactionName GetRandomEnemyFaction(FactionName player)
    {
        var values = (FactionName[])System.Enum.GetValues(typeof(FactionName));
        FactionName pick;
        do { pick = values[Random.Range(0, values.Length)]; } while (pick == player);
        return pick;
    }

    void SpawnAllBuildings(Transform rootPoint, AllFactionsData.BuildingSlot[] slots, Side side)
    {
        if (rootPoint == null || slots == null) return;

        SpawnEntry(rootPoint, slots[0], side, "MainBuilding");
        SpawnEntry(rootPoint, slots[1], side, "GoldMine");
        SpawnEntry(rootPoint, slots[2], side, "UnitBuilding");
        SpawnEntry(rootPoint, slots[3], side, "TurretBuilding");
    }

    void SpawnEntry(Transform point, AllFactionsData.BuildingSlot slot, Side side, string label)
    {
        if (slot == null || slot.prefab == null) return;

        var pos = point.position + Vector3.up * yOffset;
        var go  = Instantiate(slot.prefab, pos, Quaternion.identity, point);

        var unitSide = go.GetComponent<UnitSide>();
        if (unitSide != null)
        {
            unitSide.side = side;
            unitSide.ApplySideMaterial(slot);
        }

        Debug.Log($"[Spawner] Spawned {label} for {side}: {slot.prefab.name}");
    }
}
