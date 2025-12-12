// using UnityEngine;

// public class UnitSide : MonoBehaviour
// {
//     [Header("Building Prefabs")]
//     public GameObject playerBuilding; // assign blue building prefab in Inspector
//     public GameObject enemyBuilding;  // assign red building prefab in Inspector

//     [Header("Side Settings")]
//     public Side side; // set in Inspector or at runtime

//     void Start()
//     {
//         ToggleBuilding();
//     }

//     /// <summary>
//     /// Activates the correct building prefab based on side.
//     /// </summary>
//     public void ToggleBuilding()
//     {
//         if (side == Side.Player)
//         {
//             if (playerBuilding != null) playerBuilding.SetActive(true);
//             if (enemyBuilding != null) enemyBuilding.SetActive(false);
//         }
//         else if (side == Side.Enemy)
//         {
//             if (playerBuilding != null) playerBuilding.SetActive(false);
//             if (enemyBuilding != null) enemyBuilding.SetActive(true);
//         }
//     }

//     /// <summary>
//     /// Allows changing side at runtime and updates building accordingly.
//     /// </summary>
//     public void SetSide(Side newSide)
//     {
//         side = newSide;
//         ToggleBuilding();
//     }
// }



using UnityEngine;

public class UnitSide : MonoBehaviour
{
    [Header("Building Prefabs")]
    public GameObject playerSideOBJ_Fab; // assign blue Object prefab in Inspector
    public GameObject enemySideOBJ_Fab;  // assign red Object prefab in Inspector

    [Header("Side Settings")]
    public Side side; // set in Inspector or at runtime

    private GameObject currentBuilding; // track the instantiated building

    void Start()
    {
        SpawnBuilding();
    }

    /// <summary>
    /// Instantiates the correct building prefab as a child at local (0, -1, 0).
    /// </summary>
    public void SpawnBuilding()
    {
        // Destroy any existing building instance
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
            currentBuilding = null;
        }

        GameObject prefabToSpawn = null;
        if (side == Side.Player && playerSideOBJ_Fab != null)
        {
            prefabToSpawn = playerSideOBJ_Fab;
        }
        else if (side == Side.Enemy && enemySideOBJ_Fab != null)
        {
            prefabToSpawn = enemySideOBJ_Fab;
        }

        if (prefabToSpawn != null)
        {
            // Instantiate as child of this object
            currentBuilding = Instantiate(prefabToSpawn, transform);

            // Set local position to (0, -1, 0)
            currentBuilding.transform.localPosition = new Vector3(0f, -1f, 0f);
            currentBuilding.transform.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// Allows changing side at runtime and updates building accordingly.
    /// </summary>
    public void SetSide(Side newSide)
    {
        side = newSide;
        SpawnBuilding();
    }
}



// using UnityEngine;

// public class UnitSide : MonoBehaviour
// {
//     [Header("Building Variants")]
//     public GameObject[] playerVariants; // assign blue variants in Inspector
//     public GameObject[] enemyVariants;  // assign red variants in Inspector

//     [Header("Side Settings")]
//     public Side side; // set in Inspector or at runtime

//     private GameObject activeBuilding;

//     void Start()
//     {
//         ActivateVariant();
//     }

//     private void ActivateVariant()
//     {
//         // Disable previous active building
//         if (activeBuilding != null) activeBuilding.SetActive(false);

//         // Choose correct array
//         GameObject[] variants = side == Side.Player ? playerVariants : enemyVariants;
//         if (variants == null || variants.Length == 0) return;

//         // Randomly select one
//         int index = Random.Range(0, variants.Length);
//         activeBuilding = variants[index];
//         activeBuilding.SetActive(true);
//     }

//     public void SetSide(Side newSide)
//     {
//         side = newSide;
//         ActivateVariant();
//     }
// }
