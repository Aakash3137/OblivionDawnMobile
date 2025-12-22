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
            currentBuilding.transform.localPosition = new Vector3(0, -1, 0);
            currentBuilding.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning($"[UnitSide] No prefab to spawn for side {side} and key {EffectiveKey()}");
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
