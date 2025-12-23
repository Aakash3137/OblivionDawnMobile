// using UnityEngine;

// public class UnitSide : MonoBehaviour
// {
//     [Header("Lookup key (set by UI)")]
//     public string buildingKey; // e.g., "GoldMine", "Barracks"

//     [Header("Building Prefabs (runtime injected)")]
//     public GameObject playerSideOBJ_Fab;
//     public GameObject enemySideOBJ_Fab;

//     [Header("Side Settings")]
//     public Side side;

//     private GameObject currentBuilding;

//     void Start()
//     {
//         InjectFromStore();
//         SpawnBuilding();
//     }

//     private string EffectiveKey()
//     {
//         if (!string.IsNullOrEmpty(buildingKey))
//             return buildingKey;

//         // Fallback to object name, stripping "(Clone)"
//         var name = gameObject.name;
//         int idx = name.IndexOf("(Clone)");
//         return (idx >= 0) ? name.Substring(0, idx).Trim() : name.Trim();
//     }

//     private void InjectFromStore()
//     {
//         if (GameData.PrefabStore == null)
//         {
//             Debug.LogError("[UnitSide] PrefabStore is null. Ensure PrefabStoreProvider is in the scene.");
//             return;
//         }

//         string key = EffectiveKey();
//         if (string.IsNullOrEmpty(key))
//         {
//             Debug.LogWarning("[UnitSide] buildingKey is empty and name fallback failed.");
//             return;
//         }

//         if (side == Side.Player)
//         {
//             var prefab = GameData.PrefabStore.GetPlayerByName(key);
//             playerSideOBJ_Fab = prefab;
//             if (prefab != null)
//                 Debug.Log($"[UnitSide] Player prefab injected: {prefab.name} for key '{key}'");
//             else
//                 Debug.LogWarning($"[UnitSide] No player prefab found for key '{key}'");
//         }
//         else if (side == Side.Enemy)
//         {
//             var prefab = GameData.PrefabStore.GetEnemyByName(key);
//             enemySideOBJ_Fab = prefab;
//             if (prefab != null)
//                 Debug.Log($"[UnitSide] Enemy prefab injected: {prefab.name} for key '{key}'");
//             else
//                 Debug.LogWarning($"[UnitSide] No enemy prefab found for key '{key}'");
//         }
//     }

//     public void SpawnBuilding()
//     {
//         if (currentBuilding != null)
//         {
//             Destroy(currentBuilding);
//             currentBuilding = null;
//         }

//         GameObject prefabToSpawn = (side == Side.Player) ? playerSideOBJ_Fab : enemySideOBJ_Fab;

//         if (prefabToSpawn != null)
//         {
//             currentBuilding = Instantiate(prefabToSpawn, transform);
//             currentBuilding.transform.localPosition = new Vector3(0, -1, 0);
//             currentBuilding.transform.localRotation = Quaternion.identity;
//         }
//         else
//         {
//             Debug.LogWarning($"[UnitSide] No prefab to spawn for side {side} and key {EffectiveKey()}");
//         }
//     }


//     // Optional: call after changing side/key
//     public void Refresh()
//     {
//         InjectFromStore();
//         SpawnBuilding();
//     }

//     public void SetSide(Side newSide)
//     {
//         side = newSide;
//         Refresh();
//     }
// }






// using UnityEngine;

// public class UnitSide : MonoBehaviour
// {
//     [Header("Side Settings")]
//     public Side side; // Player or Enemy

//     [Header("Faction + Building Type")]
//     public FactionName faction;      // Past, Present, Future, Monster
//     public string buildingType;      // "MainBuilding", "GoldMine", "UnitBuilding", "TurretBuilding"

//     private GameObject currentBuilding;

//     void Start()
//     {
//         SpawnBuilding();
//     }

//     public void SpawnBuilding()
//     {
//         if (currentBuilding != null)
//         {
//             Destroy(currentBuilding);
//             currentBuilding = null;
//         }

//         // Get the faction block from the global data
//         var block = GameData.AllFactionsData != null
//             ? GetFactionBlock(GameData.AllFactionsData, faction)
//             : null;

//         if (block == null)
//         {
//             Debug.LogError($"[UnitSide] Faction {faction} not found in AllFactionsData.");
//             return;
//         }

//         // Pick the correct building slot
//         var slot = GetBuildingSlot(block, buildingType);
//         if (slot == null || slot.prefab == null)
//         {
//             Debug.LogWarning($"[UnitSide] No prefab found for {buildingType} in faction {faction}.");
//             return;
//         }

//         // Spawn prefab
//         currentBuilding = Instantiate(slot.prefab, transform);
//         currentBuilding.transform.localPosition = Vector3.zero;
//         currentBuilding.transform.localRotation = Quaternion.identity;

//         // Apply correct material
//         ApplySideMaterial(slot);
//     }

//     private AllFactionsData.FactionBlock GetFactionBlock(AllFactionsData data, FactionName name)
//     {
//         switch (name)
//         {
//             case FactionName.Past: return data.past;
//             case FactionName.Present: return data.present;
//             case FactionName.Future: return data.future;
//             case FactionName.Monster: return data.monster;
//             default: return null;
//         }
//     }

//     private AllFactionsData.BuildingSlot GetBuildingSlot(AllFactionsData.FactionBlock block, string type)
//     {
//         switch (type)
//         {
//             case "MainBuilding": return block.mainBuilding;
//             case "GoldMine": return block.goldMine;
//             case "UnitBuilding": return block.unitBuilding;
//             case "TurretBuilding": return block.turretBuilding;
//             default: return null;
//         }
//     }

//     public void ApplySideMaterial(AllFactionsData.BuildingSlot slot)
//     {
//         var renderers = currentBuilding.GetComponentsInChildren<Renderer>(true);
//         var mat = (side == Side.Player) ? slot.playerMaterial : slot.enemyMaterial;

//         if (mat == null) return;

//         foreach (var r in renderers)
//         {
//             r.material = mat;
//         }
//     }
// }

using UnityEngine;

public class UnitSide : MonoBehaviour
{
    [Header("Side Settings")]
    public Side side; // Player or Enemy

    [Header("Faction + Building Type")]
    public FactionName faction;      // Past, Present, Future, Monster
    public string buildingType;      // "MainBuilding", "GoldMine", "UnitBuilding", "TurretBuilding"

    private GameObject currentBuilding;

    void Start()
    {
        SpawnBuilding();
    }

    public void SpawnBuilding()
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
            currentBuilding = null;
        }

        var slot = GetBuildingSlot(GameData.AllFactionsData, faction, buildingType);
        if (slot == null || slot.prefab == null)
        {
            Debug.LogWarning($"[UnitSide] No prefab found for {buildingType} in faction {faction}.");
            return;
        }

        currentBuilding = Instantiate(slot.prefab, transform);
        currentBuilding.transform.localPosition = Vector3.zero;
        currentBuilding.transform.localRotation = Quaternion.identity;

        ApplySideMaterial(slot);
    }

    private AllFactionsData.BuildingSlot GetBuildingSlot(AllFactionsData data, FactionName name, string type)
    {
        switch (name)
        {
            case FactionName.Past:
                if (type == "MainBuilding") return data.pastMainBuilding;
                if (type == "GoldMine") return data.pastGoldMine;
                if (type == "UnitBuilding") return data.pastUnitBuilding;
                if (type == "TurretBuilding") return data.pastTurretBuilding;
                break;

            case FactionName.Present:
                if (type == "MainBuilding") return data.presentMainBuilding;
                if (type == "GoldMine") return data.presentGoldMine;
                if (type == "UnitBuilding") return data.presentUnitBuilding;
                if (type == "TurretBuilding") return data.presentTurretBuilding;
                break;

            case FactionName.Future:
                if (type == "MainBuilding") return data.futureMainBuilding;
                if (type == "GoldMine") return data.futureGoldMine;
                if (type == "UnitBuilding") return data.futureUnitBuilding;
                if (type == "TurretBuilding") return data.futureTurretBuilding;
                break;

            case FactionName.Monster:
                if (type == "MainBuilding") return data.monsterMainBuilding;
                if (type == "GoldMine") return data.monsterGoldMine;
                if (type == "UnitBuilding") return data.monsterUnitBuilding;
                if (type == "TurretBuilding") return data.monsterTurretBuilding;
                break;
        }
        return null;
    }

    public void ApplySideMaterial(AllFactionsData.BuildingSlot slot)
    {
        var renderers = currentBuilding.GetComponentsInChildren<Renderer>(true);
        var mat = (side == Side.Player) ? slot.playerMaterial : slot.enemyMaterial;

        if (mat == null) return;

        foreach (var r in renderers)
        {
            r.material = mat;
        }
    }
}
