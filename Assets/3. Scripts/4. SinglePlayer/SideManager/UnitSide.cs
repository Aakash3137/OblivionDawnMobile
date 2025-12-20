// using UnityEngine;

// public class UnitSide : MonoBehaviour
// {
//     [Header("Building Prefabs")]
//     public GameObject playerSideOBJ_Fab; // assign blue Object prefab in Inspector
//     public GameObject enemySideOBJ_Fab;  // assign red Object prefab in Inspector

//     [Header("Side Settings")]
//     public Side side; // set in Inspector or at runtime

//     private GameObject currentBuilding; // track the instantiated building

//     void Start()
//     {
//         SpawnBuilding();
//     }

//     /// <summary>
//     /// Instantiates the correct building prefab as a child at local (0, -1, 0).
//     /// </summary>
//     public void SpawnBuilding()
//     {
//         // Destroy any existing building instance
//         if (currentBuilding != null)
//         {
//             Destroy(currentBuilding);
//             currentBuilding = null;
//         }

//         GameObject prefabToSpawn = null;
//         if (side == Side.Player && playerSideOBJ_Fab != null)
//         {
//             prefabToSpawn = playerSideOBJ_Fab;
//         }
//         else if (side == Side.Enemy && enemySideOBJ_Fab != null)
//         {
//             prefabToSpawn = enemySideOBJ_Fab;
//         }

//         if (prefabToSpawn != null)
//         {
//             // Instantiate as child of this object
//             currentBuilding = Instantiate(prefabToSpawn, transform);

//             // Set local position to (0, -1, 0)
//             currentBuilding.transform.localPosition = new Vector3(0f, -1f, 0f);
//             currentBuilding.transform.localRotation = Quaternion.identity;
//         }
//     }

//     /// <summary>
//     /// Allows changing side at runtime and updates building accordingly.
//     /// </summary>
//     public void SetSide(Side newSide)
//     {
//         side = newSide;
//         SpawnBuilding();
//     }
// }



// using UnityEngine;

// public class UnitSide : MonoBehaviour
// {
//     [Header("Building Prefabs (runtime injected)")]
//     public GameObject playerSideOBJ_Fab;
//     public GameObject enemySideOBJ_Fab;

//     [Header("Side Settings")]
//     public Side side;

//     private GameObject currentBuilding;

//     void Start()
//     {
//         InjectFromStoreByName();
//         SpawnBuilding();
//     }

//     private void InjectFromStoreByName()
//     {
//         if (GameData.PrefabStore == null)
//         {
//             Debug.LogError("[UnitSide] PrefabStore is null. Ensure PrefabStoreProvider is in the scene.");
//             return;
//         }

//         string buildingName = gameObject.name;

//         if (side == Side.Player)
//         {
//             var prefab = GameData.PrefabStore.GetPlayerByName(buildingName);
//             playerSideOBJ_Fab = prefab;
//             Debug.Log(prefab
//                 ? $"[UnitSide] Player prefab injected: {prefab.name} for {buildingName}"
//                 : $"[UnitSide] No player prefab found for {buildingName}");
//         }
//         else if (side == Side.Enemy)
//         {
//             var prefab = GameData.PrefabStore.GetEnemyByName(buildingName);
//             enemySideOBJ_Fab = prefab;
//             Debug.Log(prefab
//                 ? $"[UnitSide] Enemy prefab injected: {prefab.name} for {buildingName}"
//                 : $"[UnitSide] No enemy prefab found for {buildingName}");
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
//             currentBuilding.transform.localPosition = new Vector3(0f, -1f, 0f);
//             currentBuilding.transform.localRotation = Quaternion.identity;
//         }
//     }

//     public void SetSide(Side newSide)
//     {
//         side = newSide;
//         InjectFromStoreByName();
//         SpawnBuilding();
//     }
// }






using UnityEngine;

public class UnitSide : MonoBehaviour
{
    [Header("Lookup key (set by UI)")]
    public string buildingKey; // e.g., "GoldMine", "Barracks"

    [Header("Building Prefabs (runtime injected)")]
    public GameObject playerSideOBJ_Fab;
    public GameObject enemySideOBJ_Fab;

    [Header("Side Settings")]
    public Side side;

    private GameObject currentBuilding;

    void Start()
    {
        InjectFromStore();
        SpawnBuilding();
    }

    private string EffectiveKey()
    {
        if (!string.IsNullOrEmpty(buildingKey))
            return buildingKey;

        // Fallback to object name, stripping "(Clone)"
        var name = gameObject.name;
        int idx = name.IndexOf("(Clone)");
        return (idx >= 0) ? name.Substring(0, idx).Trim() : name.Trim();
    }

    private void InjectFromStore()
    {
        if (GameData.PrefabStore == null)
        {
            Debug.LogError("[UnitSide] PrefabStore is null. Ensure PrefabStoreProvider is in the scene.");
            return;
        }

        string key = EffectiveKey();
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[UnitSide] buildingKey is empty and name fallback failed.");
            return;
        }

        if (side == Side.Player)
        {
            var prefab = GameData.PrefabStore.GetPlayerByName(key);
            playerSideOBJ_Fab = prefab;
            if (prefab != null)
                Debug.Log($"[UnitSide] Player prefab injected: {prefab.name} for key '{key}'");
            else
                Debug.LogWarning($"[UnitSide] No player prefab found for key '{key}'");
        }
        else if (side == Side.Enemy)
        {
            var prefab = GameData.PrefabStore.GetEnemyByName(key);
            enemySideOBJ_Fab = prefab;
            if (prefab != null)
                Debug.Log($"[UnitSide] Enemy prefab injected: {prefab.name} for key '{key}'");
            else
                Debug.LogWarning($"[UnitSide] No enemy prefab found for key '{key}'");
        }
    }

    public void SpawnBuilding()
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
            currentBuilding = null;
        }

        GameObject prefabToSpawn = (side == Side.Player) ? playerSideOBJ_Fab : enemySideOBJ_Fab;

        if (prefabToSpawn != null)
        {
            currentBuilding = Instantiate(prefabToSpawn, transform);
            currentBuilding.transform.localPosition = Vector3.zero; // adjust if needed
            currentBuilding.transform.localRotation = Quaternion.identity;
        }
    }

    // Optional: call after changing side/key
    public void Refresh()
    {
        InjectFromStore();
        SpawnBuilding();
    }

    public void SetSide(Side newSide)
    {
        side = newSide;
        Refresh();
    }
}
