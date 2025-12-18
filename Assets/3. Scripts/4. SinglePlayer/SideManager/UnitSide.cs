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








using UnityEngine;

public class UnitSide : MonoBehaviour
{
    [Header("Side Settings")]
    public Side side;   // Player or Enemy

    private GameObject currentBuilding;

    /// <summary>
    /// Assign prefab from faction based on this object's name and side.
    /// </summary>
    public void AssignBuildingFromFaction(FactionButton faction)
    {
        if (faction == null)
        {
            Debug.LogWarning($"{name}: No faction assigned.");
            return;
        }

        // Clear previous child if any
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
            currentBuilding = null;
        }

        GameObject prefabToSpawn = null;

        // Decide prefab based on object name
        if (gameObject.name.Contains("Auric Syndicate (MainBuilding)"))
        {
            prefabToSpawn = faction.mainBuildingPrefab;
        }
        else
        {
            // For other buildings, match by name + side suffix
            string suffix = side == Side.Player ? "_Blue" : "_Red";
            string expectedName = gameObject.name + suffix;
            // e.g. "Turret_Blue", "GoldMine_Red"

            foreach (var prefab in faction.buildingPrefabs)
            {
                if (prefab != null && prefab.name == expectedName)
                {
                    prefabToSpawn = prefab;
                    break;
                }
            }
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"{name}: No prefab found for {gameObject.name} (side={side}) in faction {faction.name}. Check names and assignments.");
            return;
        }

        currentBuilding = Instantiate(prefabToSpawn, transform);
        currentBuilding.transform.localPosition = Vector3.zero;
        currentBuilding.transform.localRotation = Quaternion.identity;
    }
}
