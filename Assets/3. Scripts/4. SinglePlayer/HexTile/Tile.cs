using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Ownership")]
    public Side ownerSide;            // Current owner of the tile

    [Header("Visuals")]
    public Renderer tileRenderer;     // Renderer for the tile mesh

    private SideManager sideManager;
    public bool isOpen = false; // set true when PlusIcon is activated

    public bool hasBuilding = false; // NEW flag

    void Start()
    {
        sideManager = FindAnyObjectByType<SideManager>();

        if (CubeGridManager.Instance != null)
        {
            var coord = CubeGridManager.Instance.WorldToGrid(transform.position);
            CubeGridManager.Instance.RegisterCube(coord, gameObject);
        }

        if (tileRenderer == null)
            tileRenderer = GetComponentInChildren<Renderer>();

        ApplyOwnerMaterial();
    }

    // public void SetOpen(bool open)
    // {
    //     isOpen = open;
    //     Transform plusIcon = transform.Find("Cube/Plus_Icon");
    //     if (plusIcon != null)
    //         plusIcon.gameObject.SetActive(open);
    // }



    // public void SetOpen(bool open)
    // {
    //     isOpen = open;

    //     Transform plusIcon = transform.Find("Cube/Plus_Icon");
    //     if (plusIcon != null)
    //     {
    //         // Only show PlusIcon for Player side
    //         if (ownerSide == Side.Player)
    //         {
    //             plusIcon.gameObject.SetActive(open);
    //         }
    //         else
    //         {
    //             // Enemy side never shows PlusIcon
    //             plusIcon.gameObject.SetActive(false);
    //         }
    //     }
    // }




    public void SetOpen(bool open)
    {
        isOpen = open;

        Transform plusIcon = transform.Find("Cube/Plus_Icon");
        if (plusIcon != null)
        {
            // Only show PlusIcon for Player side
            if (ownerSide == Side.Player)
            {
                plusIcon.gameObject.SetActive(open);
            }
            else
            {
                // Enemy side never shows PlusIcon
                plusIcon.gameObject.SetActive(false);
            }
        }

        // notify TileCounterUI if available
        if (TileCounterUI.Instance != null)
        {
            // Just trigger a refresh of counts whenever open state changes
            TileCounterUI.Instance.InitializeCounts();
        }

        //// Debug.Log($"Tile at {transform.position} open={isOpen}, side={ownerSide}");
    }




    // Apply correct material based on ownerSide
    public void ApplyOwnerMaterial()
    {
        if (sideManager != null && tileRenderer != null)
            sideManager.SetSide(gameObject, ownerSide);
    }

    // Flip ownership
    public void SetOwner(Side newOwner)
    {
        ownerSide = newOwner;
        ApplyOwnerMaterial();
    }

    // Called when a unit steps onto this tile
    public void Occupy(Side unitSide)
    {
        // Always flip ownership to the entering unit’s side
        // occupant = unit;   // update occupant reference
        SetOwner(unitSide);
    }

    // Called when a unit leaves
    public void Vacate(Side unitSide)
    {
        // Clear occupant only if this unit was tracked
        // if (occupant == unit)
        //     occupant = null;
    }


    // Called when a building is placed
    public void SetBuildingPlaced()
    {
        isOpen = false;
        hasBuilding = true;

        // Hide PlusIcon if present
        Transform cubeChild = transform.Find("Cube");
        if (cubeChild != null)
        {
            Transform plusIcon = cubeChild.Find("Plus_Icon");
            if (plusIcon != null) plusIcon.gameObject.SetActive(false);
        }
    }
}







// using UnityEngine;
// using System.Collections.Generic;

// public class Tile : MonoBehaviour
// {
//     [Header("Ownership")]
//     public Side ownerSide;            // Current owner of the tile

//     [Header("Visuals")]
//     public Renderer tileRenderer;     // Renderer for the tile mesh

//     private SideManager sideManager;
//     private UnitSide occupant;        // Track current occupant (optional)

//     public bool isOpen = false;       // set true when PlusIcon is activated
//     public bool hasBuilding = false;  // NEW flag

//     // NEW: Track walls by position
//     private Dictionary<Vector3, GameObject> walls = new Dictionary<Vector3, GameObject>();

//     void Start()
//     {
//         sideManager = FindAnyObjectByType<SideManager>();

//         if (CubeGridManager.Instance != null)
//         {
//             var coord = CubeGridManager.Instance.WorldToGrid(transform.position);
//             CubeGridManager.Instance.RegisterCube(coord, gameObject);
//         }

//         if (tileRenderer == null)
//             tileRenderer = GetComponentInChildren<Renderer>();

//         ApplyOwnerMaterial();
//     }

//     public void SetOpen(bool open)
//     {
//         isOpen = open;

//         Transform plusIcon = transform.Find("Cube/Plus_Icon");
//         if (plusIcon != null)
//         {
//             if (ownerSide == Side.Player)
//                 plusIcon.gameObject.SetActive(open);
//             else
//                 plusIcon.gameObject.SetActive(false);
//         }

//         if (TileCounterUI.Instance != null)
//             TileCounterUI.Instance.InitializeCounts();

//         Debug.Log($"Tile at {transform.position} open={isOpen}, side={ownerSide}");
//     }

//     public void ApplyOwnerMaterial()
//     {
//         if (sideManager != null && tileRenderer != null)
//             sideManager.SetSide(gameObject, ownerSide);
//     }

//     public void SetOwner(Side newOwner)
//     {
//         ownerSide = newOwner;
//         ApplyOwnerMaterial();
//     }

//     public void Occupy(UnitSide unit)
//     {
//         SetOwner(unit.side);
//     }

//     public void Vacate(UnitSide unit) { }

//     public void SetBuildingPlaced()
//     {
//         hasBuilding = true;
//         isOpen = false;

//         Transform cubeChild = transform.Find("Cube");
//         if (cubeChild != null)
//         {
//             Transform plusIcon = cubeChild.Find("Plus_Icon");
//             if (plusIcon != null) plusIcon.gameObject.SetActive(false);
//         }
//     }

//     // ---------------- WALL MANAGEMENT ----------------

//     public bool HasWallAt(Vector3 pos) => walls.ContainsKey(pos);

//     public void RegisterWall(Vector3 pos, GameObject wall)
//     {
//         if (!walls.ContainsKey(pos))
//             walls[pos] = wall;
//     }

//     public void RemoveWallAt(Vector3 pos)
//     {
//         if (walls.TryGetValue(pos, out GameObject wall))
//         {
//             Destroy(wall);
//             walls.Remove(pos);
//         }
//     }

//     public void ClearAllWalls()
//     {
//         foreach (var wall in walls.Values)
//             Destroy(wall);
//         walls.Clear();
//     }
// }
