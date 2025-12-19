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
